using Codeabuse.Pooling;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WordPuzzle
{
    [DisallowMultipleComponent]
    public class Letter : MonoBehaviour, IPooledBehavior
    {
        [SerializeField]
        private TMP_Text _text;
        private bool _isSelectable;

        private char _character;
        
        [SerializeField]
        private Image _image;
        public char Character => _character;

        public void Set(char c)
        {
            _character = c;
            _text.text = new string( new[]{ _character });
        }

        private void Awake()
        {
            if (_text.text.Length > 0)
                _character = _text.text[0];
        }
        public void OnGet()
        {
            
        }

        public void OnRelease()
        {
            _character = default;
        }
    }
}