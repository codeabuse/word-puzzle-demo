using Codeabuse;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace WordPuzzle
{
    public class StartupHelper : MonoBehaviour
    {
        [SerializeField]
        [WithInterface(typeof(IStartupProcedure))]
        private Component[] _startupProcedures;
        
        private async void Start()
        {
            foreach (var component in _startupProcedures)
            {
                var startupProcedure = (IStartupProcedure)component;
                if (startupProcedure is null)
                    continue;
                await startupProcedure.Load();
            }
        }
    }

    public interface IStartupProcedure
    {
        UniTask Load();
    }
}