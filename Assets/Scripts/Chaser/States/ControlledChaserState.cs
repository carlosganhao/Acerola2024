using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlledChaserState : AbstractChaserState
{
    private bool inAnimation = false;

    public ControlledChaserState(ChaserController controller) : base(controller)
    {
        
    }

    public override void EnterState(){
        EventBroker.DetachChaser += DetachListener;
        controller._controls.PlayerActions.Enable();
        controller._controls.PlayerActions.Shoot.performed += Shoot;
        EventBroker.AnimationIn += AnimationIn;
        EventBroker.AnimationOut += AnimationOut;
        EventBroker.CharacterLookAt += LookAtCamera;
        EventBroker.CharacterAim += AimGun;
        EventBroker.CharacterShoot += ShootGun;
    }

    public override void UpdateState(){
        if(!inAnimation)
        {
            controller._animator.SetBool("Aiming", controller._controls.PlayerActions.Aim.IsPressed());
            controller._animator.SetBool("Moving", controller._controls.PlayerActions.Movement.ReadValue<Vector2>() != Vector2.zero);
            controller._animator.SetBool("Running", controller._controls.PlayerActions.Run.IsPressed());
        }
    }

    public override void ExitState(){
        EventBroker.DetachChaser -= DetachListener;
        controller._controls.PlayerActions.Disable();
        controller._controls.PlayerActions.Shoot.performed -= Shoot;
        EventBroker.AnimationIn -= AnimationIn;
        EventBroker.AnimationOut -= AnimationOut;
        EventBroker.CharacterLookAt -= LookAtCamera;
        EventBroker.CharacterAim -= AimGun;
        EventBroker.CharacterShoot -= ShootGun;
    }

    private void LookAtCamera(Vector3 posisition)
    {
        Vector3 intendedDirection = posisition - controller.transform.position;

        float newAngle = Quaternion.LookRotation(intendedDirection, Vector3.up).eulerAngles.y;

        controller.transform.rotation = Quaternion.Euler(0, newAngle, 0);
    }

    private void AimGun() => controller._animator.SetBool("Aiming", true);

    private void ShootGun() => controller._animator.SetTrigger("Shoot");
    

    private void DetachListener(){
        // Debug.Log("Detaching Chaser");
        controller.transform.SetParent(null);
        controller.SwitchToState(controller.PatrolingState);
    }
    
    private void AnimationIn()
    {
        inAnimation = true;
        controller._controls.PlayerActions.Disable();
    }

    private void AnimationOut()
    {
        inAnimation = false;
        controller._controls.PlayerActions.Enable();
    }

    private void Shoot(InputAction.CallbackContext context)
    {
        if(!controller._controls.PlayerActions.Aim.IsPressed()) return;

        controller._animator.SetTrigger("Shoot");
    }
}
