using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using technical.test.editor;

internal class GizmoObject
{
    public TextField textField;
    public Vector3Field vectorField;
    public Button button;
    public SerializedProperty name;
    public SerializedProperty position;
    public Vector3 originalPosition;
    public Box box;
}

public class SceneGizmoWindow : EditorWindow
{
    SceneGizmoAsset currentAsset = null; //The Current Loaded Scriptable
    List<GizmoObject> gizmoObjects = new List<GizmoObject>(); //List of all the object containing Data
    Box informationContainer; //The Parent used to display each gizmo
    GizmoObject currentSelectedObject; //The currentSelectedObject (updated when clicking edit button)
    SerializedObject baseObject; //The SerializedObject ie the current Scriptable _gizmos
    ObjectField objectField; //The field use to load scriptable manually

    [MenuItem("Window/Custom/Show Gizmos")]
    public static void ShowWindow()
    {
        SceneGizmoWindow wnd = GetWindow<SceneGizmoWindow>();
        wnd.titleContent = new GUIContent("SceneGizmos");
    }
    private void Awake()
    {
        currentAsset = ScriptableObject.CreateInstance<SceneGizmoAsset>(); //Create instance of a scriptable
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += this.OnSceneGUI; //Register to the Scene View on GUI draw (in order to draw our gizmos)
    }

    private void OnDestroy()
    {
        SceneView.duringSceneGui -= this.OnSceneGUI; //Unregister when closing the window
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        VisualElement label = new Label("Select a Scriptable Object");
        objectField = new ObjectField();
        objectField.objectType = typeof(SceneGizmoAsset); //Limit the asset to SceneGizmoAsset only
        objectField.RegisterValueChangedCallback(SetObject); //Subscribe to the ValueChanged Event (ie we change the current scriptable to load)

        informationContainer = new Box();

        root.Add(label);
        root.Add(objectField);
        root.Add(informationContainer);
    }

    //Call from outside to Load a specific ScriptableObject
    public void SetCurrentActiveGizmoAsset(SceneGizmoAsset asset)
    {
        currentAsset = asset;
        objectField.SetValueWithoutNotify(asset);
        SetObject(null);
    }

    //Load and display all properties of the selected Scriptable
    private void SetObject(ChangeEvent<Object> evt)
    {
        //Clear to prevent duplicates
        informationContainer.Clear();
        gizmoObjects.Clear();

        //When called by the objectField
        if (evt != null)
            currentAsset = (SceneGizmoAsset)evt.newValue;

        //Prevent error when switching to none
        if (currentAsset == null)
        {
            currentSelectedObject = null; //Hide the movement handle if switching when in edit mode
            return;
        }

        //Using SerializedObject and SerializedProperty will automatically mark the object as dirty, create an undo entry
        baseObject = new UnityEditor.SerializedObject(currentAsset);

        SerializedProperty baseObjectGizmos = baseObject.FindProperty("_gizmos");
        var it_baseObjectGizmos = baseObjectGizmos.GetEnumerator();
        while (it_baseObjectGizmos.MoveNext()) //basically a foreach _gizmos
        {
            var baseObjectGizmoChild = it_baseObjectGizmos.Current as SerializedProperty;
            //Debug.Log(property.propertyPath + " : " + property.name + " : " + property.hasChildren);

            var obj = new GizmoObject(); //Create a class to keep all interesting values
            gizmoObjects.Add(obj);

            //Container of each gizmo element
            obj.box = new Box();
            obj.box.style.flexDirection = FlexDirection.Row;
            obj.box.style.justifyContent = Justify.Center;

            //Use to display and modify gizmo name
            TextField textField = new TextField();
            textField.style.marginRight = 10;
            //textField.style.flexGrow = 1;

            //Use to display and modify gizmo value
            Vector3Field vf = new Vector3Field();
            vf.style.flexGrow = 1;

            //Use to move gizmo directly on scene
            Button button = new Button();
            button.text = "Edit";
            button.clicked += () =>
            {
                SetEditMode(button);
            };


            var it_baseObjectGizmoChild = baseObjectGizmoChild.GetEnumerator();
            //Only the first 2 properties (name and position) // not optimal for general use but work in this case
            //otherwise we would also get each component of the position vector (x,y,z)
            for (int i = 0; i < 2; i++)
            {
                it_baseObjectGizmoChild.MoveNext();
                var childProperty = it_baseObjectGizmoChild.Current as SerializedProperty;
                //Debug.Log(childProperty.propertyPath + " : " + childProperty.name + " : " + childProperty.hasChildren);
                if (childProperty.propertyType == SerializedPropertyType.String) //Here if it's the name
                {
                    textField.BindProperty(childProperty); //Bind the textField to the name property (changing one change the other)
                    obj.textField = textField;
                    obj.name = childProperty;
                }
                if (childProperty.propertyType == SerializedPropertyType.Vector3) //Here if it's the position
                {
                    vf.BindProperty(childProperty);
                    obj.vectorField = vf;
                    obj.position = childProperty;
                    obj.originalPosition = childProperty.vector3Value; //Save the original location (before modification)
                }
            }
            obj.button = button;

            //Add each element to a box (one per gizmo)
            obj.box.Add(textField);
            obj.box.Add(vf);
            obj.box.Add(button);

            //Add each box to the main box (use to display all gizmos informations
            informationContainer.Add(obj.box);

        }
        //Repaint the Window and Scene (due to changes)
        Repaint();
        SceneView.lastActiveSceneView.Repaint();
    }

