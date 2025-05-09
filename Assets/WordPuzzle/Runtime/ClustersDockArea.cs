using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using WordPuzzle.Signals;
using WordPuzzle.UI;
using Zenject;

namespace WordPuzzle
{
    public class ClustersDockArea : OrderedDock, IInitializable
    {
        [SerializeField]
        private float _scaleDuration = .25f;

        [SerializeField]
        private float _dockedClustersScale = .77f;

        private float _undockedClusterScale = 1f;

        [Inject]
        private SignalBus _signalBus;

        public void Initialize()
        {
            _signalBus.Subscribe<WordsRootScaleChangedSignal>(OnWordsRootScaleChanged);
        }

        private void Start()
        {
            foreach (var dockable in GetComponentsInChildren<IDockable>())
            {
                dockable.SetHomeDock(this);
            }
        }

        private void OnWordsRootScaleChanged(WordsRootScaleChangedSignal signal)
        {
            _undockedClusterScale = signal.Scale;
        }

        protected override void OnDocked(IDockable dockable)
        {
            dockable.transform.DOScale(Vector3.one * _dockedClustersScale, _scaleDuration)
                   .ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());
            dockable.SetHomeDock(this);
        }

        protected override void OnBorrowed(IDockable dockable)
        {
            dockable.transform.DOScale(Vector3.one * _undockedClusterScale, _scaleDuration)
                   .ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());
        }

        protected override void OnReturned(IDockable dockable)
        {
            dockable.transform.DOScale(Vector3.one * _dockedClustersScale, _scaleDuration)
                   .ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());
        }
    }
}