using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class PatrolingChaserState : AbstractChaserState
{
    private float _patrolingSpeed = 3;
    private float _maxPlayerDistance = 200;
    private float _approachedDistance = 0.5f;
    private float _scanInitialCooldown = 10.0f;
    private float _scanCooldown = 1.0f;
    private float _scanTimeElapsed = 0f;
    private Waypoint _currentTargetWaypoint;
    private Waypoint _previousTargetWaypoint;

    public PatrolingChaserState(ChaserController controller) : base(controller)
    {
        
    }

    public override void EnterState()
    {
        EventBroker.SoundTriggered += SoundListener;
        
        _currentTargetWaypoint = MapController.Instance.GetClosestWaypointToPoint(controller.transform.position);
        // Debug.Log(_currentTargetWaypoint);
        controller._animator.SetBool("Moving", true);
        controller._animator.SetBool("Running", false);
        controller._animator.SetBool("Aiming", false);
        controller._characterLight.enabled = true;
        _scanTimeElapsed = _scanInitialCooldown;
        LookAtWaypoint();
    }

    public override void UpdateState()
    {
        // Debug.Log(Vector3.Distance(controller.transform.position, _currentTargetWaypoint.Position));
        if(Vector3.Distance(controller.transform.position, _currentTargetWaypoint.Position) <= _approachedDistance)
        {
            // Debug.Log("Checking new Waypoint");
            PickNextPatrolWaypoint();
            // Debug.Log(_currentTargetWaypoint);
        }

        if(_scanTimeElapsed <= 0)
        {
            // Debug.Log("Scanning...");
            ScanForPlayer();
            _scanTimeElapsed = _scanCooldown;
        }
        else
        {
            _scanTimeElapsed -= Time.deltaTime;
        }

        //controller.transform.LookAt(_currentTargetWaypoint.Position, Vector3.up);
        controller._characterController.SimpleMove(controller.transform.forward * _patrolingSpeed);
    }

    public override void ExitState()
    {

    }

    private void PickNextPatrolWaypoint()
    {
        var possibleWaypoints = _currentTargetWaypoint.ConnectedWaypoits.Where(w => w is not null 
                                    && (_previousTargetWaypoint is null || w.Position != _previousTargetWaypoint.Position));
        
        // Debug.Log(possibleWaypoints.ToArray());

        if (!possibleWaypoints.Any())
        {
            var w = _currentTargetWaypoint;
            _currentTargetWaypoint = _previousTargetWaypoint;
            _previousTargetWaypoint = w;
        }

        Dictionary<Waypoint, int> oddsTable = possibleWaypoints.ToDictionary(w => w, w => WaypointToOdds(w));
        int maxOdds = oddsTable.Values.Sum();

        var value = UnityEngine.Random.Range(1, maxOdds);
        // Debug.Log($"Total Odds = {value}");

        foreach(var waypointOdd in oddsTable)
        {   
            // Debug.Log($"Left Odds = {value}, Waypoint = {waypointOdd.Key}, Odds = {waypointOdd.Value}");
            if(value <= waypointOdd.Value)
            {
                _previousTargetWaypoint = _currentTargetWaypoint;
                _currentTargetWaypoint = waypointOdd.Key;
                UseAction(_previousTargetWaypoint, _previousTargetWaypoint.CheckDirectionToWaypoint(_currentTargetWaypoint));
                LookAtWaypoint();
                return;
            }
            else
            {
                value -= waypointOdd.Value;
            }
        }
    }

    private void LookAtWaypoint()
    {
        Vector3 intendedDirection = _currentTargetWaypoint.Position - controller.transform.position;

        float newAngle = Quaternion.LookRotation(intendedDirection, Vector3.up).eulerAngles.y;

        controller.transform.rotation = Quaternion.Euler(0, newAngle, 0);
    }

    private void UseAction(Waypoint waypoint, int direction)
    {
        // Debug.Log($"Direction: {direction}");
        if(direction >= 0 && direction < 4)
        {
            // Debug.Log($"Trying to use action {direction} of {waypoint}");
            waypoint.InteractEvents[direction].Invoke();
        }
    }

    private void ScanForPlayer()
    {
        if(MapController.Instance.IsPlayerVisible(controller.transform.position, controller.transform.forward))
        {
            controller.SwitchToState(controller.ChasingState);
        }
    }

    private int WaypointToOdds(Waypoint waypoint)
    {
        float distanceToPlayer = MapController.Instance.GetDistanceToPlayer(waypoint);
        // Debug.Log($"Distance to Player = {distanceToPlayer}");
        int oddsByDistance = (int)controller._distanceToPlayerOdds.Evaluate(distanceToPlayer);
        // Debug.Log($"Odds Generated = {oddsByDistance}");
        return oddsByDistance;
    }

    private void SoundListener(Vector3 position)
    {
        if(controller._deafenDuration > 0) return;

        controller._soundPosition = position;
        controller.SwitchToState(controller.ChasingState);
    }
}
