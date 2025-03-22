#if UNITY_EDITOR
using CoreUtility.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace Tools {
    public static class AnchorConverter 
    {
        //TODO: Fix issue weird values after conversion ("-2.002001e-0")
        [MenuItem("Tools/Convert To Anchors")]
        public static void SetAnchorsBasedOnPosition() {
            var selectedObjects = Selection.gameObjects;
            Assert.IsTrue(selectedObjects.Length != 0, "Select some object");

            foreach (var selectedObject in selectedObjects) {
                var rectTransform = selectedObject.GetComponent<RectTransform>();
                if (rectTransform == null) 
                    continue;
                
                var isCenter = Mathf.Approximately(rectTransform.anchorMin.x, 0.5f) &&
                                Mathf.Approximately(rectTransform.anchorMin.y, 0.5f) &&
                                Mathf.Approximately(rectTransform.anchorMax.x, 0.5f) &&
                                Mathf.Approximately(rectTransform.anchorMax.y, 0.5f);

                if (!isCenter) {
                    Debug.LogWarning("Is not in center, use the middle center mode and next again use converter");
                    continue;
                }
                
                var parentRectTransform = rectTransform.parent as RectTransform;
                if (parentRectTransform == null) {
                    Debug.LogWarning("Cannot convert without parent");
                    continue;
                }
                
                var parentSize = parentRectTransform.rect.size;
                var position = rectTransform.anchoredPosition;
                var size = rectTransform.sizeDelta;
                var pivot = rectTransform.pivot;
                
                var originalPosition = rectTransform.localPosition;
                
                var anchorMin = new Vector2(
                    (position.x - size.x * pivot.x) / parentSize.x + pivot.x,
                    (position.y - size.y * pivot.y) / parentSize.y + pivot.y
                );
                var anchorMax = new Vector2(
                    (position.x + size.x * (1 - pivot.x)) / parentSize.x + pivot.x,
                    (position.y + size.y * (1 - pivot.y)) / parentSize.y + pivot.y
                );
                
                const int DecimalMax = 3;
                rectTransform.anchorMin = anchorMin.Round(DecimalMax);
                rectTransform.anchorMax = anchorMax.Round(DecimalMax);
                
                rectTransform.localPosition = originalPosition;
                
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Round(size.x));
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Round(size.y));
                
                const int ResetLoop = 3;
                for (int i = 0; i < ResetLoop; i++)
                {
                    rectTransform.offsetMin = Vector2.zero;
                    rectTransform.offsetMax = Vector2.zero;
                }
            }
        }
    }
}
#endif
