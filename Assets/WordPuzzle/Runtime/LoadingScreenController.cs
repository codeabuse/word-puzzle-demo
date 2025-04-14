using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WordPuzzle
{
    public class LoadingScreenController : MonoBehaviour
    {
        private static LoadingScreenController _instance;
        public static LoadingScreenController Instance => _instance;
        
        [SerializeField]
        private Image _loadingBar;

        [SerializeField]
        private TMP_Text _actionTitle;


        public void SetProgress(float progress)
        {
            _loadingBar.fillAmount = progress;
        }

        public void SetAction(string action)
        {
            _actionTitle.text = action;
        }

        private void Awake()
        {
            _instance = this;
        }
    }
}