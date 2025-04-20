using Codeabuse.Pooling;
using UnityEngine;
using UnityEngine.UI;

namespace WordPuzzle
{
    [RequireComponent(typeof(RectTransform))]
    public class LetterCell : MonoBehaviour, IPooledBehavior
    {
        [SerializeField]
        private Image _image;
        
        [SerializeField]
        private Color _highlightColor;
        private Color _normalColor;
        
        private RectTransform _rectTransform;
        private Letter _letter;

        public bool IsFilled => (bool)_letter;
        public char Character => IsFilled ? _letter.Character : '_';
        public Letter Letter => _letter;

        private void Awake()
        {
            _rectTransform = transform as RectTransform;
            _normalColor = _image.color;
        }

        public void Put(Letter letter)
        {
            _letter = letter;
        }

        public Letter Remove()
        {
            if (!_letter)
                return null;
            var letter = _letter;
            _letter = null;
            return letter;
        }


        public void OnGet()
        {
            
        }

        public void OnRelease()
        {
            
        }

        public void SetHighlight(bool value)
        {
            _image.color = value? _highlightColor : _normalColor;
        }
    }
}