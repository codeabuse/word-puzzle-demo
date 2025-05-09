namespace WordPuzzle.Signals
{
    public class WordsRootScaleChangedSignal
    {
        public float Scale { get; }

        public WordsRootScaleChangedSignal(float scale)
        {
            Scale = scale;
        }
    }
}