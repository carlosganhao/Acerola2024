using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float normalFOV = 60;
    [SerializeField] private float aimFOV = 25;
    [SerializeField] private float _verticalMouseSensitivity = 0.1f;
    [SerializeField] private Vector2 _maxLookAngle = new Vector2(-45.0f, 45.0f);
    [SerializeField] private float _rotationDamp = 0.2f;
    [SerializeField] private float _zoomDamp = 0.2f;
    private BaseControls _controls;
    private Camera _camera;
    private float _currentRotationAdjust;
    private float _currentRotationVelocity;
    private float _currentRotation;
    private float _currentFOVTarget;
    private float _currentFOVAdjust;
    private float _currentFOVVelocity;

    void Awake()
    {
        _controls = new BaseControls();
        _camera = GetComponent<Camera>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _controls.PlayerActions.Enable();
        _currentRotation = transform.localEulerAngles.x;
    }

    // Update is called once per frame
    void Update()
    {   
        var inputRotation = _controls.PlayerActions.LookVertical.ReadValue<float>() * _verticalMouseSensitivity;
        _currentRotationAdjust = Mathf.SmoothDamp(_currentRotationAdjust, inputRotation, ref _currentRotationVelocity, _rotationDamp);
        _currentRotation += _currentRotationAdjust;
        _currentRotation = Mathf.Clamp(_currentRotation, _maxLookAngle.x, _maxLookAngle.y);
        transform.rotation = Quaternion.Euler(_currentRotation, transform.eulerAngles.y, transform.eulerAngles.z);

        if(_controls.PlayerActions.Aim.IsPressed())
        {
            _currentFOVTarget = aimFOV;
        }
        else
        {
            _currentFOVTarget = normalFOV;
        }

        _currentFOVAdjust = Mathf.SmoothDamp(_currentFOVAdjust, _currentFOVTarget, ref _currentFOVVelocity, _zoomDamp);
        _camera.fieldOfView = _currentFOVAdjust;
    }
}
