using UnityEngine.Assertions;
using CoreUtility.Extensions;
using UnityEditor;
using System.Linq;
using UnityEngine;
using TMPro;

namespace Tools {
    public static class TextSizeAdjuster {
        /// <summary>
        /// Scaling anchors based on font size the longest text
        /// Usage: select the texts to adjust with others, program find the longest text and based on him font size,
        /// adjust the others texts.
        /// Press shift + m or use it from Tools/Anchors -> Set AutoFont Sizes
        /// </summary>
        [MenuItem("Tools/Anchors/Set AutoFont Sizes #m")]
        static void AutoSizeFont() {
            int length = Selection.objects.Length;
            var selections = Selection.objects;
 
            Assert.IsTrue(length > 0, "You must select at least two objects, base and to change");

            TextMeshProUGUI coreTMP = selections.
                Select(target => target as GameObject).
                Select(go => go!.GetComponent<TextMeshProUGUI>()).
                OrderByDescending(tmp => tmp.preferredWidth).
                FirstOrDefault();

            Assert.IsNotNull(coreTMP, "Core target must be tmp pro and must be selected as first");

            // Temporary solution -> Move to window editor
            const float SubtractValue = 0.0001f;
            // Range 0 - 1
            const float MinYAnchor = 0.2f;
            const float MaxYAnchor = 0.8f;
            const float MinXAnchor = 0.49f;
            const float MaxXAnchor = 0.51f;

            const float MinAnchorValue = 0f;
            const float MaxAnchorValue = 1f;
            
            float fontSize = coreTMP.fontSize;
            for (int i = 0; i < selections.Length; i++) {
                var target = selections[i] as GameObject;
                Assert.IsNotNull(target, "Selection object must be GameObject");
 
                var targetTMP = target.GetComponent<TextMeshProUGUI>();
                Assert.IsNotNull(targetTMP, "Target must be tmp pro and must be selected as first");

                if (!Mathf.Approximately(targetTMP.fontSize, fontSize)) {
                    targetTMP.rectTransform.anchorMin = new Vector2(MinAnchorValue, MinAnchorValue);
                    targetTMP.rectTransform.anchorMax = new Vector2(MaxAnchorValue, MaxAnchorValue);
                    
                    targetTMP.ForceMeshUpdate();
                }
                
                while (!Mathf.Approximately(targetTMP.fontSize, fontSize)) {
                    Assert.IsFalse(Mathf.Approximately(targetTMP.fontSize, targetTMP.fontSizeMin), "Invalid font size ...");
                    
                    Vector2 anchorMin = targetTMP.rectTransform.anchorMin;
                    Vector2 anchorMax = targetTMP.rectTransform.anchorMax;

                    float minX = Mathf.Abs(anchorMin.x - MinXAnchor) > 0.0001f ? anchorMin.x + SubtractValue : MinXAnchor;
                    float maxX = Mathf.Abs(anchorMax.x - MaxXAnchor) > 0.0001f ? anchorMax.x - SubtractValue : MaxXAnchor;
                    float minY = Mathf.Abs(anchorMin.y - MinYAnchor) > 0.0001f ? anchorMin.y + SubtractValue : MinYAnchor;
                    float maxY = Mathf.Abs(anchorMax.y - MaxYAnchor) > 0.0001f ? anchorMax.y - SubtractValue : MaxYAnchor;
                    
                    targetTMP.rectTransform.anchorMin = anchorMin.With(minX, minY);
                    targetTMP.rectTransform.anchorMax = anchorMax.With(maxX, maxY);
                    
                    targetTMP.ForceMeshUpdate();
                    EditorUtility.SetDirty(targetTMP);
                }
                
                Undo.RecordObject(target, "Auto Font Size");
            }
        }
    }
}