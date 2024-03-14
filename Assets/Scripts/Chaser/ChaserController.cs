using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ChaserController : MonoBehaviour
{
    public GameObject _shotParticleSystem;
    [HideInInspector] public ControlledChaserState ControlledState;
    [HideInInspector] public PatrolingChaserState PatrolingState;
    [HideInInspector] public ChasingChaserState ChasingState;
    [HideInInspector] public CharacterController _characterController;
    [HideInInspector] public Vector3 _soundPosition;
    public Animator _animator;
    [HideInInspector] public BaseControls _controls;
    public AnimationCurve _distanceToPlayerOdds;
    public Light _characterLight;
    private AbstractChaserState _currentState = null;
    private AbstractChaserState _previousState;
    public CinemachineImpulseSource _impulseSource;

    void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _impulseSource = GetComponent<CinemachineImpulseSource>();
        
        _controls = new BaseControls();

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
