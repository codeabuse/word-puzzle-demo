using Codeabuse;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace WordPuzzle
{
    public class StartupHelper : MonoBehaviour
    {
        [SerializeField]
        [WithInterface(typeof(IStartupProcedure))]
        private Component[] _startupProcedures;
        
        private async void Start()
        {
            foreach (IStartupProcedure startupProcedure in _startupProcedures)
            {
                if (startupProcedure is null)
                    continue;
                await startupProcedure.Load();
            }
        }
    }

    public interface IStartupProcedure
    {
        [Inject]
        void SetLoadingProgressHandler(ILoadProgressHandler loadProgressHandler);
        UniTask Load();
    }
}