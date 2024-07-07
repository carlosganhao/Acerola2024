using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Ink.Runtime;

public class ChaserController : MonoBehaviour
{
    public GameObject _shotParticleSystem;
    [HideInInspector] public ControlledChaserState ControlledState;
    [HideInInspector] public PatrolingChaserState PatrolingState;
    [HideInInspector] public ChasingChaserState ChasingState;
    [HideInInspector] public CharacterController _characterController;
    [SerializeField] private TextAsset _reactionsAsset;
    private Story _reactions;
    public float _minBanterCooldown = 300;
    public float _maxBanterCooldown = 900;
    [HideInInspector] public Vector3 _soundPosition;
    [HideInInspector] public float _deafenDuration;
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
        _reactions = new Story(_reactionsAsset.text);

        ControlledState = new ControlledChaserState(this);
        PatrolingState = new PatrolingChaserState(this);
        ChasingState = new ChasingChaserState(this);

        EventBroker.DeafenChaser += DeafenChaser;
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

        if(_deafenDuration > 0)
        {
            _deafenDuration -= Time.deltaTime;
        }
    }

    public void OnDestroy()
    {
        _currentState?.ExitState();
    }

    public void SwitchToState(AbstractChaserState newState)
    {
        if(_currentState is not null) _currentState.ExitState();
        _currentState = newState;
        _currentState.EnterState();
    }

    public void PlayReaction(ReactionType type)
    {
        _reactions.ChoosePathString(type.ToString());
        EventBroker.InvokeWriteMessage(_reactions.Continue(), "Character", null);
    }

    private void DeafenChaser(float duration)
    {
        _deafenDuration = duration;
    }
    
    public enum ReactionType {
        reaction_found,
        reaction_searching,
        reaction_heard,
        reaction_banter
    }
}
