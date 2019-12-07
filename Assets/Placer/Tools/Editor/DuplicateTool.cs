using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.IMGUI.Controls;
using UnityEditor.ShortcutManagement;
using UnityEditorInternal;
using UnityEngine;

namespace Placer.PlacerTools
{
    [EditorTool("Duplicator", typeof(GameObject))]
    public class DuplicateTool : EditorTool
    {
        [SerializeField]
        private Texture2D toolIcon = null;

        public override GUIContent toolbarIcon => new GUIContent()
        {
            image = toolIcon,
            text = "Duplicator",
            tooltip =
                "Duplicate selection.\n" +
                "\n" +
                "- Hold Alt after clicking control handle to pin center in place.\n" +
                "- Hold Shift after clicking control handle to scale uniformly.",
        };

        public delegate void PerformedDuplicate();
        public PerformedDuplicate performedDuplicate = null; 

        [SerializeField]
        [HideInInspector]
        private Bounds objectBounds, duplicationBounds = new Bounds();

        private BoxBoundsHandle boxBoundsHandle = new BoxBoundsHandle();

        // The tool breaks if it is selected when re-initialising the code. This method deselects the tool on initialization.
        [InitializeOnLoadMethod]
        [SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Called on initialization")]
        private static void RestorePreviousIfSelectedOnInitialize()
        {
            if(EditorTools.activeToolType == typeof(DuplicateTool))
                EditorTools.RestorePreviousPersistentTool();
        }

        [Shortcut("Tools/Duplicate", KeyCode.D)]
        [SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Called by the Shortcut Manager")]
        private static void SelectToolShortcut()
        {
            // Deselect the tool if it is selected, Select the tool if it can be selected.
            if(EditorTools.activeToolType == typeof(DuplicateTool))
                EditorTools.RestorePreviousPersistentTool();
            else if (Selection.gameObjects.Length > 0)
                EditorTools.SetActiveTool<DuplicateTool>();
        }

        private void OnEnable()
        {
            boxBoundsHandle.wireframeColor = Handles.zAxisColor;
            EditorTools.activeToolChanged += ResetBoundsIfSelected;
            EditorTools.activeToolChanging += DuplicateIfDeselected;
        }

        private void OnDisable()
        {
            EditorTools.activeToolChanged -= ResetBoundsIfSelected;
            EditorTools.activeToolChanging -= DuplicateIfDeselected;
        }

        // Whenever the active Editor Tool has changed, this method is called to check if this tool was selected.
        // If it is, the tool does everything it needs to do on selection.
        // This way, these methods are called even when the Editor Tool is changed from script.
        private void ResetBoundsIfSelected()
        {
            if(EditorTools.IsActiveTool(this))
            {
                ResetBounds();
                Selection.selectionChanged += ResetBounds;
                Undo.undoRedoPerformed += ResetHandleBounds;
            }
        }

        // Same as above, but for deselection.
        private void DuplicateIfDeselected()
        {
            if(EditorTools.IsActiveTool(this))
            {
                Selection.selectionChanged -= ResetBounds;
                Undo.undoRedoPerformed -= ResetHandleBounds;
                Duplicate();
            }
        }

        // When the selection is changed, reset the bounds of the tool.
        public void ResetBounds()
        {
            GameObject[] selectedGameObjects = Selection.gameObjects;
            if(selectedGameObjects.Length > 0)
            {
                objectBounds = InternalEditorUtility.CalculateSelectionBounds(true, false);

                foreach(var point in from GameObject gameObject in selectedGameObjects
                                     from Transform transform in gameObject.GetComponentsInChildren<Transform>()
                                     where transform.GetComponent<Renderer>() == null
                                     select transform.position)
                    objectBounds.Encapsulate(point);

                if(objectBounds.size == Vector3.zero)
                    objectBounds = new Bounds(selectedGameObjects[0].transform.position, Vector3.one);
            }
            else
            {
                objectBounds = default;
            }

            duplicationBounds.SetMinMax(objectBounds.center, objectBounds.center);
            ResetHandleBounds();
        }

        // Handle Bounds need to be manually changed on UndoRedoPerformed because it can't be serialized.
        // Basically, don't worry about it.
        private void ResetHandleBounds()
        {
            boxBoundsHandle.center = duplicationBounds.center;
            boxBoundsHandle.size = duplicationBounds.max - duplicationBounds.min + objectBounds.size;
        }

        // Duplicate the selection and move them accordingly.
        public void Duplicate()
        {
            if(Mathf.Approximately(objectBounds.size.x, 0)
                || Mathf.Approximately(objectBounds.size.y, 0)
                || Mathf.Approximately(objectBounds.size.z, 0))
                return;

            Vector3 offset = new Vector3();
            for(offset.x = duplicationBounds.min.x; offset.x < duplicationBounds.max.x || Mathf.Approximately(offset.x, duplicationBounds.max.x); offset.x += objectBounds.size.x)
                for(offset.y = duplicationBounds.min.y; offset.y < duplicationBounds.max.y || Mathf.Approximately(offset.y, duplicationBounds.max.y); offset.y += objectBounds.size.y)
                    for(offset.z = duplicationBounds.min.z; offset.z < duplicationBounds.max.z || Mathf.Approximately(offset.z, duplicationBounds.max.z); offset.z += objectBounds.size.z)
                        if(offset != objectBounds.center)
                            InstantiateObjects(offset - objectBounds.center);

            Selection.objects = new Object[0];

            performedDuplicate?.Invoke();

            void InstantiateObjects(Vector3 translation)
            {
                Selection.objects = targets.ToArray();

                SceneView.lastActiveSceneView.Focus();
                if(EditorWindow.focusedWindow.SendEvent(EditorGUIUtility.CommandEvent("Duplicate")))
                {
                    for(int i = 0; i < Selection.gameObjects.Length; ++i)
                    {
                        GameObject go = Selection.gameObjects[i];
                        go.transform.Translate(translation, Space.World);
                    }
                }
            }
        }

        // Draw the tool.
        public override void OnToolGUI(EditorWindow window)
        {
            if(objectBounds.size == Vector3.zero)
                return;

            EditorGUI.BeginChangeCheck();

            boxBoundsHandle.DrawHandle();
            
            // When the handle is resized, update the necessary values.
            if(EditorGUI.EndChangeCheck())
            {
                Vector3 min = new Vector3();
                Vector3 max = new Vector3();

                for(int axis = 0; axis < 3; ++axis)
                {
                    float handleAxisMin = boxBoundsHandle.center[axis] - boxBoundsHandle.size[axis] / 2;
                    float handleAxisMax = handleAxisMin + boxBoundsHandle.size[axis];
                    float axisSize = objectBounds.size[axis];
                    float axisCenter = objectBounds.center[axis];

                    int behind = Mathf.Approximately(objectBounds.min[axis], handleAxisMin) ? 0 : Mathf.FloorToInt((objectBounds.min[axis] - handleAxisMin) / axisSize);
                    int ahead = Mathf.Approximately(handleAxisMax, objectBounds.max[axis]) ? 0 : Mathf.FloorToInt((handleAxisMax - objectBounds.max[axis]) / axisSize);

                    min[axis] = axisCenter - axisSize * behind;
                    max[axis] = axisCenter + axisSize * ahead;
                }

                duplicationBounds.SetMinMax(min, max);

                Undo.RegisterCompleteObjectUndo(this, "Bounds Changed");
            }

            Handles.color = Handles.xAxisColor;
            Handles.DrawWireCube(duplicationBounds.center, duplicationBounds.max - duplicationBounds.min + objectBounds.size);
        }
    }
}
