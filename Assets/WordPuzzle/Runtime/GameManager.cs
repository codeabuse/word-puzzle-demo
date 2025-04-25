using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace WordPuzzle
{
    public class GameManager : MonoBehaviour
    {
        private const int default_word_length = 6;
        
        [SerializeField]
        private GameBoard _gameBoard;

        [SerializeField]
        private PuzzleSet _tutorialPuzzles;

        [Inject]
        private IPuzzleManager _puzzleManager;

        private IPuzzleCollection _puzzleCollection;

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
            
            SetWordLength(default_word_length);

            var tutorialPlayed = PlayerPrefs.GetInt(PlayerKeys.TUTORIAL_PLAYED, 0) != 0;

            var playingTutorial = _tutorialPuzzles && !tutorialPlayed;
            var puzzles = playingTutorial ? _tutorialPuzzles : _puzzleCollection;
            _playTask = _gameBoard.Play(puzzles, linkedCancellation.Token);
            
            if (playingTutorial)
            {
                _playTask.ContinueWith(() =>
                        PlayerPrefs.SetInt(PlayerKeys.TUTORIAL_PLAYED, 1));
            }
        }

        private void SetWordLength(int wordLength)
        {
            if (_puzzleManager.TryGetPuzzles(wordLength, out var puzzleCollection))
            {
                _puzzleCollection = puzzleCollection;
            }
        }

        public void Play()
        {
            var linkedCancellation =
                    CancellationTokenSource.CreateLinkedTokenSource(
                            _gameCancellation.Token,
                            this.GetCancellationTokenOnDestroy());

            _playTask = PlayLoop(linkedCancellation.Token);
        }

        private async UniTask PlayLoop(CancellationToken ct)
        {
            await _gameBoard.Play(_puzzleCollection, ct);
        }

        public void ResetTutorial()
        {
            PlayerPrefs.SetInt(PlayerKeys.TUTORIAL_PLAYED, 0);
        }
    }

    public static class PlayerKeys
    {
        public const string TUTORIAL_PLAYED = "tutorial_played";
    }
}