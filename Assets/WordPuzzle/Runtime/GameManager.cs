using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace WordPuzzle
{
    public class GameManager : MonoBehaviour
    {
        private const int default_word_length = 6;
        
        [SerializeField]
        private GameBoard _gameBoard;

        [FormerlySerializedAs("_tutorialPuzzles")]
        [SerializeField]
        private PuzzleSet _builtInPuzzles;

        [Inject]
        private IPuzzleManager _puzzleManager;
        [Inject]
        private IPopUpMenu _popUpMenu;

        private IPuzzleCollection _selectedPuzzleCollection;

        private UniTask _playTask;
        private CancellationTokenSource _gameCancellation = new();

        private void Start()
        {
            if (_puzzleManager is null)
            {
                Debug.LogError($"Injection failed: puzzle provider not found");
                return;
            }

            var linkedCancellation =
                    CancellationTokenSource.CreateLinkedTokenSource(
                            _gameCancellation.Token,
                            this.GetCancellationTokenOnDestroy());
            
            var wordLength = PlayerPrefs.GetInt(PlayerKeys.WORD_LENGTH, default_word_length);

            if (!_puzzleManager.TryGetPuzzles(wordLength, out var puzzleCollection))
            {
                Debug.LogError($"No puzzles with word length {wordLength}");
                return;
            }
            
            _selectedPuzzleCollection = puzzleCollection;
            _selectedPuzzleCollection.ResetProgress();

            var tutorialPlayed = PlayerPrefs.GetInt(PlayerKeys.TUTORIAL_PLAYED, 0) != 0;

            _builtInPuzzles.ResetProgress();
            
            if (!tutorialPlayed)
            {
                var tutorialPuzzle = _puzzleManager.GetTutorial() ?? _builtInPuzzles.NextPuzzle().Value;
                _playTask = _gameBoard.Play(tutorialPuzzle, true, linkedCancellation.Token)
                       .ContinueWith(() =>
                                PlayerPrefs.SetInt(PlayerKeys.TUTORIAL_PLAYED, 1));
            }

            _playTask = _playTask.ContinueWith(() => Play(_selectedPuzzleCollection, linkedCancellation.Token));
        }

        private async UniTask Play(IPuzzleCollection puzzles, CancellationToken ct)
        {
            while (puzzles.NextPuzzle() is { HasValue: true } puzzleOption)
            {
                var showEndGameScreen = puzzles.PuzzlesRemainig > 0;
                await _gameBoard.Play(puzzleOption.Value, showEndGameScreen, ct);
            }
            
            _popUpMenu.Show(GameState.PuzzleCollectionSolved, ct);
        }

        private void OnDestroy()
        {
            _gameCancellation.Cancel(false);
        }
    }

    public static class PlayerKeys
    {
        public const string TUTORIAL_PLAYED = "tutorial_played";
        public const string WORD_LENGTH = "word_length";
    }
}