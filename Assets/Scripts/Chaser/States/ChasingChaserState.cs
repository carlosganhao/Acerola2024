using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasingChaserState : AbstractChaserState
{
    private float _shotCooldown = 1.0f;
    private float _shotTimeElapsed;
    private float _accuracy = 4;
    private float _chasingSpeed = 5;
    private Vector3 _playersPosition;
    private float _approachedDistance = 0.5f;
    private Stack<Waypoint> _pathToPlayer;
    private Waypoint _previousTargetWaypoint;
    private Waypoint _targetWaypoint;
    private bool _sawPlayer = false;

    public ChasingChaserState(ChaserController controller) : base(controller)
    {

    }

    public override void EnterState()
    {
        EventBroker.SoundTriggered += SoundListener;

        if(controller._soundPosition != Vector3.zero)
        {
            GetPathTo(controller._soundPosition);
            GetNextTargetWaypoint();
            controller._soundPosition = Vector3.zero;

            controller._animator.SetBool("Moving", true);
            controller._animator.SetBool("Running", true);
            controller._animator.SetBool("Aiming", false);
        }
        else
        {
            _sawPlayer = true;
            controller.PlayReaction(ChaserController.ReactionType.reaction_found);
            controller._animator.SetBool("Moving", false);
            controller._animator.SetBool("Running", false);
            controller._animator.SetBool("Aiming", true);
        }
        // Debug.Log("Found Player!");
    }

    public override void UpdateState()
    {
        if(MapController.Instance.IsPlayerVisible(controller.transform.position, controller.transform.forward))
        {
            if(!_sawPlayer)
            {
                _sawPlayer = true;
                controller.PlayReaction(ChaserController.ReactionType.reaction_found);
            }
            controller._animator.SetBool("Moving", false);
            controller._animator.SetBool("Running", false);
            controller._animator.SetBool("Aiming", true);

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
            controller._animator.SetBool("Moving", true);
            controller._animator.SetBool("Running", true);
            controller._animator.SetBool("Aiming", false);

            if(_pathToPlayer is null)
            {
                GetPathTo(_playersPosition);
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
        EventBroker.SoundTriggered -= SoundListener;
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

        if(Physics.Raycast(controller.transform.position, shotDirection, out RaycastHit hit, 100, LayerMask.GetMask("Player", "Terrain")))
        {
            if(hit.collider.tag == "Player")
            {
                hit.collider.GetComponent<PlayerController>().TakeDamage(1);
            }
            else
            {
                GameObject.Instantiate(controller._shotParticleSystem, hit.point, controller.transform.rotation);
            }
        }
    }

    private void GetPathTo(Vector3 position)
    {
        var lastWaypoint = MapController.Instance.GetClosestWaypointToPoint(position);
        var firstWaypoint = MapController.Instance.GetClosestWaypointToPointFromOther(position, controller.transform.position);

        _pathToPlayer = MapController.Instance.GetPathToDestination(lastWaypoint, firstWaypoint);
    }

    private void GetNextTargetWaypoint()
    {
        if(_pathToPlayer.Count > 0)
        {
            _previousTargetWaypoint = _targetWaypoint;
            _targetWaypoint = _pathToPlayer.Pop();
            LookAtWaypoint(); 
            UseAction(_previousTargetWaypoint, _previousTargetWaypoint.CheckDirectionToWaypoint(_targetWaypoint));
        }
        else
        {
            controller.PlayReaction(ChaserController.ReactionType.reaction_searching);
            controller.SwitchToState(controller.PatrolingState);
        }
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
    
    private void SoundListener(Vector3 position)
    {
        if(controller._deafenDuration > 0) return;

        GetPathTo(controller._soundPosition);
        GetNextTargetWaypoint();
        controller._soundPosition = Vector3.zero;
    }
}
