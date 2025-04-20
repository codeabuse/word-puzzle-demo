using System.Collections.Generic;
using System.Text;
using Codeabuse;
using Codeabuse.Pooling;
using Cysharp.Threading.Tasks;
using UnityEngine;
using WordPuzzle.UI;
using Zenject;

namespace WordPuzzle
{
    public class WordCell : MonoBehaviour, IDockArea
    {
        private readonly List<LetterCell> _cells = new();
        private readonly Dictionary<LettersCluster, int> _dockedClusters = new();
        private readonly StringBuilder _stringBuilder = new();
        
        private PrefabPool<LetterCell> _letterCellsPool;
        
        private int _lastFrameHighlight;
        private bool _highlightEnabled;

        private RectTransform _rectTransform;
        
        private float _cellWidth;
        private readonly Vector3[] _worldCorners = new Vector3[4];

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            var attachedCells = GetComponentsInChildren<LetterCell>();
            var cellsCount = attachedCells.Length;
            foreach (var cell in attachedCells)
            {
                _letterCellsPool.Release(cell);
            }
            SetLength(cellsCount);
        }

        private void Start()
        {
            UniTask.DelayFrame(1)
                   .ContinueWith(CalculateCellWidth)
                   .ContinueWith(()=> 
                            _rectTransform.GetWorldCorners(_worldCorners));
        }

        private void CalculateCellWidth()
        {
            _cellWidth = _rectTransform.rect.size[0] / _cells.Count;
        }

        public Option<string> Read(StringBuilder stringBuilder)
        {
            stringBuilder.Clear();
            foreach (var cell in _cells)
            {
                stringBuilder.Append(cell.Character);
            }

            return stringBuilder.ToString();
        }

        [Inject]
        public void Initialize(PrefabPool<LetterCell> letterCellsPool)
        {
            _letterCellsPool = letterCellsPool;
        }

        public void SetLength(int length)
        {
            ClearCells();
            for (var i = 0; i < length; i++)
            {
                var cell = _letterCellsPool.Get();
                cell.transform.SetParent(transform);
                
                // WordCell uses reversed order in layout group to avoid hiding attached clusters
                _cells.Insert(0, cell);
            }
            UniTask.DelayFrame(1).ContinueWith(CalculateCellWidth);
        }

        public void ClearCells()
        {
            foreach (var letterCell in _cells)
            {
                _letterCellsPool.Release(letterCell);
            }
            _cells.Clear();
        }

        public Option<int> CanDock(IDockable dockable)
        {
            if (dockable is not LettersCluster cluster)
                return Option<int>.None;
            
            var relativeLetterPosition = cluster.Letters[0].transform.position.x - _worldCorners[0].x;
            
            var start = (int)(relativeLetterPosition / _cellWidth);
            if (start < 0 || start >= _cells.Count)
            {
                return Option<int>.None;
            }
            
            var end = start + cluster.Size;
            if (end > _cells.Count)
            {
                return Option<int>.None;
            }

            for (var i = start; i < end; i++)
            {
                
                if (_cells[i].IsFilled)
                    return Option<int>.None;
            }

            return start;
        }

        bool IDockArea.CanDock(IDockable dockable)
        {
            return CanDock(dockable).HasValue;
        }

        public void Dock(IDockable dockable)
        {
            if (dockable is not LettersCluster cluster)
                return;

            if (CanDock(dockable) is not { HasValue: true } option)
                return;
            var start = option.Value;
            var end = start + cluster.Size;

            for (int i = start, j = 0; i < end; i++, j++)
            {
                _cells[i].Put(cluster.Letters[j]);
            }

            _dockedClusters[cluster] = start;
            cluster.transform.SetParent(_cells[start].transform);
            cluster.OnDocked(this);
            Debug.Log(Read(_stringBuilder).Value);
        }

        public void Undock(IDockable dockable)
        {
            if (dockable is not LettersCluster cluster)
                return;
            
            if (!_dockedClusters.TryGetValue(cluster, out var position))
                return;

            _dockedClusters.Remove(cluster);
            for (var i = position; i < position + cluster.Size; i++)
            {
                _cells[i].Remove();
            }
            dockable.transform.SetParent(transform.root);
            dockable.OnUndocked(this);
            Debug.Log(Read(_stringBuilder).Value);
        }
    }
}