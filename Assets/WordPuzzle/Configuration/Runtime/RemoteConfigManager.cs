using System;
using Cysharp.Threading.Tasks;
using Unity.Services.Authentication;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.RemoteConfig;
using Zenject;

namespace WordPuzzle.Configuration
{
    public struct AppAttributes
    {
        public string version;
    }

    public struct UserAttributes { }

    public class RemoteConfigManager : MonoBehaviour, IStartupProcedure
    {
        [Inject]
        private ILoadProgressHandler _loadingProgressHandler;
        private bool _configFetchCompleted;
        private const int retry_time_seconds = 2;
        private const int retry_attempts = 5;

        [Inject]
        public void SetLoadingProgressHandler(ILoadProgressHandler loadProgressHandler)
        {
            _loadingProgressHandler = loadProgressHandler;
        }

        public async UniTask Load()
        {
            _loadingProgressHandler?.SetProgress(0f,"CONNECTING...");
            int retryConnect = retry_attempts;
            var retryDelay = TimeSpan.FromSeconds(retry_time_seconds);
            while (retryConnect > 0 && !Utilities.CheckForInternetConnection())
            {
                Debug.Log(
                        $"No internet connection available, retrying in {retry_time_seconds}s ({retryConnect} attempts remaining)");
                _loadingProgressHandler?.SetProgress(0f,$"CONNECTING...(attempts remaining: {retryConnect})");
                await UniTask.Delay(retryDelay);
                --retryConnect;
            }

            if (!Utilities.CheckForInternetConnection())
            {
                Debug.LogError("No internet connection available");
                _loadingProgressHandler?.SetMessage("CHECK INTERNET CONNECTION");
                return;
            }

            switch (UnityServices.State)
            {
                case ServicesInitializationState.Uninitialized:
                    break;
                case ServicesInitializationState.Initializing:
                    return;
                case ServicesInitializationState.Initialized:
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            await InitServices();

            RemoteConfigService.Instance.FetchCompleted -= OnFetchConfig;
            RemoteConfigService.Instance.FetchCompleted += OnFetchConfig;
            RemoteConfigService.Instance.FetchConfigs(new UserAttributes(), new AppAttributes()
            {
                    version = Application.version
            });
            await UniTask.WaitUntil(() => _configFetchCompleted);
        }

        private async UniTask InitServices()
        {
            _loadingProgressHandler?.SetProgress(.15F, "CHECKING FOR UPDATES...");
            await UnityServices.InitializeAsync();

            if (!AuthenticationService.Instance.IsAuthorized)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
        }

        private void OnFetchConfig(ConfigResponse response)
        {
            switch (response.requestOrigin)
            {
                case ConfigOrigin.Default:
                    break;
                case ConfigOrigin.Cached:
                    break;
                case ConfigOrigin.Remote:
                    _loadingProgressHandler?.SetProgress(.35F, "UPDATING PUZZLES...");
                    CacheConfigValues();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            _loadingProgressHandler?.SetProgress(1f, "READY!");
            _loadingProgressHandler?.Hide(1f)
                   .ContinueWith(() =>_configFetchCompleted = true);
        }

        private void CacheConfigValues()
        {
            var puzzleDailyJson = RemoteConfigService.Instance.appConfig.GetJson(ConfigKeys.PUZZLE_DAILY);
            var wordSetsJson = RemoteConfigService.Instance.appConfig.GetJson(ConfigKeys.PUZZLE_SETS);
            
            var puzzleManager = new PuzzleManager();
            try
            {
                var puzzleDaily = JsonUtility.FromJson<Puzzle>(puzzleDailyJson);
                if (puzzleDaily is { })
                {
                    puzzleManager.AddDailyPuzzle(puzzleDaily);
                    _loadingProgressHandler?.SetProgress(.5F, "UPDATING PUZZLES...");
                }

                var wordSets = JsonUtility.FromJson<PuzzlesList>(wordSetsJson);
                if (wordSets is {})
                    puzzleManager.AddWordsSet(wordSets);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            var container = ProjectContext.Instance.Container;
            container.Bind<IPuzzleManager>().To<PuzzleManager>().FromInstance(puzzleManager).AsSingle().NonLazy();
            Debug.Log("PuzzleManager registered");
        }
    }
}