using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using WordPuzzle.UI;
using Zenject;

namespace WordPuzzle
{
    public class ClustersDockArea : OrderedDock
    {
        [SerializeField]
        private float _scaleDuration = .25f;

        [SerializeField]
        private float _dockedClustersScale = .77f;

        private float _boardClustersScale;

        private void Start()
        {
            foreach (var dockable in GetComponentsInChildren<IDockable>())
            {
                dockable.SetHomeDock(this);
            }

        }

        protected override void OnDocked(IDockable dockable)
        {
            dockable.transform.DOScale(Vector3.one * _dockedClustersScale, _scaleDuration)
                   .ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());
            dockable.SetHomeDock(this);
        }

        protected override void OnBorrowed(IDockable dockable)
        {
            
            var scale = (float)ProjectContext.Instance.Container
                   .Resolve(new BindingId(){Identifier = GameBoard.WORDS_SCALE, Type = typeof(float)});

            dockable.transform.DOScale(Vector3.one * scale, _scaleDuration)
                   .ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());
        }

        protected override void OnReturned(IDockable dockable)
        {
            dockable.transform.DOScale(Vector3.one * _dockedClustersScale, _scaleDuration)
                   .ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());
        }

        protected override void OnUndocked(IDockable dockable)
        {
            dockable.transform.DOScale(Vector3.one, _scaleDuration)
                   .ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());
        }
    }
}