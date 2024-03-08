using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlledChaserState : AbstractChaserState
{
    public ControlledChaserState(ChaserController controller) : base(controller)
    {
        
    }

    public override void EnterState(){
        EventBroker.DetachChaser += DetachListener;
    }

    public override void UpdateState(){
        
    }

    public override void ExitState(){
        EventBroker.DetachChaser -= DetachListener;
    }

    private void DetachListener(){
        Debug.Log("Detaching Chaser");
        controller.transform.SetParent(null);
        controller.SwitchToState(controller.PatrolingState);
    }
}
