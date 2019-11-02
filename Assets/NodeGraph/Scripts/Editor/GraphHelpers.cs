using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeGraph.Editor
{
    public static class GraphHelpers
    {
        public static Vector2 MousePosition()
        {
            return ScreenToGraphSpace(NodeGraphEditorBase.NodeGraphGlobals.CurrentEvent.mousePosition);
        }
        
        public static Vector2 ScreenToGraphSpace(Vector2 screenPos)
        {
            var graphRect = NodeGraphEditorBase.NodeGraphGlobals.GraphView;
            var center = graphRect.size / 2f;
            return (screenPos) - NodeGraphEditorBase.NodeGraphGlobals.PanOffset;
        }

        public static Vector2 GraphToScreenSpace(Vector2 graphPos)
        {
            return graphPos + NodeGraphEditorBase.NodeGraphGlobals.PanOffset;
        }

        public static void GraphToScreenSpace(ref Vector2 graphPos)
        {
            graphPos += NodeGraphEditorBase.NodeGraphGlobals.PanOffset;
        }
        
        public static bool IsUnderMouse(Rect r)
        {
            return r.Contains(MousePosition());
        }
        
        public static Texture2D MakeTex( int width, int height, Color col )
        {
            Color[] pix = new Color[width * height];
            for( int i = 0; i < pix.Length; ++i )
            {
                pix[ i ] = col;
            }
            Texture2D result = new Texture2D( width, height );
            result.SetPixels( pix );
            result.Apply();
            return result;
        }
    }
}

