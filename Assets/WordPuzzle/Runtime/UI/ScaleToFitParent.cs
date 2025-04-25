using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WordPuzzle.UI
{
    [RequireComponent(typeof(RectTransform))]
    [ExecuteAlways]
    public class ScaleToFitParent : UIBehaviour, ILayoutSelfController
    {
        private const float presicision = 0.001f;
        
        private RectTransform _self;

        private RectTransform _parent;

        [SerializeField]
        private RectTransform.Axis _referenceAxis;

        protected override void Awake()
        {
            base.Awake();
            
        }

        protected override void Start()
        {
            base.Start();
            _self = GetComponent<RectTransform>();
            _parent = _self.parent as RectTransform;
        }

        protected override void OnTransformParentChanged()
        {
            _parent = _self.parent as RectTransform;
            AdjustScale();
        }

        public void SetLayoutHorizontal()
        {
            if (!_parent)
                return;
            if (_referenceAxis is not RectTransform.Axis.Horizontal)
                return;
            
            AdjustScale();
        }

        public void SetLayoutVertical()
        {
            if (!_parent)
                return;
            if (_referenceAxis is not RectTransform.Axis.Vertical)
                return;

            AdjustScale();
        }

        [ContextMenu(nameof(AdjustScale))]
        private void AdjustScale()
        {
            
            if (!_parent)
                return;
            
            var axisIndex = (int)_referenceAxis;
            var currentScale = transform.localScale[axisIndex];

            var scale = GetTargetScaleToFit(_self, _parent, _referenceAxis);
            Debug.Log($"Parent size: {_parent.rect.size[axisIndex]}, self size: {_self.rect.size[axisIndex]}");
            Debug.Log($"{_referenceAxis} target scale: {scale}");
                    
            if (Mathf.Abs(currentScale - scale) < presicision)
                return;
            
            var otherAxisIndex = (axisIndex + 1) % 2;
            var orthogonalSide = _self.rect.size[otherAxisIndex];
            var orthogonalSideScaled = orthogonalSide * scale;
            var fitOrthogonalSide = _parent.rect.size[otherAxisIndex];
            if (orthogonalSideScaled > fitOrthogonalSide)
            {
                Debug.Log($"Other axis scale: {scale}");
                scale = fitOrthogonalSide / orthogonalSide;
            }

            _self.localScale = Vector3.one * scale;
        }

        private static float GetTargetScaleToFit(RectTransform target, RectTransform fitInto, RectTransform.Axis axis)
        {
            var axisIndex = (int)axis;
            var fitSize = fitInto.rect.size[axisIndex];
            var currentScale = target.localScale[axisIndex];
            var unscaledSize = target.rect.size[axisIndex];
            var selfSize = unscaledSize * currentScale;
            
            if (Mathf.Abs(fitSize - selfSize) < presicision)
                return currentScale;
            return fitSize / unscaledSize;
        }
    }
}