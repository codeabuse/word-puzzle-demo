using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Zenject;

namespace WordPuzzle
{
    public class SettingsController : MonoBehaviour
    {
        [SerializeField]
        private TMP_Dropdown _wordsLengthDropdown;

        private readonly Dictionary<int, int> _lengths = new();

        [Inject]
        private IPuzzleManager _puzzleManager;

        private void Start()
        {
            _wordsLengthDropdown.ClearOptions();
            _lengths.Clear();
            _wordsLengthDropdown.onValueChanged.RemoveAllListeners();
            var availableWordsLengths = _puzzleManager.AvailablePuzzlesWordLength;
            var index = 0;
            var options = UnityEngine.Pool.ListPool<TMP_Dropdown.OptionData>.Get();
            
            foreach (var length in availableWordsLengths)
            {
                options.Add(new TMP_Dropdown.OptionData(length.ToString()));
                _lengths.Add(index++, length);
            }
            _wordsLengthDropdown.AddOptions(options);
            
            UnityEngine.Pool.ListPool<TMP_Dropdown.OptionData>.Release(options);
            _wordsLengthDropdown.onValueChanged.AddListener(WordLengthChanged);

            var lastPlayedWordLength = PlayerPrefs.GetInt(PlayerKeys.WORD_LENGTH, availableWordsLengths.First());

            _wordsLengthDropdown.value = _lengths.First(kvp => kvp.Value == lastPlayedWordLength).Key;
        }

        private void WordLengthChanged(int index)
        {
            PlayerPrefs.SetInt(PlayerKeys.WORD_LENGTH, _lengths[index]);
        }

        public void ResetTutorial()
        {
            PlayerPrefs.SetInt(PlayerKeys.TUTORIAL_PLAYED, 0);
        }

        public void Show(bool value)
        {
            gameObject.SetActive(value);
        }
    }
}