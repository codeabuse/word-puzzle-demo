using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace WordPuzzle
{
    public class ProjectInstaller : MonoInstaller
    {
        [SerializeField]
        private EventSystem _eventSystemPrefab;
        
        public override void InstallBindings()
        {
            Container.Bind<ILoadProgressHandler>().To<LoadingScreenController>().FromComponentInHierarchy().AsSingle();
            Container.Bind<EventSystem>().FromComponentInNewPrefab(_eventSystemPrefab).AsSingle().NonLazy();
        }
    }
}