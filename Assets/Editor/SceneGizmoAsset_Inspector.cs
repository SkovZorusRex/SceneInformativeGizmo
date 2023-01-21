using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using technical.test.editor;

[CustomEditor(typeof(SceneGizmoAsset))]
public class SceneGizmoAsset_Inspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if(GUILayout.Button("Open Editor"))
        {
            var window = (SceneGizmoWindow)EditorWindow.GetWindow(typeof(SceneGizmoWindow),true);
            window.SetCurrentActiveGizmoAsset((SceneGizmoAsset)this.target);
            window.Show();
        }
    }
}
