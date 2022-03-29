#if (UNITY_EDITOR) 
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace WaypointEditor
{
    public class WaypointWindow : EditorWindow
    {
        //Current position of the scrolling rect for all of the paths.
        Vector2 scrollPosition;

        // An array of all WaypointPath in the scene.
        public static WaypointPath[] waypointsPaths;

        //Path merger
        AnimBool showMerger;
        WaypointPath firstPathToMerge, secondPathToMerge;

        /// <summary>
        /// Allow to open the window via the unity menu.
        /// </summary>
        [MenuItem("Window/Waypoint Paths")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(WaypointWindow));
        }

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        void OnEnable()
        {
            showMerger = new AnimBool(false);
            showMerger.valueChanged.AddListener(Repaint);
        }


        public static WaypointPath CreateNewPath()
        {
            var newPath = new GameObject("Path #" + waypointsPaths.Length);
            var waypointPath = newPath.AddComponent<WaypointPath>();
            waypointPath.Initialize();
            Undo.RegisterCreatedObjectUndo(waypointPath.gameObject, "Created new path");

            return waypointPath;
        }

        public void OnInspectorUpdate()
        {
            //Here I get all paths in the scene. It must be done this way because the user can also add paths manually instead of through the GUI.
            //Since this is for the editor only and not in game, we don't have to worry about performance issues.
            waypointsPaths = FindObjectsOfType<WaypointPath>();

            // This will only get called 10 times per second. This updates the window to show any new paths founds or remove paths deleted
            Repaint();
        }


        void OnGUI()
        {
            //Create scroll bar and remember where we scrolled to.
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            //Title
            DrawStyledLabel("Waypoint Paths", FontStyle.Bold, TextAnchor.UpperCenter, 20);


            if (GUILayout.Button("+ Add Path"))
                CreateNewPath();

            //Path merger
            showMerger.target = EditorGUILayout.ToggleLeft("Show Path Merger", showMerger.target);
            if (EditorGUILayout.BeginFadeGroup(showMerger.faded))
            {
                DrawPathMerger();
            }
            EditorGUILayout.EndFadeGroup();

            GUILayout.Space(32);

            if (waypointsPaths != null)
            {
                for (int i = 0; i < waypointsPaths.Length; i++)
                {
                    var path = waypointsPaths[i];
                    if (path == null)
                        continue;

                    GUILayout.BeginHorizontal();

                    DrawPathButtonsAndInfo(path);

                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndScrollView();

        }

        #region Window Draw Methods

        void DrawStyledLabel(string text, FontStyle fontStyle, TextAnchor alignment, int fontSize)
        {
            var style = new GUIStyle();
            style.alignment = alignment;
            style.fontStyle = fontStyle;
            style.fontSize = fontSize;
            style.wordWrap = true;

            EditorGUILayout.LabelField(text, style);
        }

        /// <summary>
        /// Simple shorthand method used for functions such as merge, to keep a unified style for their titles and descriptions.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="description"></param>
        void DrawFunctionWindow(string title, string description)
        {
            DrawStyledLabel(title, FontStyle.Normal, TextAnchor.MiddleCenter, 12);

            DrawStyledLabel(description, FontStyle.Normal, TextAnchor.UpperLeft, 10);
        }

        void DrawPathMerger()
        {
            DrawFunctionWindow("Merge Paths", "Merges path A into path B.");

            firstPathToMerge = (WaypointPath)EditorGUILayout.ObjectField("Path A", firstPathToMerge, typeof(WaypointPath), true);
            secondPathToMerge = (WaypointPath)EditorGUILayout.ObjectField("Path B", secondPathToMerge, typeof(WaypointPath), true);

            if (GUILayout.Button("Merge!"))
            {
                if (WaypointPathUtilities.MergePaths(firstPathToMerge, secondPathToMerge))
                {
                    //Get rid of both references once done merging
                    firstPathToMerge = null;
                    secondPathToMerge = null;
                }
            }
        }

        void DrawPathButtonsAndInfo(WaypointPath path)
        {
            //Set the buttons to use the pathColor
            GUI.backgroundColor = path.pathColor;

            //Path name button
            if (GUILayout.Button(path.name, GUILayout.Height(32)))
            {
                Selection.activeGameObject = path.gameObject;
                WaypointPathUtilities.FocusSceneCameraOn(path.gameObject);
            }

            //Draw point amount
            GUILayout.Label(string.Format("Points:{0}", path.Points.Length), GUILayout.Width(80));

            //'Delete this path' button
            if (GUILayout.Button(WaypointEditorAssets.DeleteIcon, GUILayout.Width(32), GUILayout.Height(32)))
            {
                //Destroy the path, and if in edit mode, store it in the undo buffer such
                // that you can undo the destroy action
                Undo.DestroyObjectImmediate(path.gameObject);
                Repaint();
            }
        }

        #endregion
    }
}

#endif