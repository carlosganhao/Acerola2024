using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractChaserState
{   
    protected ChaserController controller;

    protected AbstractChaserState(ChaserController controller)
    {
        this.controller = controller;
    }

    public void EnterState(ChaserController controller)
    {
        this.controller = controller;
        EnterState();
    }
    
    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
}
