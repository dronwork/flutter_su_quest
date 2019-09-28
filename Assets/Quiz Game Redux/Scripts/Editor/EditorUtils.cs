using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

namespace AHG.QuizRedux
{
    public class EditorUtils 
    {
    #if UNITY_EDITOR
        public static void SetObjectDirty(Object obj) {
            EditorUtility.SetDirty(obj);
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }
        public static void SetObjectDirty(GameObject obj)
        {
            EditorUtility.SetDirty(obj);
            EditorSceneManager.MarkSceneDirty(obj.scene);
        }

        public static void MakeObjectField<T>(string prefixLabel, ref T obj, bool allowSceneObjects) where T : Object
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(prefixLabel + ":");
            obj = EditorGUILayout.ObjectField(obj, typeof(T), allowSceneObjects) as T;
            EditorGUILayout.EndHorizontal();
        }

        public static void MakeObjectsField<T>(string prefixLabel, List<T> objs, bool allowSceneObjects) where T : Object
        {
            if (objs.Count > 0)
            {
                EditorGUILayout.BeginVertical("box");
                for (int i = 0; i < objs.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(prefixLabel + " " + (i + 1) + ":");
                    objs[i] = EditorGUILayout.ObjectField(objs[i], typeof(T), allowSceneObjects) as T;
                    if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(20)))
                    {
                        objs.RemoveAt(i);
                        break;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
            }
        }

        public static void DoStyledLabelField(string text, GUIStyle style)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField(text, style);
            EditorGUILayout.EndVertical();
        }

        public static void DoSeparator() => EditorGUILayout.LabelField("", new GUIStyle("horizontalSlider"));
    #endif
    }

    public struct GUIStyles
    {
        public static GUIStyle BoldCenterText
        {
            get
            {
                return new GUIStyle("label")
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontStyle = FontStyle.Bold
                };
            }
        }
    }
}
