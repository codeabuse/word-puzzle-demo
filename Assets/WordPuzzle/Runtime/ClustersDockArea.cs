using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using WordPuzzle.UI;

namespace WordPuzzle
{
    public class ClustersDockArea : OrderedDock
    {
        [SerializeField]
        private float _scaleDuration = .25f;

        [SerializeField]
        private float _dockedClustersScale = .77f;

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
            dockable.transform.DOScale(Vector3.one, _scaleDuration)
                   .ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());
        }

        protected override void OnReturned(IDockable dockable)
        {
            dockable.transform.DOScale(Vector3.one * _dockedClustersScale, _scaleDuration)
                   .ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());
        }

        protected override void OnUndocked(IDockable dockable)
        {
            
        }
    }
}