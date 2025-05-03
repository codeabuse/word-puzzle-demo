using System.Collections.Generic;
using System.Text;
using System.Threading;
using Codeabuse.Pooling;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Zenject;
using Random = UnityEngine.Random;

namespace WordPuzzle
{
    public class GameBoard : MonoBehaviour
    {
        public const string WORDS_SCALE = "words_scale";
        
        [SerializeField]
        private ClustersDockArea _clustersDock;
        [SerializeField]
        private Transform _wordsRoot;
        
        [Inject]
        private PrefabPool<LettersCluster> _clustersPool;
        [Inject]
        private PrefabPool<WordCell> _wordCellsPool;

        private Transform _wordsRootParent;

        private readonly UnityEvent _onWordsMatch = new();

        private readonly List<WordCell> _wordCells = new();
        private readonly List<LettersCluster> _clusters = new();

        private readonly StringBuilder _stringBuilder = new();
        
        private readonly HashSet<string> _goalWords = new();

        [Inject]
        private IWordCutter _wordCutter;
        
        [Inject]
        private IPopUpMenu _popUpMenu;

        private void Awake()
        {
            _wordCells.AddRange(_wordsRoot.GetComponentsInChildren<WordCell>());
            _wordsRootParent = _wordsRoot.parent;
        }

        private void Start()
        {
            UniTask.DelayFrame(1).ContinueWith(RegisterWordsScale);
        }

        private void RegisterWordsScale()
        {
            var scale = _wordsRoot.transform.lossyScale[0];
            ProjectContext.Instance.Container.UnbindId<float>(WORDS_SCALE);
            ProjectContext.Instance.Container
                   .BindInstance(scale)
                   .WithId(WORDS_SCALE);
            Debug.Log($"Words Scale: {scale}");
        }

        public void Initialize(int wordLength, int wordsCount)
        {
            Cleanup();
            
            for (var i = 0; i < wordsCount; i++)
            {
                var wordCell = _wordCellsPool.Get();
                wordCell.SetLength(wordLength);
                _wordCells.Add(wordCell);
                wordCell.transform.SetParent(_wordsRoot);
            }
        }

        public async UniTask Play(Puzzle puzzle, bool showEndGameScreen, CancellationToken ct)
        {
            Initialize(puzzle.WordLength, puzzle.Words.Count);
            var cuts = _wordCutter.Cut(puzzle.Words);
            StartGame(puzzle.Words, cuts);
            
            await _onWordsMatch.OnInvokeAsync(ct);
            

            if (!showEndGameScreen)
                return;
            
            _wordsRoot.SetParent(_popUpMenu.SolvedPuzzleDisplayRoot);
            EnableClusters(false);
            await _popUpMenu.Show(GameState.PuzzleSolved, ct).ContinueWith(()=>
            {
                if (!this)
                    return;
                _wordsRoot.SetParent(_wordsRootParent);
                EnableClusters(true);
            });
        }

        private void EnableClusters(bool value)
        {
            foreach (var lettersCluster in _clusters)
            {
                lettersCluster.enabled = value;
            }
        }

        private void OnDestroy()
        {
            if (_wordsRoot)
                Destroy(_wordsRoot.gameObject);
        }

        private void StartGame(IReadOnlyList<string> words, string[] cuts)
        {
            _stringBuilder.AppendJoin(", ", words);
            Debug.Log($"Game started: {_stringBuilder.ToString()}");
            
            _stringBuilder.Clear();
            _goalWords.Clear();
            _goalWords.UnionWith(words);
            
            Shuffle(cuts, cuts.Length * 2);
            PopulateClusters(cuts);
            
            foreach (var wordCell in _wordCells)
            {
                wordCell.OnWordCellFilled.RemoveListener(OnWordFilled);
                wordCell.OnWordCellFilled.AddListener(OnWordFilled);
            }
        }

        private void OnWordFilled()
        {
            if (GoalWordsMatch())
                _onWordsMatch.Invoke();
        }

        private void Cleanup()
        {
            foreach (var wordCell in _wordCells)
            {
                wordCell.OnWordCellFilled.RemoveListener(OnWordFilled);
                _wordCellsPool.Release(wordCell);
            }

            foreach (var cluster in _clustersDock.GetComponentsInChildren<LettersCluster>())
            {
                if (cluster.IsDocked)
                {
                    _clustersDock.Undock(cluster);
                }
                _clustersPool.Release(cluster);
            }
        }

        private void Shuffle(string[] clusters, int shuffleSteps)
        {
            if (clusters.Length == 2 && Random.Range(0, 1f) > .5f)
            {
                (clusters[0], clusters[1]) = (clusters[1], clusters[0]);
                return;
            }
                
            var length = clusters.Length;
            int i1 = 0, i2 = 0;
            for (var i = 0; i < shuffleSteps; i++)
            {
                i1 = Random.Range(0, length);
                i2 = Random.Range(0, length);
                while (i2 == i1)
                {
                    i2 = Random.Range(0, length);
                }
                (clusters[i1], clusters[i2]) = (clusters[i2], clusters[i1]);
            }
        }

        private void PopulateClusters(string[] clusters)
        {
            ClearClusterViews();
            foreach (var clusterString in clusters)
            {
                var cluster = _clustersPool.Get();
                cluster.SetLetters(clusterString);
                _clusters.Add(cluster);
                _clustersDock.Dock(cluster);
            }
        }

        private void ClearClusterViews()
        {
            foreach (var cluster in _clusters)
            {
                _clustersDock.Undock(cluster);
                _clustersPool.Release(cluster);
            }
            _clusters.Clear();
        }

        private bool GoalWordsMatch()
        {
            var matchWords = UnityEngine.Pool.HashSetPool<string>.Get();
            
            foreach (var wordCell in _wordCells)
            {
                if (!wordCell.IsFilled)
                {
                    return false;
                }
                wordCell.Read(_stringBuilder);
                var word = _stringBuilder.ToString();
                
                if (!_goalWords.Contains(word))
                {
                    return false;
                }
                
                matchWords.Add(word);
            }

            var result = matchWords.Count == _goalWords.Count;
            UnityEngine.Pool.HashSetPool<string>.Release(matchWords);
            Debug.Log($"Words filled correctly: {result}");
            return result;
        }
    }
}