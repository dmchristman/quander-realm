using UnityEditor;
using UnityEngine;
using UnityEditor.UI;

namespace Wrapper
{
    [CustomEditor(typeof(MinigameButton), true)]
    [CanEditMultipleObjects]
    public class MinigameButtonEditor : ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Custom Elements", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("minigame"), 
                new GUIContent("Minigame", "Minigame to load"), true);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("audioName"),
                new GUIContent("Button Click Audio Name", "Use this audio clip instead of the default button audio"), true);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Default Button Elements", EditorStyles.boldLabel);

            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
        }
    }
}
