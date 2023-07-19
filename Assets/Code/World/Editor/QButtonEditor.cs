using UnityEditor;
using UnityEngine;
using UnityEditor.UI;

namespace Wrapper
{
    [CustomEditor(typeof(QButton), true)]
    [CanEditMultipleObjects]
    public class QButtonEditor : ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Custom Elements", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("audioName"), 
                new GUIContent("Button Click Audio Name", "Use this audio clip instead of the default Button Audio"), true);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Default Button Elements", EditorStyles.boldLabel);

            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
        }
    }
}
