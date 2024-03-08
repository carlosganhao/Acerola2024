using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PatrolingChaserState : AbstractChaserState
{
    private float _patrolingSpeed = 3;
    private float _approachedDistance = 0.5f;
    private Waypoint _currentTargetWaypoint;
    private Waypoint _previousTargetWaypoint;

    public PatrolingChaserState(ChaserController controller) : base(controller)
    {
        
    }

    public override void EnterState()
    {
        _currentTargetWaypoint = MapController.Instance.GetClosestWaypointToPoint(controller.transform.position);
        Debug.Log(_currentTargetWaypoint);
        LookAtWaypoint();
    }

    public override void UpdateState()
    {
        //Debug.Log(Vector3.Distance(controller.transform.position, _currentTargetWaypoint.Position));
        if(Vector3.Distance(controller.transform.position, _currentTargetWaypoint.Position) <= _approachedDistance)
        {
            //Debug.Log("Checking new Waypoint");
            PickNextPatrolWaypoint();
        }

        //controller.transform.LookAt(_currentTargetWaypoint.Position, Vector3.up);
        controller._characterController.SimpleMove(controller.transform.forward * _patrolingSpeed);
    }

    public override void ExitState()
    {

    }

    private void PickNextPatrolWaypoint()
    {
        var possibleWaypoints = _currentTargetWaypoint.ConnectedWaypoits.Where(w => w is not null && (_previousTargetWaypoint is null || w.Position != _previousTargetWaypoint.Position));
        var value = Random.value;

        foreach(var waypoint in possibleWaypoints)
        {   
            Debug.Log(value);
            Debug.Log(waypoint);
            Debug.Log(waypoint.Position);
            if(value < 1.0f/possibleWaypoints.Count())
            {
                _previousTargetWaypoint = _currentTargetWaypoint;
                _currentTargetWaypoint = waypoint;
                Debug.Log("Chosen");
                LookAtWaypoint();
                return;
            }
            else
            {
                value -= 1.0f/possibleWaypoints.Count();
            }
        }
    }

    private void LookAtWaypoint()
    {
        Vector3 intendedDirection = _currentTargetWaypoint.Position - controller.transform.position;

        float newAngle = Quaternion.LookRotation(intendedDirection, Vector3.up).eulerAngles.y;

        controller.transform.rotation = Quaternion.Euler(0, newAngle, 0);
    }
}
