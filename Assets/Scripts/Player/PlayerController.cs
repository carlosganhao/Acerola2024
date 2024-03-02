using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private GameObject _placeholder;
    [SerializeField] private LayerMask _hitscanLayers;
    [SerializeField] private int _maxHealth = 5;
    [SerializeField] private float _maxVelocity = 2;
    [SerializeField] private float _horizontalMouseSensitivity = 0.1f;
    [SerializeField] private float _rotationDamp = 0.2f;
    private BaseControls _controls; 
    private CharacterController _characterController;
    private float _currentRotationAdjust;
    private float _currentRotationVelocity;

    void Awake()
    {
        _controls = new BaseControls();
        _characterController = GetComponent<CharacterController>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        _controls.PlayerActions.Enable();

        _controls.PlayerActions.Shoot.performed += Shoot;
    }

    // Update is called once per frame
    void Update()
    {
        var inputRotation = _controls.PlayerActions.LookHorizontal.ReadValue<float>() * _horizontalMouseSensitivity;
        _currentRotationAdjust = Mathf.SmoothDamp(_currentRotationAdjust, inputRotation, ref _currentRotationVelocity, _rotationDamp);
        transform.Rotate(Vector3.up, _currentRotationAdjust);
    }

    void FixedUpdate()
    {
        var inputMovement = _controls.PlayerActions.Movement.ReadValue<Vector2>();
        var movement = transform.right * inputMovement.x + transform.forward * inputMovement.y;
        _characterController.SimpleMove(_maxVelocity * movement);
    }

    private void Shoot(InputAction.CallbackContext context)
    {
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
}
