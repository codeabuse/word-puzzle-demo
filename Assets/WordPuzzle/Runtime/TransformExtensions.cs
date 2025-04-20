using System;
using Codeabuse;
using UnityEngine;

namespace WordPuzzle
{
    public static class TransformExtensions
    {
        public static float GetPositionOnAxis(this Transform t, Axis axis)
        {
            if (((int)axis) is < 0 or > 2)
                throw new ArgumentOutOfRangeException($"Axis index must be between 0 and 2");
            return t.position[(int)axis];
        }

        public static bool Overlaps(this RectTransform self, RectTransform other)
        {
            if (!self.root || self.root != other.root)
            {
                // no common root (canvas or prefab root)
                return false;
            }

            var rootSpaceSelf = self.RootSpaceRect();
            var rootSpaceOther = other.RootSpaceRect();
            return rootSpaceOther.Overlaps(rootSpaceSelf);
        }

        public static Rect RootSpaceRect(this RectTransform self)
        {
            var root = self.root;
            return RelativeTo(self, root);
        }

        private static Rect RelativeTo(this RectTransform self, Transform root)
        {
            var position = (Vector2)root.TransformPoint(self.position);
            return new Rect((Vector2)root.position + position, self.rect.size);
        }
    }
}