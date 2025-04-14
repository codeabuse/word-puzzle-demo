using System.Collections.Generic;
using Codeabuse;

namespace WordPuzzle
{
    public interface IPuzzleManager
    {
        public IEnumerable<int> AvailablePuzzlesWordLength { get; }
        public bool TryGetPuzzles(int wordLength, out IPuzzleCollection puzzleCollection);

        public Puzzle GetDailyPuzzle();
    }

    public interface IPuzzleCollection
    {
        public void Add(Puzzle puzzle);
        void AddRange(IEnumerable<Puzzle> puzzles);
        Option<Puzzle> NextPuzzle();
    }
}