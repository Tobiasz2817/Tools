using System;
using UnityEngine;

namespace Tools {
    internal static class ToolsUtility {
        #region Extensions

        /// <summary>
        /// Round vector
        /// </summary>
        internal static Vector2 Round(this Vector2 vector2, int decimals = 0) =>
            new ((float)Math.Round(vector2.x, decimals), (float)Math.Round(vector2.y, decimals));
        
        /// <summary>
        /// Sets any x y values of a Vector2
        /// </summary>
        internal static Vector2 With(this Vector2 vector2, float? x = null, float? y = null) =>
            new (x ?? vector2.x, y ?? vector2.y);
        
        internal static void RoundAnchors(this RectTransform rectTransform, int decimalPlaces) {
            rectTransform.anchorMin = rectTransform.anchorMin.Round(decimalPlaces);
            rectTransform.anchorMax = rectTransform.anchorMax.Round(decimalPlaces);
        }

        #endregion
        
        
        
        
    }
}