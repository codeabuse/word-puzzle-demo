using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WordPuzzle
{
    public class PopupMenuController : MonoBehaviour, IPopUpMenu
    {
        [SerializeField]
        private string _allPuzzlesSolvedText = "CONGRATULATIONS! ALL PUZZLES SOLVED!";
        [SerializeField]
        private string _puzzleSolvedText = "PUZZLE SOLVED!";
        [SerializeField]
        private string _pauseText = "Pause";
        [SerializeField]
        private string _continueText = "Continue";
        [SerializeField]
        private string _nextPuzzleText = "Next puzzle";
        
        [SerializeField]
        private TMP_Text _title;
        
        [SerializeField]
        private TMP_Text _message;

        [SerializeField]
        private Button _continueButton;
        [SerializeField]
        private Button _backToMenuButton;

        [SerializeField]
        private Transform _solvedPuzzleDisplayRoot;

        [SerializeField]
        private SceneLoader _menuSceneLoader;

        private TMP_Text _continueButtonText;

        public Transform SolvedPuzzleDisplayRoot => _solvedPuzzleDisplayRoot;

        private void Awake()
        {
            _continueButtonText = _continueButton.GetComponentInChildren<TMP_Text>();
            _backToMenuButton.onClick.AddListener(_menuSceneLoader.Load);
        }

        private void Start()
        {
            gameObject.SetActive(false);
        }

        
        public UniTask Show(GameState state, CancellationToken cancellationToken)
        {
            _continueButton.onClick.RemoveAllListeners();
            switch (state)
            {
                case GameState.Playing:
                    _message.text = _pauseText;
                    _continueButtonText.text = _continueText;
                    _continueButton.gameObject.SetActive(true);
                    _continueButtonText.gameObject.SetActive(true);
                    break;
                case GameState.PuzzleSolved:
                    _message.text = _puzzleSolvedText;
                    _continueButtonText.text = _nextPuzzleText;
                    _continueButton.gameObject.SetActive(true);
                    _continueButtonText.gameObject.SetActive(true);
                    break;
                case GameState.PuzzleCollectionSolved:
                    _message.text = _allPuzzlesSolvedText;
                    _continueButton.gameObject.SetActive(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
            
            _continueButton.onClick.AddListener(() => gameObject.SetActive(false));
            gameObject.SetActive(true);
            
            return UniTask.WhenAny(
                    _continueButton.OnClickAsync(cancellationToken),
                    _backToMenuButton.OnClickAsync(cancellationToken));
        }

        /// <summary>
        /// Do not call before <see cref="Show"/> method since button events are removed.
        /// </summary>
        /// <returns></returns>
        public UniTask WaitContinueCommand(CancellationToken ct)
        {
            return _continueButton.OnClickAsync(ct);
        }

        
        /// <summary>
        /// Do not call before <see cref="Show"/> method since button events are removed.
        /// </summary>
        /// <returns></returns>
        public UniTask WaitBackToMenuCommand(CancellationToken ct)
        {
            return _backToMenuButton.OnClickAsync(ct);
        }
    }
}