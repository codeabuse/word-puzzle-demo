using Codeabuse;
using UnityEngine;
using WordPuzzle.UI;

namespace WordPuzzle
{
    public class ClusterDockProxy : MonoBehaviour, IOrderedDockArea
    {
        [SerializeField]
        private ClustersDockArea _clustersDockArea;
        
        bool IDockArea.CanDock(IDockable dockable)
        {
            return ((IDockArea)_clustersDockArea).CanDock(dockable);
        }

        public void DockAt(IDockable dockable, int position)
        {
            _clustersDockArea.DockAt(dockable, position);
        }

        public void Borrow(IDockable dockable)
        {
            _clustersDockArea.Borrow(dockable);
        }

        public void ReturnBorrowed(IDockable dockable)
        {
            _clustersDockArea.ReturnBorrowed(dockable);
        }

        public Option<int> GetItemPosition(IDockable dockable)
        {
            return _clustersDockArea.GetItemPosition(dockable);
        }

        Option<int> IOrderedDockArea.CanDock(IDockable dockable)
        {
            return _clustersDockArea.CanDock(dockable);
        }

        public void Dock(IDockable dockable)
        {
            _clustersDockArea.Dock(dockable);
        }

        public void Undock(IDockable dockable)
        {
            _clustersDockArea.Undock(dockable);
        }
    }
}