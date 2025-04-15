using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace WordPuzzle
{
    public class LoadingScreenController : MonoInstaller, ILoadProgressHandler
    {
        private CanvasGroup _rootCanvas;
        
        [SerializeField]
        private Image _loadingBar;

        [FormerlySerializedAs("_actionTitle")]
        [SerializeField]
        private TMP_Text _message;
        [SerializeField]
        private string _defaultMessage = "LOADING...";

        public void Show()
        {
            _rootCanvas.enabled = true;
            _rootCanvas.alpha = 1;
            _rootCanvas.blocksRaycasts = true;
        }

        public void SetProgress(float progress, string message = default)
        {
            _loadingBar.fillAmount = progress;

            if (!string.IsNullOrEmpty(message))
            {
                _message.text = message;
            }
        }

        public void SetMessage(string message)
        {
            _message.text = message;
        }
        
        public UniTask Hide(float fadeOutSeconds = 0)
        {
            fadeOutSeconds = Mathf.Clamp(fadeOutSeconds, 0, float.PositiveInfinity);
            if (fadeOutSeconds > 0)
            { 
                return _rootCanvas.DOFade(0, fadeOutSeconds)
                       .OnComplete(SetHide)
                       .ToUniTask();
            }
            else
            {
                SetHide();
            }
            return UniTask.CompletedTask;
        }

        private void SetHide()
        {
            _rootCanvas.alpha = 0;
            _rootCanvas.blocksRaycasts = false;
        }
        // public override void InstallBindings()
        // {
        //     ProjectContext.Instance.Container.QueueForInject(this);
        // }

        private void Awake()
        {
            _rootCanvas = GetComponentInParent<CanvasGroup>();
            SetProgress(0);
            SetMessage(_defaultMessage);
            Hide();
        }

        private void OnDestroy()
        {
            var container = ProjectContext.Instance.Container;
            if (container.HasBinding<ILoadProgressHandler>() &&
                container.Resolve<ILoadProgressHandler>() is {} impl &&  
                impl.Equals(this))
                container.Unbind<ILoadProgressHandler>();
        }
    }
}