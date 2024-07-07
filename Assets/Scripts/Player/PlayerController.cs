using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;


public class PlayerController : MonoBehaviour
{
    [HideInInspector] public HideProp PropHidingInsideOf;
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private CinemachineVirtualCamera _cinemachineCamera;
    [SerializeField] private Transform _chaserTransform;
    [SerializeField] private LayerMask _hitscanLayers;
    [SerializeField] private GameObject _shotParticleSystem;
    [SerializeField] private int _maxHealth = 3;
    [SerializeField] private float _maxVelocity = 2;
    [SerializeField] private float _maxRunVelocity = 4;
    [SerializeField] private float _aimVelocity = 0.5f;
    [SerializeField] private float _velocityDamp = 0.2f;
    [SerializeField] private float _horizontalMouseSensitivity = 0.1f;
    [SerializeField] private float _rotationDamp = 0.2f;
    [SerializeField] private float _soundRadius = 10;
    [SerializeField] private float _soundCooldown = 0.75f;
    [SerializeField] private AudioClip _stabSound;
    [SerializeField] private AudioClip _cameraCrackSound;
    private int _health;
    private float _soundCooldownTime;
    private BaseControls _controls; 
    private CharacterController _characterController;
    private CinemachineBasicMultiChannelPerlin _cameraNoise;
    private AudioSource _audioSource;
    private float _currentRotationAdjust;
    private float _currentRotationVelocity;
    private float _targetVelocity;
    private float _currentVelocity;
    private float _currentVelocityChange;
    private Vector3 _previousToHidingPosition;
    private bool _isControllingChaser = true;
    public bool IsControllingChaser { get => _isControllingChaser; private set => _isControllingChaser = value; }

    void Awake()
    {
        _controls = new BaseControls();
        _characterController = GetComponent<CharacterController>();
        _cameraNoise = _cinemachineCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _audioSource = GetComponent<AudioSource>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        _controls.PlayerActions.Enable();

        _health = _maxHealth;
        _soundCooldownTime = _soundCooldown;
        _currentVelocity = _maxVelocity; 

        _controls.PlayerActions.Shoot.performed += Shoot;
        _controls.PlayerActions.Interact.performed += Interact;
        EventBroker.DetachChaser += DetachChaser;
        EventBroker.AnimationIn += AnimationIn;
        EventBroker.AnimationOut += AnimationOut;
        EventBroker.HidePlayerForCutscene += HidePlayerForCutscene;
    }

    // Update is called once per frame
    void Update()
    {
        var inputRotation = _controls.PlayerActions.LookHorizontal.ReadValue<float>() * _horizontalMouseSensitivity;
        _currentRotationAdjust = Mathf.SmoothDamp(_currentRotationAdjust, inputRotation, ref _currentRotationVelocity, _rotationDamp);
        transform.Rotate(Vector3.up, _currentRotationAdjust);

        if(_isControllingChaser && _controls.PlayerActions.Aim.IsPressed())
        {
            _targetVelocity = _aimVelocity;
        }
        else if(_controls.PlayerActions.Run.IsPressed())
        {
            _targetVelocity = _maxRunVelocity;

            if(_soundCooldownTime < 0)
            {
                MakeSound();
                _soundCooldownTime = _soundCooldown; 
            }
        }
        else
        {
            _targetVelocity = _maxVelocity;
        }

        _currentVelocity = Mathf.SmoothDamp(_currentVelocity, _targetVelocity, ref _currentVelocityChange, _velocityDamp);

        if(PropHidingInsideOf is null)
        {
            // Debug.Log($"Position: {transform.position}, CurrentVelocity: {_currentVelocity}");

            var inputMovement = _controls.PlayerActions.Movement.ReadValue<Vector2>();
            
            if(inputMovement != Vector2.zero)
            {
                _cameraNoise.m_AmplitudeGain = _targetVelocity / 8.0f;
            }
            else
            {
                _cameraNoise.m_AmplitudeGain = 0;
            }

            var movement = transform.right * inputMovement.x + transform.forward * inputMovement.y;
            _characterController.SimpleMove(_currentVelocity * movement);

            // Debug.Log($"Position: {transform.position}, Movement: {movement}");
        }

        _soundCooldownTime -= Time.deltaTime;
    }

