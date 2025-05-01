using System;
using Codeabuse.SceneManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Zenject;

namespace WordPuzzle
{
    public class SceneLoader : MonoBehaviour, IStartupProcedure
    {
        [SerializeField]
        private LoadSceneOptions[] _loadSceneOptions;
        
        [Inject]
        private ILoadProgressHandler _loadingProgressHandler;

        async UniTask IStartupProcedure.Load()
        {
            var counter = 0;
            foreach (var loadSceneOption in _loadSceneOptions)
            {
                if (( loadSceneOption.IsAddressable && loadSceneOption.AssetReference is null) ||string.IsNullOrEmpty(loadSceneOption.Scene.Name))
                {
                    Debug.LogWarning("Scene loader has no scene to load!", this);
                    return;
                }

                _loadingProgressHandler?.Show();
                _loadingProgressHandler?.SetProgress((float)++counter / _loadSceneOptions.Length, $"LOADING {loadSceneOption.Scene.Name}");
                
                await loadSceneOption.LoadAsync();
            }
        }

        public void Load()
        {
            ((IStartupProcedure)this).Load().Forget();
        }
    }

    [Serializable]
    public struct LoadSceneOptions
    {
        [SerializeField]
        private BuildScene _scene;
        [SerializeField]
        private AssetReference _assetReference;
        [SerializeField]
        private LoadSceneMode _loadMode;
        [SerializeField]
        private bool _isAddressable;

        public BuildScene Scene => _scene;

        public LoadSceneMode LoadMode
        {
            get => _loadMode;
            set => _loadMode = value;
        }

        public bool IsAddressable => _isAddressable;

        public AssetReference AssetReference => _assetReference;

        public UniTask LoadAsync()
        {
            if (_isAddressable)
            {
                return Addressables.LoadSceneAsync(_assetReference.RuntimeKey, _loadMode, true)
                       .ToUniTask();
            }
            
            return SceneManager.LoadSceneAsync(_scene.Name, _loadMode).ToUniTask();
        }
    }
}