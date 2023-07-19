using UnityEditor;
using UnityEngine;
using UnityEditor.UI;

namespace Wrapper
{
    [CustomEditor(typeof(DialogueButton), true)]
    [CanEditMultipleObjects]
    public class DialogueButtonEditor : ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Custom Elements", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("step"), 
                new GUIContent("Step", "Choose whether this button moves dialogue forward of backward"), true);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("audioName"),
                new GUIContent("Button Click Audio Name", "Use this audio clip instead of the default button audio"), true);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Default Button Elements", EditorStyles.boldLabel);

            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
        }
    }
}
