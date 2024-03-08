using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaserController : MonoBehaviour
{
    [HideInInspector] public ControlledChaserState ControlledState;
    [HideInInspector] public PatrolingChaserState PatrolingState;
    [HideInInspector] public ChasingChaserState ChasingState;
    [HideInInspector] public CharacterController _characterController;
    private AbstractChaserState _currentState = null;
    private AbstractChaserState _previousState;

    void Awake()
    {
        _characterController = GetComponent<CharacterController>();

        ControlledState = new ControlledChaserState(this);
        PatrolingState = new PatrolingChaserState(this);
        ChasingState = new ChasingChaserState(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        SwitchToState(ControlledState);
    }

    // Update is called once per frame
    void Update()
    {
        _currentState.UpdateState();
    }

    public void SwitchToState(AbstractChaserState newState)
    {
        if(_currentState is not null) _currentState.ExitState();
        _currentState = newState;
        _currentState.EnterState();
    }
}