    //Set the current object we want to move in the scene
    private void SetEditMode(Button button)
    {
        //Linq is costly but works great in editor
        var obj = gizmoObjects.Where(item => item.button == button).FirstOrDefault();
        if (obj != null) //if we find the corresponding object
        {
            if (currentSelectedObject == obj) //Unselect
            {
                obj.button.style.backgroundColor = Color.grey; //WRONG GREY :(
                obj.box.style.backgroundColor = Color.clear; //WRONG AGAIN (not the original unity color) :(
                currentSelectedObject = null;
            }
            else //Select
            {
                if (currentSelectedObject != null) //Reset the previously selected one
                {
                    currentSelectedObject.button.style.backgroundColor = Color.grey;
                    currentSelectedObject.box.style.backgroundColor = Color.clear;
                }
                obj.box.style.backgroundColor = Color.grey;
                obj.button.style.backgroundColor = Color.red;
                currentSelectedObject = obj;
            }
            SceneView.lastActiveSceneView.Repaint(); //Repaint to display Handle quicker
        }
    }

    void OnSceneGUI(SceneView sceneView)
    {
        Handles.color = Color.white;
        GUI.color = Color.black;

        //Create a sphere with the name for each gizmo
        foreach (var gizmo in gizmoObjects)
        {
            Handles.SphereHandleCap(0, gizmo.position.vector3Value, Quaternion.identity, 1f, EventType.Repaint);
            Handles.Label(gizmo.position.vector3Value + new Vector3(0, 1, 0), gizmo.textField.value);
        }

        //Show the MoveHandle if there is a selected object
        if (currentSelectedObject != null)
        {
            //Check for positionChange
            EditorGUI.BeginChangeCheck();
            Vector3 newPos = Handles.PositionHandle(currentSelectedObject.position.vector3Value, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                currentSelectedObject.vectorField.value = newPos; //Due to the binding this will update both vectorField and Gizmo Position
            }
        }

        // If right mouse click
        if (Event.current.button == 1 && Event.current.type == EventType.MouseDown)
        {
            if (currentAsset == null)
                return;
            foreach (var gizmo in gizmoObjects)
            {
                //How far the mouse is from a gizmo sphere
                float dist = Vector2.Distance(Event.current.mousePosition, HandleUtility.WorldToGUIPoint(gizmo.position.vector3Value));
                if (dist < 25f) //Arbitrary distance (more or less the size of the sphere)
                {
                    //Debug.Log("In Range of : " + gizmo.textField.value);
                    ShowMenu(gizmo);
                    return;
                }
            }
        }
    }

    private void ShowMenu(GizmoObject selectedObject)
    {
        var menu = new GenericMenu();
        menu.AddItem(new GUIContent("Reset Position"), false, ResetPosition, selectedObject);
        menu.AddItem(new GUIContent("Delete Gizmo"), false, DeleteGizmo, selectedObject);
        menu.ShowAsContext();
    }

    //Reset the gizmo to the original position
    private void ResetPosition(object selectedObject)
    {
        var obj = selectedObject as GizmoObject;
        obj.vectorField.value = obj.originalPosition;
    }

    //Delete the gizmo (Only visually for now)
    private void DeleteGizmo(object selectedObject)
    {
        var obj = selectedObject as GizmoObject;

        if (currentSelectedObject == obj) //If the object we are deleting is the one we are editing
            SetEditMode(obj.button);

        int objIndex = gizmoObjects.IndexOf(obj);
        //Undo.RecordObject(baseObject.context, "Delete"); //Reverting will restore scriptable obejct element but break editor window ui
        informationContainer.Remove(obj.box);
        obj.position.DeleteCommand();
        obj.name.DeleteCommand();
        gizmoObjects.Remove(obj);

        //Safety unbind
        obj.textField.Unbind();
        obj.vectorField.Unbind();
        obj.button.Unbind();

        // /!\ Will cause error when drawing handle (line 196 & 197) because the property is no longer there
        //Apparently Fixed in later version : https://issuetracker.unity3d.com/issues/serializedproperty-counters-dot-array-dot-data-1-has-disappeared-error-when-removing-array-item-containing-managed-reference
        //baseObject.FindProperty("_gizmos").DeleteArrayElementAtIndex(objIndex); //Delete the element
        //baseObject.ApplyModifiedProperties(); //Save the delete

        SceneView.lastActiveSceneView.Repaint();
        Repaint();
    }
}