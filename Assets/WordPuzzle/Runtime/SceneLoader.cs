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
        private ILoadProgressHandler _loadProgressHandler;

        [Inject]
        public void SetLoadingProgressHandler(ILoadProgressHandler loadProgressHandler)
        {
            _loadProgressHandler = loadProgressHandler;
        }

        public async UniTask Load()
        {
            var counter = 0;
            foreach (var loadSceneOptions in _loadSceneOptions)
            {
                if (string.IsNullOrEmpty(loadSceneOptions.Scene.Name))
                {
                    Debug.LogWarning("Scene loader has no scene to load!", this);
                    return;
                }

                _loadProgressHandler?.Show();
                _loadProgressHandler?.SetProgress((float)++counter / _loadSceneOptions.Length, $"LOADING {loadSceneOptions.Scene.Name}");
                if (loadSceneOptions.IsAddressable)
                {
                    await Addressables.LoadSceneAsync(loadSceneOptions.AssetReference.RuntimeKey, loadSceneOptions.LoadMode, true)
                           .ToUniTask();
                }
                else
                {
                    await SceneManager.LoadSceneAsync(loadSceneOptions.Scene.Name, loadSceneOptions.LoadMode);
                }
            }
        }

        public void LoadScenes()
        {
            Load().Forget();
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
    }
}