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

    public class RemoteConfigManager : MonoBehaviour
    {
        private const int retry_time_seconds = 2;

        private async void Start()
        {
            
            
            int retryConnect = 5;
            var retryDelay = TimeSpan.FromSeconds(retry_time_seconds);
            while (retryConnect > 0 && !Utilities.CheckForInternetConnection())
            {
                Debug.Log(
                        $"No internet connection available, retrying in {retry_time_seconds}s ({retryConnect} attempts remaining)");
                await UniTask.Delay(retryDelay);
                --retryConnect;
            }

            if (!Utilities.CheckForInternetConnection())
            {
                Debug.LogError("No internet connection available");
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
        }

        private async UniTask InitServices()
        {
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
                    CacheConfigValues();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void CacheConfigValues()
        {
            var wordLength = RemoteConfigService.Instance.appConfig.GetInt(ConfigKeys.WORD_LENGTH);
            var puzzleDailyJson = RemoteConfigService.Instance.appConfig.GetJson(ConfigKeys.PUZZLE_DAILY);
            var wordSetsJson = RemoteConfigService.Instance.appConfig.GetJson(ConfigKeys.PUZZLE_SETS);
            
            var puzzleManager = new PuzzleManager();
            try
            {
                var puzzleDaily = JsonUtility.FromJson<Puzzle>(puzzleDailyJson);
                if (puzzleDaily is {})
                    puzzleManager.AddDailyPuzzle(puzzleDaily);

                var wordSets = JsonUtility.FromJson<PuzzlesList>(wordSetsJson);
                if (wordSets is {})
                    puzzleManager.AddWordsSet(wordSets);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            var container = new DiContainer();
            
            container.Bind<IPuzzleManager>().FromInstance(puzzleManager).AsSingle();

        }
    }
}