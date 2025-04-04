namespace WordPuzzle.Runtime
{
    // for DI
    
    public interface IGameSettings
    {
        public int WordLength { get; }
    }

    public interface IPuzzleProvider
    {
        // TODO: Get collection of puzzles from here
    }
}