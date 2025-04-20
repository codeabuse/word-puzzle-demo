using System.Collections.Generic;
using Codeabuse;
using UnityEngine;

namespace WordPuzzle.UI
{
    public class OrderedDock : MonoBehaviour, IOrderedDockArea
    {
        [SerializeField]
        private RectTransform _dockAreaTransform;

        [SerializeField]
        private Axis _axis;

        private DockItemPlaceholder _placeholder;

        private readonly List<IDockable> _dockedItems = new();
        private IDockable _borrowedItem;

        protected List<IDockable> dockedItems => _dockedItems;

        public IReadOnlyList<IDockable> DockedItems => _dockedItems;

        protected virtual void Awake()
        {
            _dockedItems.AddRange(_dockAreaTransform.GetComponentsInChildren<IDockable>());
            _placeholder = GetComponentInChildren<DockItemPlaceholder>(true);
        }

        public Option<int> CanDock(IDockable dockable)
        {
            if (!enabled)
                return Option<int>.None;
            
            var dockablePositionOnAxis = dockable.transform.GetPositionOnAxis(_axis);
            for (var i = 0; i < _dockedItems.Count; i++)
            {
                var child = _dockedItems[i];
                if (dockablePositionOnAxis < child.transform.GetPositionOnAxis(_axis))
                {
                    continue;
                }

                return i;
            }

            return _dockedItems.Count;
        }

        bool IDockArea.CanDock(IDockable dockable)
        {
            return CanDock(dockable).HasValue;
        }

        public void Dock(IDockable dockable)
        {
            DockAt(dockable, _dockedItems.Count);
        }

        public void DockAt(IDockable dockable, int position)
        {
            dockable.transform.SetParent(_dockAreaTransform);
            dockable.transform.SetSiblingIndex(position);
            _dockedItems.Insert(position, dockable);
            dockable.OnDocked(this);
            Debug.Log($"{dockable.name} was docked at position {position}");
            this.OnDocked(dockable);
        }

        public void Borrow(IDockable dockable)
        {
            if (!_dockedItems.Contains(dockable))
            {
                Debug.LogError($"Trying to borrow {dockable.name} from {name}, but it is not docked.");
                return;
            }
            Debug.Log($"Borrowed {dockable.name}");
            _borrowedItem = dockable;
            var index = dockable.transform.GetSiblingIndex();
            var size = dockable.RectTransform.rect.size[(int)_axis];
            
            dockable.transform.SetParent(dockable.transform.root);
            
            _placeholder.transform.SetSiblingIndex(index);
            _placeholder.SetSize(size);
            _placeholder.gameObject.SetActive(true);
            this.OnBorrowed(dockable);
        }

        public void Return(IDockable dockable)
        {
            if (dockable is { } && dockable.Equals(_borrowedItem))
            {
                _borrowedItem = null;
            }

            var position = _placeholder.transform.GetSiblingIndex();
            _placeholder.gameObject.SetActive(false);
            dockable.transform.SetParent(_dockAreaTransform);
            _placeholder.transform.SetSiblingIndex(_dockAreaTransform.childCount);
            dockable.transform.SetSiblingIndex(position);
            Debug.Log($"Returning borrowed item at position {position}");
            this.OnReturned(dockable);
        }

        public void Undock(IDockable dockable)
        {
            if (!_dockedItems.Contains(dockable))
            {
                Debug.LogError($"Trying to undock {dockable.name} from {name}, but it is not docked.");
                return;
            }

            if (_borrowedItem is { } && _borrowedItem.Equals(dockable))
            {
                _placeholder.gameObject.SetActive(false);
                _borrowedItem = null;
            }
            
            _dockedItems.Remove(dockable);
            dockable.OnUndocked(this);
            this.OnUndocked(dockable);
        }

        public Option<int> GetItemPosition(IDockable dockable)
        {
            return _dockedItems.Contains(dockable)? dockable.transform.GetSiblingIndex() : Option<int>.None;
        }


        protected virtual void OnDocked(IDockable dockable) { }
        protected virtual void OnUndocked(IDockable dockable) { }
        protected virtual void OnBorrowed(IDockable dockable) { }
        protected virtual void OnReturned(IDockable dockable) { }
    }
}