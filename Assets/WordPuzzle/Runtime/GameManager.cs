using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace WordPuzzle
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private GameBoard _gameBoard;

        [Inject]
        private IPuzzleManager _puzzleManager;

        private CancellationTokenSource _gameCancellation = new();

        private Button _quitButton;

        private IPuzzleCollection _puzzleCollection;

        [SerializeField]
        private PuzzleSet _tutorialPuzzles;

        [Inject]
        public void SetPuzzleManager(IPuzzleManager puzzleManager)
        {
            _puzzleManager = puzzleManager;
        }

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

            var tutorialPlayed = PlayerPrefs.GetInt(PlayerKeys.TUTORIAL_PLAYED, 0) != 0;
            if (_tutorialPuzzles && !tutorialPlayed)
                _gameBoard.Play(_tutorialPuzzles);
        }

        public void SetWordLength(int wordLength)
        {
            if (_puzzleManager.TryGetPuzzles(wordLength, out var puzzleCollection))
            {
                _puzzleCollection = puzzleCollection;
            }
        }
    }

    public static class PlayerKeys
    {
        public const string TUTORIAL_PLAYED = "tutorial_played";
    }
}