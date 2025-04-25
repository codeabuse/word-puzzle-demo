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
        }

        private void Start()
        {
            _backToMenuButton.onClick.AddListener(_menuSceneLoader.Load);
            _backToMenuButton.onClick.AddListener(()=> Show(false));
            _continueButton.onClick.AddListener(()=> Show(false));
            Show(false);
        }

        public void Show(GameState state)
        {
            _continueButton.onClick.RemoveAllListeners();
            switch (state)
            {
                case GameState.Playing:
                    _message.text = _pauseText;
                    _continueButtonText.gameObject.SetActive(true);
                    break;
                case GameState.PuzzleSolved:
                    _message.text = _puzzleSolvedText;
                    _continueButtonText.text = "NEXT PUZZLE";
                    _continueButtonText.gameObject.SetActive(true);
                    _continueButton.OnClickAsync().ContinueWith(() => Show(false));
                    break;
                case GameState.PuzzleCollectionSolved:
                    _message.text = _allPuzzlesSolvedText;
                    _continueButtonText.gameObject.SetActive(false);
                    _continueButton.OnClickAsync().ContinueWith(() => Show(false));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
            Show(true);
        }

        public UniTask WaitContinueCommand(CancellationToken ct)
        {
            return _continueButton.OnClickAsync(ct);
        }

        public UniTask WaitBackToMenuCommand(CancellationToken ct)
        {
            return _backToMenuButton.OnClickAsync(ct);
        }

        private void Show(bool value)
        {
            gameObject.SetActive(value);
        }
    }
}