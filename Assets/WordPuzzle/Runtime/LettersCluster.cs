using System;
using System.Collections.Generic;
using Codeabuse.Pooling;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WordPuzzle.UI;
using Zenject;

namespace WordPuzzle
{
    public class LettersCluster : MonoBehaviour, 
            IPooledBehavior, 
            IBeginDragHandler, 
            IDragHandler, 
            IEndDragHandler,
            IDockable
    {
        [SerializeField]
        private List<Letter> _letters = new();
        [SerializeField]
        private Graphic _targetGraphic;
        
        private Vector2 _dragOffset;
        private PrefabPool<Letter> _lettersPool;
        private GraphicRaycaster _graphicRaycaster;

        private readonly List<RaycastResult> _raycastResults = new();
        private readonly Vector3[] _worldCorners = new Vector3[4];

        private IDockArea _dockedTo;
        private IDockArea _dockTarget;
        private IDockArea _homeDock;
        private bool _draggingDockContainer;

        public IReadOnlyList<Letter> Letters => _letters;
        public int Size => _letters.Count;


        public RectTransform RectTransform { get; private set; }
        public bool IsDocked => _dockedTo is { };

        private void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
            if (GetComponentInParent<IDockArea>() is { } dockArea)
                _dockedTo = dockArea;
        }

        [Inject]
        private void Construct(GraphicRaycaster graphicRaycaster, PrefabPool<Letter> lettersPool)
        {
            _graphicRaycaster = graphicRaycaster;
            _lettersPool = lettersPool;
        }

        public void SetLetters(string letters)
        {
            if (letters is null)
            {
                throw new ArgumentException("Letters must not be null", nameof(letters));
            }

            var count = letters.Length - _letters.Count;
            if (count > 0)
            {
                for (var i = 0; i < count; i++)
                {
                    var letter = _lettersPool.Get();
                    letter.transform.SetParent(transform);
                    letter.transform.localScale = Vector3.one;
                    _letters.Add(letter);
                }
            }
            else if (count < 0)
            {
                for (var (i, j) = (0, _letters.Count - 1); i > count; i--, j--)
                {
                    var letter = _letters[j];
                    _lettersPool.Release(letter);
                    _letters.RemoveAt(j);
                }
            }
            

            for (var i = 0; i < _letters.Count; i++)
            {
                var letter = _letters[i];
                letter.Set(letters[i]);
            }
        }

        void IPooledBehavior.OnGet()
        {
            
        }

        void IPooledBehavior.OnRelease()
        {
            foreach (var letter in _letters)
            {
                _lettersPool.Release(letter);
                letter.transform.localScale = Vector3.one;
            }
            _letters.Clear();
            transform.localScale = Vector3.one;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            var delta = eventData.delta;
            if (_dockedTo == _homeDock &&
                _homeDock is Component dockComponent && 
                Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                _targetGraphic.raycastTarget = false;
                _draggingDockContainer = true;
                // pass-through the drag event
                // SendMessageUpwards would lead to infinite cycle and hangs the game
                dockComponent.SendMessage("OnBeginDrag", eventData);
                return;
            }
            switch (_dockedTo)
            {
                case IOrderedDockArea orderedDockArea:
                    orderedDockArea.Borrow(this);
                    break;
                case {}:
                    _dockedTo.Undock(this);
                    break;
            }
            
            _dragOffset = eventData.position - (Vector2)transform.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_draggingDockContainer && _homeDock is Component dockComponent)
            {
                dockComponent.SendMessage("OnDrag", eventData);
                return;
            }
            transform.position = eventData.position - _dragOffset;
            
            if (OverlapsDockArea(out var dockArea) && dockArea.CanDock(this))
            {
                _dockTarget = dockArea;
            }
            else
            {
                _dockTarget = null;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_draggingDockContainer && _homeDock is Component dockComponent)
            {
                dockComponent.SendMessage("OnEndDrag", eventData);
                _draggingDockContainer = false;
                _targetGraphic.raycastTarget = true;
                return;
            }
            // find dock target
            if (_dockTarget is {} && _dockTarget.CanDock(this))
            {
                _dockedTo?.Undock(this);
                _dockTarget.Dock(this);
            }
            // if none, return to home dock
            else
            {
                _homeDock?.Dock(this);
            }
        }

        private bool OverlapsDockArea(out IDockArea dockArea)
        {
            dockArea = default;
            RectTransform.GetWorldCorners(_worldCorners);
            var overlappedWordCells = UnityEngine.Pool.HashSetPool<IDockArea>.Get();
            foreach (var worldCorner in _worldCorners)
            {
                if (RaycastCanvas<IDockArea>(worldCorner, out var overlappwdWordCell))
                {
                    overlappedWordCells.Add(overlappwdWordCell);
                }
            }

            var smallestDistance = float.MaxValue;
            var position = transform.position;
            foreach (var overlappedDockArea in overlappedWordCells)
            {
                var distance = (overlappedDockArea.transform.position - position).magnitude;
                if (distance < smallestDistance)
                {
                    smallestDistance = distance;
                    dockArea = overlappedDockArea;
                }
            }

            UnityEngine.Pool.HashSetPool<IDockArea>.Release(overlappedWordCells);
            return dockArea is {};
        }

        private bool RaycastCanvas<T>(Vector2 point, out T result)
        {
            result = default;
            _graphicRaycaster.Raycast(new PointerEventData(EventSystem.current)
            {
                    position = point
            }, _raycastResults);

            foreach (var raycastResult in _raycastResults)
            {
                if ((result = raycastResult.gameObject.GetComponent<T>()) is {})
                {
                    _raycastResults.Clear();
                    return true;
                }
            }
            _raycastResults.Clear();
            return false;
        }

        public void SetHomeDock(IDockArea home)
        {
            _homeDock = home;
        }

        public void OnDocked(IDockArea dockArea)
        {
            _dockedTo = dockArea;
        }

        public void OnUndocked(IDockArea dockArea)
        {
            _dockedTo = null;
        }
    }
}