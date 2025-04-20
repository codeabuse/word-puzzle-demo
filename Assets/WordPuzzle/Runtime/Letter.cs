using Codeabuse.Pooling;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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
        
        private LettersCluster _cluster;
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

        public void AssignCluster(LettersCluster cluster)
        {
            _cluster = cluster;
        }

        public void OnGet()
        {
            
        }

        public void OnRelease()
        {
            _character = default;
            _cluster = null;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!_cluster)
                return;

            _cluster.OnBeginDrag(eventData);
        }
    }
}