    public void OnDestroy()
    {
        _controls.PlayerActions.Shoot.performed -= Shoot;
        _controls.PlayerActions.Interact.performed -= Interact;
        EventBroker.DetachChaser -= DetachChaser;
        EventBroker.AnimationIn -= AnimationIn;
        EventBroker.AnimationOut -= AnimationOut;
        EventBroker.HidePlayerForCutscene -= HidePlayerForCutscene;
    }

    public void Hide(HideProp hideProp)
    {
        PropHidingInsideOf = hideProp;
        _characterController.enabled = false;
    }

    public void Unhide()
    {
        PropHidingInsideOf = null;
        _characterController.enabled = true;
    }

    public void MakeSound()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, _soundRadius, LayerMask.NameToLayer("Character"));
        if(hits.Length > 0)
        {
            EventBroker.InvokeSoundTriggered(transform.position);
        }
    }

    public void TakeDamage(int damage)
    {
        // Debug.Log("Took Damange");

        _health = Mathf.Max(0, _health - damage);
        EventBroker.InvokePlayerHealthChanged(-damage, _isControllingChaser);
        if(_isControllingChaser)
        {
            _audioSource.PlayOneShot(_stabSound);
        }
        else
        {
            _audioSource.PlayOneShot(_cameraCrackSound);
        }

        if(_health <= 0)
        {
            EventBroker.InvokeGameOver();
        }
    }

    public void Teleport(Vector3 position)
    {
        _characterController.enabled = false;
        transform.position = position;
        _characterController.enabled = true;
    }

    public void Heal(int heal)
    {
        // Debug.Log("Healed");
        _health = Mathf.Min(_maxHealth, _health + heal);
        EventBroker.InvokePlayerHealthChanged(heal, _isControllingChaser);
    }

    private void Shoot(InputAction.CallbackContext context)
    {
        if(!_controls.PlayerActions.Aim.IsPressed()) return;

        if(Physics.Raycast(_cameraController.transform.position + _cameraController.transform.forward * 0.5f, _cameraController.transform.forward, out RaycastHit hit, 100))
        {
            if(hit.collider.gameObject.layer == 7)
            {
                hit.collider.GetComponentInParent<IDamageable>().TakeDamage(gameObject, DamageAmount(hit));
            }
            else
            {
                Instantiate(_shotParticleSystem, hit.point, _cameraController.transform.rotation);
            }
        }

        int DamageAmount(RaycastHit hit) => hit.collider.name switch 
        {
            "HeadCollider" => 5,
            "ChestCollider" => 3,
            "LegCollider" => 2,
            _ => 1,
        };
    }

    private void Interact(InputAction.CallbackContext context)
    {
        if(PropHidingInsideOf is not null)
        {
            PropHidingInsideOf.Interact(this);
            return;
        }

        if(Physics.Raycast(_cameraController.transform.position, _cameraController.transform.forward, out RaycastHit hit, 3))
        {
            // Debug.Log(hit.collider.gameObject);
            if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Interactable"))
            {
                hit.collider.GetComponent<IInteractable>().Interact(this);
            }
        }
    }

    private void DetachChaser()
    {
        _isControllingChaser = false;
        _cameraController.transform.localPosition = new Vector3(0, 0.5f, 0);
        _rotationDamp = 0.0f;
        var thirdPersonFollow = _cinemachineCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        thirdPersonFollow.CameraDistance = 0;
        thirdPersonFollow.CameraCollisionFilter = 0;
    }

    private void AnimationIn()
    {
        _controls.PlayerActions.Disable();
    }

    private void AnimationOut()
    {
        _controls.PlayerActions.Enable();
    }

    private void HidePlayerForCutscene(Vector3 position, HideProp prop)
    {
        _characterController.enabled = false;
        transform.position = position;
        transform.rotation = prop.transform.rotation;
        prop.Interact(this);
    }

    void OnDrawGizmos()
    {
        // Gizmos.color = Color.red;
        // Gizmos.DrawRay(_cameraController.transform.position, _cameraController.transform.forward * 3);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _soundRadius);
    }
}
