using System.Collections.Generic;
using Codeabuse;
using Cysharp.Threading.Tasks;

namespace WordPuzzle
{
    public interface ILoadProgressHandler
    {
        void Show();
        void SetProgress(float progress, string message = default);
        void SetMessage(string message);
        UniTask Hide(float fadeOutSeconds = 0f);
    }
    
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