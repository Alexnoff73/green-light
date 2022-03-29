using UnityEngine;
namespace WaypointEditor
{

    /// <summary>
    /// A collection of points representing a movement path. Any child of a path is considered a point.
    /// </summary>
    [ExecuteInEditMode]
    public class WaypointPath : MonoBehaviour
    {

        public enum LineType
        {
            Solid,
            Dotted
        }
        public LineType lineType;
        /// <summary>
        /// How far apart the lines are if <see cref='lineType'/> is of type <see cref='LineType.Dotted'/>.
        /// </summary>
        public float lineInterval = 5f;

        public Color pathColor = Color.green;
        public bool loopsAround;

        public Transform[] Points;

#if UNITY_EDITOR
//Almost all methods have to be editor only, in game you only need to have the point locations.
        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake()
        {
            UpdatePathPoints();
        }

        /// <summary>
        /// These must be ran independent of the constructor due to gameobject.transform 
        /// not being accessible until Start() or Awake() is called. Since we are in the editor,
        /// they are not called.
        /// </summary>
        public void Initialize()
        {
            pathColor = Utilities.GetRandomBrightColor();

            CreateNewPathPoint(transform.position + new Vector3(0, 0, -1));
            CreateNewPathPoint(transform.position + new Vector3(0, 0, 1));
        }
        /// <summary>
        /// Adds a new point to the end of the path.
        /// </summary>
        /// <param name="at">The position to place the new point at.</param>
        public void CreateNewPathPoint(Vector3 at)
        {
            var newPoint = new GameObject("Point " + Points.Length);
            newPoint.transform.position = at;
            newPoint.transform.parent = this.transform;
            UnityEditor.Undo.RegisterCreatedObjectUndo(newPoint, string.Format("Added point {0}", newPoint.name));
        }

        /// <summary>
        /// Brings the Y position of each path point to the same Y position as the path.
        /// </summary>
        public void Flatten()
        {
            //Capture previous states of all points before flatten
            UnityEditor.Undo.RecordObjects(Points, "Flatten Path");

            for (int i = 0; i < Points.Length; i++)
            {
                var newPos = Points[i].transform.position;
                newPos.y = transform.position.y;

                Points[i].transform.position = newPos; //reassign
            }

        }
        /// <summary>
        /// Renames all points so the point's name corresponds to it's order as a child of the path.
        /// </summary>
        public void RenamePointsAccordingToOrder()
        {
            //Capture previous states of all points before RenamePointsAccordingToOrder
            UnityEditor.Undo.RegisterFullObjectHierarchyUndo(gameObject, "Rename Points");

            for (int i = 0; i < Points.Length; i++)
            {
                Points[i].name = string.Format("Point {0}", i);
            }
        }

        void DrawLineBetweenPoints(int pointIndexA, int pointIndexB)
        {
            UnityEditor.Handles.color = pathColor;

            switch (lineType)
            {
                case WaypointPath.LineType.Dotted:
                    UnityEditor.Handles.DrawDottedLine(Points[pointIndexA].position, Points[pointIndexB].position, lineInterval);
                    break;
                case WaypointPath.LineType.Solid:
                    UnityEditor.Handles.DrawLine(Points[pointIndexA].position, Points[pointIndexB].position);
                    break;
            }
        }

        /// <summary>
        /// Finds all children with transforms and assigns them as points to the path.
        /// </summary>
        public void UpdatePathPoints()
        {
            /*Why not use GetComponentsInChildren<Transform>() to get all children transforms? 
                          Because that would also return the parent, in this case, this WaypointPath.
                          We only want children here.
                        */
            int children = transform.childCount;
            Points = new Transform[children];
            for (int i = 0; i < children; i++)
            {
                Points[i] = transform.GetChild(i);
            }
        }

        /// <summary>
        /// Callback to draw gizmos that are pickable and always drawn.
        /// </summary>
        void OnDrawGizmos()
        {
            if (Points == null)
                return;

            Gizmos.color = pathColor;

            int pointAmo = Points.Length;

            for (int i = 0; i < pointAmo; i++)
            {
                var element = Points[i];
                if (element == null)
                    continue;

                //Draw pin
                Gizmos.DrawMesh(WaypointEditorAssets.PinMesh, element.position, element.rotation, Vector3.one);
                //Draw line between point
                if (i + 1 < pointAmo)
                    DrawLineBetweenPoints(i, i + 1);

                //Draw loop around line
                if (loopsAround &&
                    pointAmo >= 2)
                    DrawLineBetweenPoints(0, Points.Length - 1);
            }
        }

        ///Points are updated constantly in the editor or when running the game in the editor.
        ///This way you can make add/delete points and the path will adjust accordingly. For
        /// performance reasons,This will not run on every Update in a release build, so if
        /// you are altering paths in-game, make sure to call <see cref='UpdatePathPoints'/> again!.
        void Update()
        {
            UpdatePathPoints();
        }
#endif

        /// <summary>
        /// Returns the point with the shortest distance between it and <see cref="from"/>.
        /// </summary>
        /// <param name="to">The point to compare distances with</param>
        /// <returns>The index of the closest point. -1 if none found.</returns>
        public int GetClosestPointTo(Vector3 to)
        {
            int pointIndex = -1;

            //Start at infinity, that way no matter what we compare to,
            //the distance will be smaller and thus it will be set as the current closest one.
            float currentDistance = Mathf.Infinity;

            for (int i = 0; i < Points.Length; i++)
            {
                float d = Vector3.Distance(to, Points[i].position);
                if (d < currentDistance)
                {
                    currentDistance = d;
                    pointIndex = i;
                }
            }

            return pointIndex;
        }

    }
}