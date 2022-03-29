using UnityEngine;
using UnityEngine.AI;

namespace WaypointEditor
{
	/// <summary>
	/// Allows simple integration of a navmesh agent into a <see cref='WaypointPath'/>
	/// </summary>
    public class NavmeshPathFollower : MonoBehaviour
    {
        public WaypointPath startingPath;
        WaypointPath currentPath;

        public enum MoveState
        {
            None,
            AdvanceForward,
            AdvanceBackward
        }
        public MoveState moveState;

        int currentPoint;
        int destinationPoint;

        NavMeshAgent agent;

        // Use this for initialization
        void Start()
        {
            agent = GetComponent<NavMeshAgent>();

            if (startingPath)
                SetPath(startingPath);
        }

        /// <summary>
        /// Sets the path of a navmesh agent, along with setting it's destination to the nearest point on the path.
        /// </summary>
        /// <param name="newPath"></param>
        public void SetPath(WaypointPath newPath)
        {
            currentPath = newPath;
            int startingPoint = currentPath.GetClosestPointTo(transform.position);
            SetDestinationToPathPointOfIndex(startingPoint);
        }


        public void SetMoveState(MoveState newState)
        {
            moveState = newState;
        }

        void SetDestinationToPathPointOfIndex(int indexOfPathPoint)
        {
            destinationPoint = indexOfPathPoint;
            agent.SetDestination(currentPath.Points[destinationPoint].position);
        }

        int GetNextPointToGoTo()
        {
            int point = destinationPoint;

            switch (moveState)
            {
                case MoveState.AdvanceForward:
                    point = currentPoint + 1;

                    if (point >= currentPath.Points.Length) //If cannot go forward any farther
                    {
                        if (currentPath.loopsAround)
                            point = 0;
                        //If path does not loop, then go back around
                        else
                        {
                            point = currentPoint - 1;
                            moveState = MoveState.AdvanceBackward;
                        }
                    }
                    break;

                case MoveState.AdvanceBackward:
                    point = currentPoint - 1;

                    if (point < 0)   //If cannot go back any farther
                    {
                        if (currentPath.loopsAround)
                            point = currentPath.Points.Length - 1;
                        //If path does not loop, then go back around
                        else
                        {
                            point = currentPoint + 1;
                            moveState = MoveState.AdvanceForward;
                        }
                    }
                    break;

            }
            return point;
        }

        void LateUpdate()
        {
            if (currentPath == null)
                return;

            //If at destination.
            if (agent.remainingDistance < .025f)
            {
                //We are at the destination point
                currentPoint = destinationPoint;
                SetDestinationToPathPointOfIndex(GetNextPointToGoTo());
            }
        }
    }
}
