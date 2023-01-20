using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using technical.test.editor;

class GizmoObject
{
    public GizmoObject(Gizmo g, Vector3Field vf, Button b) { gizmo = g; vectorField = vf; button = b; }
    public Gizmo gizmo;
    public Vector3Field vectorField;
    public Button button;
}

public class SceneGizmoWindow : EditorWindow
{
    SceneGizmoAsset currentAsset = null;
    List<GizmoObject> gizmoObjects = new List<GizmoObject>();
    Box informationContainer;
    GizmoObject currentSelectedObject;

    [MenuItem("Window/Custom/Show Gizmos")]
    public static void ShowWindow()
    {
        SceneGizmoWindow wnd = GetWindow<SceneGizmoWindow>();
        wnd.titleContent = new GUIContent("SceneGizmos");
    }
    private void OnEnable()
    {
        //SceneView.onSceneGUIDelegate += this.OnSceneGUI;
        SceneView.duringSceneGui += this.OnSceneGUI;
    }

    private void OnDestroy()
    {
        SceneView.duringSceneGui -= this.OnSceneGUI;
#if UNITY_EDITOR
        EditorUtility.SetDirty(currentAsset);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
#endif
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy.
        VisualElement label = new Label("Hello World! From C#");
        ObjectField objectField = new ObjectField();
        objectField.objectType = typeof(SceneGizmoAsset);
        objectField.RegisterValueChangedCallback(SetObject);

        informationContainer = new Box();

        root.Add(label);
        root.Add(objectField);
        root.Add(informationContainer);
    }

    public void SetCurrentActiveGizmoAsset(SceneGizmoAsset asset)
    {
        Debug.Log(asset.name);
        currentAsset = asset;
        Repaint();
    }

    private void SetPosition(ChangeEvent<Vector3> evt)
    {
        //Using linq has a cost but since it's in editor it's not really important
        var obj = gizmoObjects.Where(item => item.vectorField == evt.target).FirstOrDefault();
        if (obj.vectorField != null)
        {
            obj.gizmo.Position = obj.vectorField.value;
            AssetDatabase.SaveAssets();
            SceneView.lastActiveSceneView.Repaint();
        }
    }

    private void SetObject(ChangeEvent<Object> evt)
    {
        currentAsset = (SceneGizmoAsset)evt.newValue;
//#if UNITY_EDITOR
//        EditorUtility.SetDirty(currentAsset);
//#endif
        if (currentAsset != null)
        {
            informationContainer.Clear();
            foreach (var gizmo in currentAsset.GetGizmos())
            {
                Box box = new Box();
                box.style.flexDirection = FlexDirection.Row;
                //box.style.justifyContent = Justify.Center;
                Vector3Field vf = new Vector3Field(gizmo.Name);
                vf.value = gizmo.Position;
                vf.style.flexGrow = 1;
                vf.RegisterValueChangedCallback(SetPosition);

                Button button = new Button();
                button.text = "Edit";
                button.clicked += () =>
                {
                    SetEditMode(button);
                };

                var obj = new GizmoObject(gizmo, vf, button);
                gizmoObjects.Add(obj);


                box.Add(vf);
                box.Add(button);

                informationContainer.Add(box);
                //SceneView.lastActiveSceneView.Repaint();
            }
        }
        Repaint();
    }

    private void SetEditMode(Button button)
    {
        var obj = gizmoObjects.Where(item => item.button == button).FirstOrDefault();
        if (obj.button != null)
        {
            currentSelectedObject = obj;
        }
    }

    void OnSceneGUI(SceneView sceneView)
    {
        //Handles.BeginGUI();
        //Handles.EndGUI();
        //Handles.DrawSolidDisc(new Vector3(5, 15, 0), Vector3.up, 5);
        //Handles.SphereHandleCap(0, new Vector3(5, 15, 0), Quaternion.identity, 1f, EventType.Repaint);
        Handles.color = Color.white;
        GUI.color = Color.black;
        foreach (var gizmo in gizmoObjects)
        {
            Handles.SphereHandleCap(0, gizmo.gizmo.Position, Quaternion.identity, 1f, EventType.Repaint);
            Handles.Label(gizmo.gizmo.Position + new Vector3(0, 1, 0), gizmo.gizmo.Name);
        }

        if (currentSelectedObject != null)
        {
            EditorGUI.BeginChangeCheck();
            Vector3 t = Handles.PositionHandle(currentSelectedObject.gizmo.Position, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                //Undo.RecordObject(currentSelectedObject, "Update Position");
                currentSelectedObject.gizmo.Position = t;
                currentSelectedObject.vectorField.value = t;
                //AssetDatabase.SaveAssets();
                //AssetDatabase.Refresh();
                //sceneView.Repaint();
            }
        }

    }
}