using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeGraph.Editor
{
    public static class GUILayoutHelpers
    {
        public static void RenderInsideBox(GUIStyle renderStyle, Action onGUI)
        {
            GUILayout.BeginVertical(renderStyle);

            onGUI?.Invoke();

            GUILayout.EndVertical();
        }

        public static void DrawBezier(Vector2 start, Vector2 end, Color color, float thickness = 3.0f)
        {
            Vector2 endToStart = (end - start);
            float dirFactor = Mathf.Clamp(endToStart.magnitude, 20.0f, 80.0f);
            
            endToStart.Normalize();
            Vector2 project = Vector3.Project(endToStart, Vector3.right);

            Vector2 startTan = start + project * dirFactor;
            Vector2 endTan = end - project * dirFactor;
            
            UnityEditor.Handles.DrawBezier(start, end, startTan, endTan, color, null, thickness);
        }
    }
}

