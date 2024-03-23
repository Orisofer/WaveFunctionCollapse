using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WFC
{
    [CustomEditor(typeof(Tile))]
    public class TileEditor : Editor
    {
        private SerializedObject so;
        
        private SerializedProperty propSprite;
        
        private SerializedProperty propEdgeUp;
        private SerializedProperty propEdgeDown;
        private SerializedProperty propEdgeLeft;
        private SerializedProperty propEdgeRight;

        private void OnEnable()
        {
            so = serializedObject;
            
            propSprite = so.FindProperty("TileSprite");

            propEdgeUp = so.FindProperty("EdgeUp");
            propEdgeDown = so.FindProperty("EdgeDown");
            propEdgeLeft = so.FindProperty("EdgeLeft");
            propEdgeRight = so.FindProperty("EdgeRight");
            
        }

        public override void OnInspectorGUI()
        {
            so.Update();
            
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GuiUtiility.Header("WFC Tile", 18);
            }
            
            EditorGUILayout.Space(4);
            
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.PropertyField(propSprite);

                using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                {
                    GUILayout.FlexibleSpace();
                    Texture2D tex = AssetPreview.GetAssetPreview(propSprite.objectReferenceValue);
                    GUILayout.Label(tex, GUILayout.Width(100), GUILayout.Height(100));
                    GUILayout.FlexibleSpace();
                }
                
            }
            
            EditorGUILayout.Space(4);
            
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.PropertyField(propEdgeUp);
                EditorGUILayout.Space(4);
                EditorGUILayout.PropertyField(propEdgeDown);
                EditorGUILayout.Space(4);
                EditorGUILayout.PropertyField(propEdgeLeft);
                EditorGUILayout.Space(4);
                EditorGUILayout.PropertyField(propEdgeRight);
            }

            so.ApplyModifiedProperties();
        }
    }
}

