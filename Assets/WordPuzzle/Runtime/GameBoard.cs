using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Codeabuse.Pooling;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace WordPuzzle
{
    public class GameBoard : MonoBehaviour
    {
        private const int max_failed_cuts = 10;
        
        //[SerializeField]
        private WordCutParameters _clusterCutParams;
        
        [SerializeField]
        private WordCell _wordCellPrefab;
        [SerializeField]
        private Transform _wordsRoot;
        
        [SerializeField]
        private LetterCell _letterCellPrefab;


        [SerializeField]
        private LettersCluster _lettersClusterPrefab;

        [SerializeField]
        private Letter _letterPrefab;
        
        [SerializeField]
        private Transform _objectsPoolRoot;
        

        private PrefabPool<LetterCell> _cellsPool;
        private PrefabPool<Letter> _lettersPool;
        private PrefabPool<LettersCluster> _clustersPool;

        private readonly List<WordCell> _words = new();
        private readonly List<LetterCell> _cells = new();
        private readonly List<LettersCluster> _clusters = new();
        private string[] _goalWords;

        private void Awake()
        {
            _cellsPool = new(_letterCellPrefab, _objectsPoolRoot);
            _lettersPool = new(_letterPrefab, _objectsPoolRoot);
            _clustersPool = new(_lettersClusterPrefab, _objectsPoolRoot)
            {
                    OnCreate = x => x.Initialize(_lettersPool)
            };
        }

        public void Initialize(int wordLength, int wordsCount)
        {
            Cleanup();
            
            for (var i = 0; i < wordsCount; i++)
            {
                var word = Instantiate(_wordCellPrefab, _wordsRoot);
                //word.Initialize(_cellsPool);
                word.SetLength(wordLength);
                _words.Add(word);
            }
        }

        private void Cleanup()
        {
            foreach (var cell in _cells)
            {
                if (cell.IsFilled)
                {
                    _lettersPool.Release(cell.Letter);
                }
                _cellsPool.Release(cell);
            }

            foreach (var wordCell in _words)
            {
                Destroy(wordCell.gameObject);
            }
        }

        public async UniTask StartGame(string[] words, CancellationToken cancellationToken)
        {
            _goalWords = words;
            var clusters = Cut(words, _clusterCutParams);
            Shuffle(clusters);
            PopulateClusterViews(clusters);
            while (!GoalWordsMatch())
            {
                 //await UniTask.WhenAll()
            }
        }

        private void Shuffle(string[] clusters)
        {
            if (clusters.Length == 2 && Random.Range(0, 1f) > .5f)
            {
                (clusters[0], clusters[1]) = (clusters[1], clusters[0]);
                return;
            }
                
            var length = clusters.Length;
            var shuffleSteps = length * 2;
            int i1 = 0, i2 = 0;
            for (var i = 0; i < shuffleSteps; i++)
            {
                i1 = Random.Range(0, length);
                while (i2 == i1)
                {
                    i2 = Random.Range(0, length);
                }
                (clusters[i1], clusters[i2]) = (clusters[i2], clusters[i1]);
            }
        }

        private void PopulateClusterViews(string[] clusters)
        {
            ClearClusterViews();
            foreach (var cluster in clusters)
            {
                var clusterView = _clustersPool.Get();
                clusterView.Initialize(_lettersPool);
                clusterView.SetLetters(cluster);
            }
        }

        private void ClearClusterViews()
        {
            foreach (var cluster in _clusters)
            {
                _clustersPool.Release(cluster);
            }
            _clusters.Clear();
        }

        private string[] Cut(string[] words, WordCutParameters clusterCutParams)
        {
            var cuts = ListPool<string>.Get();
            var cutSize = clusterCutParams.RandomCut();
            List<char> currentlyCut = new();
            int failedCutAttempts = 0;
            foreach (var word in words)
            {
                currentlyCut.AddRange(word);
                var remainingLength = currentlyCut.Count - cutSize;
                while (remainingLength < 0 || remainingLength < clusterCutParams.MinCut && remainingLength > 0)
                {
                    if (failedCutAttempts >= max_failed_cuts)
                    {
                        throw new ImpossibleWordCutException(word, new string(currentlyCut.ToArray()),
                                clusterCutParams);
                    }
                    failedCutAttempts++;
                    cutSize = clusterCutParams.RandomCut();
                    cuts.Add(new string(currentlyCut.GetRange(0, cutSize).ToArray()));
                    currentlyCut.RemoveRange(0, cutSize);
                    remainingLength = currentlyCut.Count - cutSize;
                }
            }

            return default;
        }

        private bool GoalWordsMatch()
        {
            return default;
        }

        public async UniTask Play(IPuzzleCollection puzzles)
        {
            while (puzzles.NextPuzzle() is { HasValue: true } puzzleOption)
            {
                puzzleOption.Match(puzzle =>
                {
                    
                } );
            }
        }

        private UniTask PlayPuzzle(Puzzle puzzle)
        {
            throw new NotImplementedException();
        }
    }

    internal class ImpossibleWordCutException : Exception
    {
        public override string Message { get; }

        public ImpossibleWordCutException(string word, string partToCut, WordCutParameters clusterCutParams)
        {
            this.Message = $"Impossible to cut the word's '{word}' remaining part: '{partToCut}' with settings: " +
                           $"MinCut {clusterCutParams.MinCut}, MaxCut: {clusterCutParams.MaxCut}";
        }
    }

    [Serializable]
    public struct WordCutParameters
    {
        public int MinCut;
        public int MaxCut;

        public int RandomCut()
        {
            return Random.Range(MinCut, MaxCut);
        }
    }
}