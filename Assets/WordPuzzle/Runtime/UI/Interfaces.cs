using Codeabuse;
using UnityEngine;

namespace WordPuzzle.UI
{
    public interface IDockArea
    {
        Transform transform { get; }
        bool CanDock(IDockable dockable);
        void Dock(IDockable dockable);
        void Undock(IDockable dockable);
    }

    public interface IOrderedDockArea : IDockArea
    {
        new Option<int> CanDock(IDockable dockable);
        void DockAt(IDockable dockable, int position);
        void Borrow(IDockable dockable);
        void Return(IDockable dockable);
        Option<int> GetItemPosition(IDockable dockable);
    }

    public interface IDockable
    {
        string name { get; }
        Transform transform { get; }
        RectTransform RectTransform { get; }
        void SetHomeDock(IDockArea home);
        void OnDocked(IDockArea dockArea);
        void OnUndocked(IDockArea dockArea);
    }
}