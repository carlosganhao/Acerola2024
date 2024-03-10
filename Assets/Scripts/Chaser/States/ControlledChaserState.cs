using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlledChaserState : AbstractChaserState
{
    public ControlledChaserState(ChaserController controller) : base(controller)
    {
        
    }

    public override void EnterState(){
        EventBroker.DetachChaser += DetachListener;
        controller._controls.PlayerActions.Enable();
        controller._controls.PlayerActions.Shoot.performed += Shoot;
    }

    public override void UpdateState(){
        controller._animator.SetBool("Aiming", controller._controls.PlayerActions.Aim.IsPressed());
        controller._animator.SetBool("Moving", controller._controls.PlayerActions.Movement.ReadValue<Vector2>() != Vector2.zero);
    }

    public override void ExitState(){
        EventBroker.DetachChaser -= DetachListener;
        controller._controls.PlayerActions.Disable();
        controller._controls.PlayerActions.Shoot.performed -= Shoot;
    }

    private void DetachListener(){
        Debug.Log("Detaching Chaser");
        controller.transform.SetParent(null);
        controller.SwitchToState(controller.PatrolingState);
    }

    private void Shoot(InputAction.CallbackContext context)
    {
        if(!controller._controls.PlayerActions.Aim.IsPressed()) return;

        controller._animator.SetTrigger("Shoot");
    }
}
