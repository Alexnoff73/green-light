using UnityEditor;
using UnityEngine;

namespace WaypointEditor
{
    [CustomEditor(typeof(WaypointPath))]
    public class WaypointPathEditor : UnityEditor.Editor
    {
        static public KeyCode addPathPointHotKey = KeyCode.A;


        //Draw labels this distance above the move handle
        const float LABEL_Y_OFFSET = 1f;
        const int FONT_SIZE = 16;


        static float lastInputTime;


        //Path we are editing
        WaypointPath path;


        public void OnEnable()
        {
            path = (WaypointPath)target;
            SceneView.onSceneGUIDelegate += ShortcutKeyUpdate;
        }

        public void OnDisable()
        {
            SceneView.onSceneGUIDelegate -= ShortcutKeyUpdate;
        }


        void ShortcutKeyUpdate(SceneView sceneview)
        {
            if (Selection.activeGameObject == null)
                return;

            Event e = Event.current;

            if (e != null
                && e.keyCode == addPathPointHotKey)
            {
                //Check if has been sufficient amount of time since last input, to prevent long spams of adding new points.
                if (Time.realtimeSinceStartup > (lastInputTime + .15f))
                {
                    AddPathPoint();
                }
                lastInputTime = Time.realtimeSinceStartup;
            }

        }



        //Everything in here is called only when the path is selected.
        void OnSceneGUI()
        {
            //Wait until path is set
            if (!path)
                return;

            GUIStyle textStyle = new GUIStyle();
            textStyle.normal.textColor = Color.white;
            textStyle.fontSize = FONT_SIZE;
            textStyle.alignment = TextAnchor.UpperCenter;


            //Draw handle for path
            DrawLabeledMoveHandle(path.transform, path.name, textStyle);

            //Draw handle for each path points
            for (int i = 0; i < path.Points.Length; i++)
            {
                var element = path.Points[i];
                if (element == null)
                    continue;

                DrawLabeledMoveHandle(element, i.ToString(), textStyle);
            }
        }


        public override void OnInspectorGUI()
        {
            InspectorDrawPathButtons();

            GUILayout.Space(24);

            path.pathColor = EditorGUILayout.ColorField("Path Color:", path.pathColor);

            path.loopsAround = EditorGUILayout.Toggle("Loops Around:", path.loopsAround);

            GUILayout.Space(16);
            InspectorDrawLineAttributes();

        }


        /// <summary>
        /// Draws a handle with a label above it. This handle lets you move <paramref name="toDisplay"/> and undo or redo the change.
        /// </summary>
        /// <param name="toDisplay"></param>
        /// <param name="labelText"></param>
        /// <param name="style"></param>
        void DrawLabeledMoveHandle(Transform toDisplay, string labelText, GUIStyle style)
        {
            //Draw move handle
            var newPos = Handles.PositionHandle(toDisplay.position, toDisplay.rotation);
            if (newPos != toDisplay.position)//If position change
            {
                Undo.RecordObject(toDisplay, "Move Transform :" + toDisplay);
                toDisplay.position = newPos;
            }

            //Draw label
            Handles.Label(toDisplay.position + Vector3.up * LABEL_Y_OFFSET, labelText, style);
        }

        void AddPathPoint()
        {
            var pos = path.transform.position;
            //Create new point in front of one last in path
            if (path.Points.Length > 0)
                pos = path.Points[path.Points.Length - 1].position + new Vector3(0, 0, 1);
            path.CreateNewPathPoint(pos);

        }

        void InspectorDrawPathButtons()
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("+ [A]dd Point"))
                AddPathPoint();

            if (GUILayout.Button("Flatten Path"))
                path.Flatten();

            if (GUILayout.Button("Rename Points"))
                path.RenamePointsAccordingToOrder();

            GUILayout.EndHorizontal();
        }


        void InspectorDrawLineAttributes()
        {
            path.lineType = (WaypointPath.LineType)EditorGUILayout.EnumPopup("Line Type", path.lineType);

            //Display line interval option if its a dotted line
            if (path.lineType == WaypointPath.LineType.Dotted)
                path.lineInterval = EditorGUILayout.FloatField("Dot interval", path.lineInterval);

            //Display path points
            int pointCount = path.Points.Length;

            if (pointCount > 0)
            {
                EditorGUILayout.LabelField("Path points:");

                for (int i = 0; i < pointCount; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    var element = path.Points[i];
                    path.Points[i] = (Transform)EditorGUILayout.ObjectField(element, typeof(Transform), true);

                    if (GUILayout.Button("Delete", GUILayout.Width(48)))
                    {
                        //Destroy the point, and if in edit mode, store it in the undo buffer such
                        // that you can undo the destroy action
                        Utilities.SafeDestroyWithUndo(element.gameObject);
                        break;
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
        }


    }
}