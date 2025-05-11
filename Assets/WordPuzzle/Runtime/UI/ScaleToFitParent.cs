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
        [Tooltip("Axis by which the object's RectTransform will be scaled to fit parent's axis. Scale is unitform.")]
        private RectTransform.Axis _referenceAxis;

        protected override void Awake()
        {
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

            var proposedScale = GetTargetScaleToFit(_self, _parent, _referenceAxis);
            if (float.IsInfinity(proposedScale))
                return;

            var selfRect = _self.rect;
            var parentRect = _parent.rect;
            
            var orthogonalAxisIndex = (axisIndex + 1) % 2;
            var orthogonalSide = selfRect.size[orthogonalAxisIndex];
            var orthogonalSideScaled = orthogonalSide * proposedScale;
            var parentOrthogonalSide = parentRect.size[orthogonalAxisIndex];
            if (orthogonalSideScaled > parentOrthogonalSide)
            {
                var orthogonalSideCorrection = parentOrthogonalSide / orthogonalSideScaled;
                
                if (float.IsInfinity(orthogonalSideCorrection))
                    return;
                
                proposedScale *= orthogonalSideCorrection;
            }
            
            if (Mathf.Abs(currentScale - proposedScale) < presicision)
                return;
            
            if (_self.root == false)
                Debug.LogError($"Attempt to modify local scale of rootless transform!", this);
            

            _self.localScale = Vector3.one * proposedScale;
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