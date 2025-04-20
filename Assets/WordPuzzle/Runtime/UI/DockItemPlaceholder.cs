using UnityEngine;

namespace WordPuzzle.UI
{
    public class DockItemPlaceholder : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _dummy;

        [SerializeField]
        private RectTransform.Axis _axis;
        public RectTransform Dummy => _dummy;

        public void SetSize(float width)
        {
            _dummy.SetSizeWithCurrentAnchors(_axis, width);
        }
    }
}