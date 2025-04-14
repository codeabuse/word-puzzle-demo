using System.Collections.Generic;
using Codeabuse;
using UnityEngine;

namespace WordPuzzle
{
    [CreateAssetMenu(fileName = "New Puzzle Set", menuName = "WordPuzzle/Puzzle Set", order = 0)]
    public class PuzzleSet : ScriptableObject, IPuzzleCollection
    {
        [SerializeField]
        private PuzzlesList _puzzles;
        
        public void Add(Puzzle puzzle) => _puzzles.Add(puzzle);

        public void AddRange(IEnumerable<Puzzle> puzzles) => _puzzles.AddRange(puzzles);

        public Option<Puzzle> NextPuzzle() => _puzzles.NextPuzzle();
    }
}