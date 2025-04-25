using UnityEngine;
using Zenject;

namespace WordPuzzle
{
    public class MenuButtonHandler : MonoBehaviour
    {
        [Inject]
        private IPopUpMenu _popUpMenu;
        
        public void OpenMenu()
        {
            _popUpMenu.Show(GameState.Playing);
        }
    }
}