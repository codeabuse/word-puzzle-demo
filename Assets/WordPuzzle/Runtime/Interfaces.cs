using System.Collections.Generic;
using System.Threading;
using Codeabuse;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace WordPuzzle
{
    public interface IWordCutter
    {
        string[] Cut(IEnumerable<string> words);
    }

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

    public interface IPopUpMenu
    {
        Transform SolvedPuzzleDisplayRoot { get; }
        void Show(GameState state);
        UniTask WaitContinueCommand(CancellationToken ct);
        UniTask WaitBackToMenuCommand(CancellationToken ct);
    }
}