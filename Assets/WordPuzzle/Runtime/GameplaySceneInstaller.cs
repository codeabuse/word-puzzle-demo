using Codeabuse.Pooling;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace WordPuzzle
{
    public class GameplaySceneInstaller : MonoInstaller
    {
        [SerializeField]
        private GraphicRaycaster _graphicRaycaster;
        [SerializeField]
        private Letter _letterPrefab;
        [SerializeField]
        private LetterCell _letterCellPrefab;
        [FormerlySerializedAs("_lettersPoolRoot")]
        [SerializeField]
        private Transform _commonPoolRoot;

        public override void InstallBindings()
        {
            Container.Bind<GraphicRaycaster>()
                   .FromInstance(_graphicRaycaster)
                   .AsSingle();
            
            Container.Bind<PrefabPool<Letter>>()
                   .FromInstance(new PrefabPool<Letter>(_letterPrefab, _commonPoolRoot))
                   .AsSingle()
                   .NonLazy();
            Container.Bind<PrefabPool<LetterCell>>()
                   .FromInstance(new PrefabPool<LetterCell>(_letterCellPrefab, _commonPoolRoot))
                   .AsSingle()
                   .NonLazy();
        }
    }
}