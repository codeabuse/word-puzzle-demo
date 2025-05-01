using System.Collections.Generic;

namespace WordPuzzle.Configuration
{
    public class PuzzleManager : IPuzzleManager
    {
        private readonly Dictionary<int, PuzzlesList> _puzzles = new();
        private Puzzle _dailyPuzzle;

        public void AddPuzzles(PuzzlesList puzzles)
        {
            foreach (var puzzle in puzzles)
            {

                if (!_puzzles.TryGetValue(puzzle.WordLength, out var puzzlesList))
                {
                    puzzlesList = new();
                    _puzzles.Add(puzzle.WordLength, puzzlesList);
                }
                puzzlesList.Add(puzzle);
            }
        }

        public void AddDailyPuzzle(Puzzle puzzle)
        {
            _dailyPuzzle = puzzle;
        }

        public IEnumerable<int> AvailablePuzzlesWordLength => _puzzles.Keys;
        
        public bool TryGetPuzzles(int wordLength, out IPuzzleCollection puzzleCollection)
        {
            var result = _puzzles.TryGetValue(wordLength, out var collection);
            puzzleCollection = collection;
            return result;
        }

        public Puzzle GetTutorial()
        {
            return _dailyPuzzle;
        }
    }
}