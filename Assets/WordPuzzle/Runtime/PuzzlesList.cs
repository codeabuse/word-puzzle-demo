using System.Collections;
using System.Collections.Generic;
using Codeabuse;
using UnityEngine;

namespace WordPuzzle
{
    [System.Serializable]
    public class PuzzlesList : IPuzzleCollection, IEnumerable<Puzzle>
    {
        [SerializeField]
        private List<Puzzle> puzzles = new();

        private int _nextPuzzleIndex = 0;

        public int PuzzlesRemainig => puzzles.Count - _nextPuzzleIndex;
        public int Count => puzzles.Count;

        public void Add(Puzzle puzzle)
        {
            puzzles.Add(puzzle);
        }

        public void AddRange(IEnumerable<Puzzle> puzzles)
        {
            this.puzzles.AddRange(puzzles);
        }

        public void Remove(Puzzle puzzle)
        {
            this.puzzles.Remove(puzzle);
        }

        public Option<Puzzle> NextPuzzle()
        {
            if (puzzles.Count == 0)
                return null;
            if (_nextPuzzleIndex >= puzzles.Count)
            {
                return Option<Puzzle>.None;
            }
            
            return puzzles[_nextPuzzleIndex++];
        }

        public void ResetProgress()
        {
            _nextPuzzleIndex = 0;
        }

        public IEnumerator<Puzzle> GetEnumerator() => puzzles.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}