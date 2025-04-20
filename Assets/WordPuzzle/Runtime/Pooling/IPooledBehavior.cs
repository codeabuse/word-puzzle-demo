namespace Codeabuse.Pooling
{
    public interface IPooledBehavior
    {
        void OnGet();
        void OnRelease();
    }
}