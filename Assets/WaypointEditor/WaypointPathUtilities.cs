#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WaypointEditor
{
    /// <summary>
    /// Static class allowing useful methods for altering WaypointPaths.
    /// </summary>
    public static class WaypointPathUtilities
    {
        static void DestroyAllPointsInPath(WaypointPath path)
        {
            Undo.RecordObjects(path.Points, "Destroy all path points of:" + path.name);
            for (int i = 0; i < path.Points.Length; i++)
            {
                Utilities.SafeDestroy(path.Points[i].gameObject);
            }
        }

        /// <summary>
        /// Merges path <paramref name="A"> into path <paramref name="B">, and then destroys <paramref name="A">
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns>Returns if the operation was successful</returns>
        public static bool MergePaths(WaypointPath A, WaypointPath B)
        {
            bool isSuccessful = false;

            if (!A)
                Debug.LogError("Cannot complete merge function as path A is not set.");
            else if (!B)
                Debug.LogError("Cannot complete merge function as path B is not set.");
            else
            {
                //Create group so you can undo the entire merge process in one step
                Undo.SetCurrentGroupName(string.Format("Merge {0} into {1}", A.name, B.name));
                int group = Undo.GetCurrentGroup();

                for (int i = 0; i < A.Points.Length; i++)
                {
                    Undo.SetTransformParent(A.Points[i].transform, B.transform,
                        string.Format("Move point {0} under {1}", A.Points[i].name, B.name));
                    A.Points[i].SetAsLastSibling();
                }

                //Delete path A
                Utilities.SafeDestroyWithUndo(A.gameObject);

                Undo.CollapseUndoOperations(group);

                isSuccessful = true;
            }

            return isSuccessful;
        }

        public static void FocusSceneCameraOn(GameObject item)
        {
            EditorGUIUtility.PingObject(item);
            //Stores what we currently have active
            var currentlyActive = Selection.activeGameObject;

            if (currentlyActive
                //If the scene view is open
                &&
                SceneView.lastActiveSceneView != null)
            {
                //Set to focus on 'item'
                Selection.activeGameObject = item.gameObject;
                SceneView.lastActiveSceneView.FrameSelected();
                //Then reselect what was previously selected
                Selection.activeGameObject = currentlyActive;
            }
        }

    }
}
#endif