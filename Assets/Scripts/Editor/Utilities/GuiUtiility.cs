using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class GuiUtiility
{
    public static readonly GUILayoutOption OPT_EXPAND_WIDTH = GUILayout.ExpandWidth(true);
    public static readonly GUILayoutOption OPT_HEIGHT_H1 = GUILayout.Height(2);

    public static Color HEADER_COLOR = new Color(0.1f, 0.1f, 0.1f, 0.9f);
    
    public static GUIStyle boxGUIStyle = new GUIStyle()
    {
        alignment = TextAnchor.MiddleCenter,
        font = EditorStyles.label.font,
        fontSize = 14,
        fontStyle = FontStyle.Bold,
    };
    
    public static void Line()
    {
        GUILayout.Space(2);
        GUILayout.Box("", OPT_HEIGHT_H1, OPT_EXPAND_WIDTH);
        GUILayout.Space(2);
    }

    public static void Header(string title, int height)
    {
        boxGUIStyle.normal.textColor = HEADER_COLOR;
        GUILayout.Space(height);
        GUILayout.Label(title, boxGUIStyle);
        GUILayout.Space(height);
    }
}
