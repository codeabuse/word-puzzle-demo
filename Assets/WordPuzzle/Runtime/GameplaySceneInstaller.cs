using Codeabuse.Pooling;
using UnityEngine;
using UnityEngine.UI;
using WordPuzzle.Signals;
using Zenject;

namespace WordPuzzle
{
    public class GameplaySceneInstaller : MonoInstaller
    {
        [SerializeField]
        private GraphicRaycaster _graphicRaycaster;
        [SerializeField]
        private ClustersDockArea _clustersDockArea;
        [SerializeField]
        private Letter _letterPrefab;
        [SerializeField]
        private LetterCell _letterCellPrefab;
        [SerializeField]
        private WordCell _wordCellPrefab;
        
        [SerializeField]
        private LettersCluster _lettersClusterPrefab;
        
        [SerializeField]
        private Transform _commonPoolRoot;

        public override void InstallBindings()
        {
            Container.Bind<IWordCutter>().To<RandomWordCutter>().AsSingle();
            
            Container.Bind<GraphicRaycaster>()
                   .FromInstance(_graphicRaycaster)
                   .AsSingle();
            
            Container.Bind<PrefabPool<Letter>>()
                   .FromInstance(new PrefabPool<Letter>(_letterPrefab, _commonPoolRoot))
                   .AsSingle()
                   .NonLazy();
            
            Container.Bind<PrefabPool<LetterCell>>()
                   .FromInstance(new PrefabPool<LetterCell>(
                                        _letterCellPrefab, 
                                        _commonPoolRoot))
                   .AsSingle()
                   .NonLazy();
            
            Container.Bind<PrefabPool<LettersCluster>>()
                   .FromInstance(new PrefabPool<LettersCluster>(
                                        _lettersClusterPrefab, 
                                        _commonPoolRoot,
                                        (p, t) => 
                                                      Container.InstantiatePrefab(p, t)
                                                                   .GetComponent<LettersCluster>()))
                   .AsSingle()
                   .NonLazy();
            
            Container.Bind<PrefabPool<WordCell>>()
                   .FromInstance(new PrefabPool<WordCell>(
                                        _wordCellPrefab, 
                                        _commonPoolRoot,
                                        (p, t) => 
                                                      Container.InstantiatePrefab(p, t)
                                                                   .GetComponent<WordCell>()))
                   .AsSingle()
                   .NonLazy();

            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<WordsRootScaleChangedSignal>();
            Container.BindInterfacesTo<ClustersDockArea>().FromInstance(_clustersDockArea).AsSingle();
        }
    }
}