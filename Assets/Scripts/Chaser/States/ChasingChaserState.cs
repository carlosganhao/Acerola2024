using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasingChaserState : AbstractChaserState
{
    private float _shotCooldown = 1.0f;
    private float _shotTimeElapsed;
    private float _accuracy = 10;
    private float _chasingSpeed = 3;
    private Vector3 _playersPosition;
    private float _approachedDistance = 0.5f;
    private Stack<Waypoint> _pathToPlayer;
    private Waypoint _targetWaypoint;

    public ChasingChaserState(ChaserController controller) : base(controller)
    {

    }

    public override void EnterState()
    {
        controller._animator.SetBool("Moving", false);
        controller._animator.SetBool("Aiming", true);
        Debug.Log("Found Player!");
    }

    public override void UpdateState()
    {
        if(MapController.Instance.IsPlayerVisible(controller.transform.position))
        {
            _pathToPlayer = null;
            _playersPosition = MapController.Instance.PlayerPosition;
            LookAtPlayer();

            if(_shotTimeElapsed <= 0)
            {
                Shoot();
                _shotTimeElapsed = _shotCooldown;
            }
            else
            {
                _shotTimeElapsed -= Time.deltaTime;
            }
        }
        else
        {
            if(_pathToPlayer is null)
            {
                GetPathToPlayer();
                GetNextTargetWaypoint();
            }
            else
            {
                if(Vector3.Distance(controller.transform.position, _targetWaypoint.Position) <= _approachedDistance)
                {
                    GetNextTargetWaypoint();
                }

                controller._characterController.SimpleMove(controller.transform.forward * _chasingSpeed);
            }
        }
    }

    public override void ExitState()
    {

    }

    private void LookAtPlayer()
    {
        Vector3 intendedDirection = _playersPosition - controller.transform.position;

        float newAngle = Quaternion.LookRotation(intendedDirection, Vector3.up).eulerAngles.y;

        controller.transform.rotation = Quaternion.Euler(0, newAngle, 0);
    }

    private void LookAtWaypoint()
    {
        Vector3 intendedDirection = _targetWaypoint.Position - controller.transform.position;

        float newAngle = Quaternion.LookRotation(intendedDirection, Vector3.up).eulerAngles.y;

        controller.transform.rotation = Quaternion.Euler(0, newAngle, 0);
    }

    private void Shoot()
    {
        controller._animator.SetTrigger("Shoot");

        Vector3 playerDirection = _playersPosition - controller.transform.position;

        Vector3 shotDirection = Quaternion.AngleAxis(Random.Range(-_accuracy, _accuracy), Vector3.up) * (Quaternion.AngleAxis(Random.Range(-_accuracy, _accuracy), controller.transform.right) * playerDirection);

        if(Physics.Raycast(controller.transform.position, shotDirection, out RaycastHit hit, 100))
        {
            if(hit.collider.tag == "Player")
            {
                hit.collider.GetComponent<PlayerController>().TakeDamage(1);
            }
            else
            {
                // GameObject.Instantiate(controller._placeholder, hit.point, Quaternion.identity);
            }
        }
    }

    private void GetPathToPlayer()
    {
        var lastWaypoint = MapController.Instance.GetClosestWaypointToPoint(_playersPosition);
        var firstWaypoint = MapController.Instance.GetClosestWaypointToPointFromOther(_playersPosition, controller.transform.position);

        _pathToPlayer = MapController.Instance.GetPathToDestination(lastWaypoint, firstWaypoint);
    }

    private void GetNextTargetWaypoint()
    {
        if(_pathToPlayer.Count > 0)
        {
            _targetWaypoint = _pathToPlayer.Pop();
            LookAtWaypoint(); 
        }
        else
        {
            controller.SwitchToState(controller.PatrolingState);
        }
    }
}
