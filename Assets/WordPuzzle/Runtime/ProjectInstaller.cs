using Zenject;

namespace WordPuzzle
{
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ILoadProgressHandler>().To<LoadingScreenController>().FromComponentInHierarchy().AsSingle();
            Container.Bind<IPopUpMenu>().To<PopupMenuController>().FromComponentInHierarchy().AsSingle();
        }
    }
}