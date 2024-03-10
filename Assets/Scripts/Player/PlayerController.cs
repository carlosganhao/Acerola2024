using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    [HideInInspector] public HideProp PropHidingInsideOf;
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private Transform _chaserTransform;
    [SerializeField] private LayerMask _hitscanLayers;
    [SerializeField] private int _maxHealth = 5;
    [SerializeField] private float _maxVelocity = 2;
    [SerializeField] private float _aimVelocity = 0.5f;
    [SerializeField] private float _velocityDamp = 0.2f;
    [SerializeField] private float _horizontalMouseSensitivity = 0.1f;
    [SerializeField] private float _rotationDamp = 0.2f;
    private BaseControls _controls; 
    private CharacterController _characterController;
    private float _currentRotationAdjust;
    private float _currentRotationVelocity;
    private float _targetVelocity;
    private float _currentVelocity;
    private float _currentVelocityChange;
    private Vector3 previousToHidingPosition;
    private bool isControllingChaser = true;

    void Awake()
    {
        _controls = new BaseControls();
        _characterController = GetComponent<CharacterController>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        _controls.PlayerActions.Enable();

        _currentVelocity = _maxVelocity;

        _controls.PlayerActions.Shoot.performed += Shoot;
        _controls.PlayerActions.Interact.performed += Interact;
        EventBroker.DetachChaser += DetachChaser;
    }

    // Update is called once per frame
    void Update()
    {
        var inputRotation = _controls.PlayerActions.LookHorizontal.ReadValue<float>() * _horizontalMouseSensitivity;
        _currentRotationAdjust = Mathf.SmoothDamp(_currentRotationAdjust, inputRotation, ref _currentRotationVelocity, _rotationDamp);
        transform.Rotate(Vector3.up, _currentRotationAdjust);

        if(isControllingChaser && _controls.PlayerActions.Aim.IsPressed())
        {
            _targetVelocity = _aimVelocity;
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
            var movement = transform.right * inputMovement.x + transform.forward * inputMovement.y;
            _characterController.SimpleMove(_currentVelocity * movement);

            // Debug.Log($"Position: {transform.position}, Movement: {movement}");
        }
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

    public void TakeDamage(int damage)
    {
        Debug.Log("Took Damange");
        EventBroker.InvokePlayerHealthChanged(-damage);
    }

    private void Shoot(InputAction.CallbackContext context)
    {
        if(!_controls.PlayerActions.Aim.IsPressed()) return;

        if(Physics.Raycast(_cameraController.transform.position, _cameraController.transform.forward, out RaycastHit hit, 100, _hitscanLayers))
        {
            hit.collider.GetComponentInParent<IDamageable>().TakeDamage(gameObject, DamageAmount(hit));
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

        if(Physics.Raycast(_cameraController.transform.position, _cameraController.transform.forward, out RaycastHit hit, 3, LayerMask.NameToLayer("Interactable")))
        {
            Debug.Log(hit.collider.gameObject);
            hit.collider.GetComponentInParent<IInteractable>().Interact(this);
        }
    }

    private void DetachChaser()
    {
        isControllingChaser = false;
        _cameraController.transform.localPosition = new Vector3(0, 0.8f, 0);
        _rotationDamp = 0.0f;
    }

    void OnDrawGizmos()
    {
        // Gizmos.color = Color.red;
        // Gizmos.DrawRay(_cameraController.transform.position, _cameraController.transform.forward * 3);
    }
}
