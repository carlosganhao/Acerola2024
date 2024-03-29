using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject _reticle;
    [SerializeField] private float normalFOV = 60;
    [SerializeField] private float aimFOV = 25;
    [SerializeField] private float _verticalMouseSensitivity = 0.1f;
    [SerializeField] private Vector2 _maxLookAngle = new Vector2(-45.0f, 45.0f);
    [SerializeField] private float _rotationDamp = 0.2f;
    [SerializeField] private float _zoomDamp = 0.2f;
    private BaseControls _controls;
    [SerializeField] private CinemachineVirtualCamera _camera;
    private float _currentRotationAdjust;
    private float _currentRotationVelocity;
    private float _currentRotation;
    private float _currentFOVTarget;
    private float _currentFOVAdjust;
    private float _currentFOVVelocity;
    private bool isControllingChaser = true;

    void Awake()
    {
        _controls = new BaseControls();
        // _camera = GetComponent<CinemachineVirtualCamera>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _controls.PlayerActions.Enable();
        _currentRotation = transform.localEulerAngles.x;

        EventBroker.DetachChaser += DetachChaser;
        EventBroker.AnimationIn += AnimationIn;
        EventBroker.AnimationOut += AnimationOut;
    }

    void OnDestroy()
    {
        EventBroker.DetachChaser -= DetachChaser;
        EventBroker.AnimationIn -= AnimationIn;
        EventBroker.AnimationOut -= AnimationOut;
    }

    // Update is called once per frame
    void Update()
    {   
        var inputRotation = _controls.PlayerActions.LookVertical.ReadValue<float>() * _verticalMouseSensitivity;
        _currentRotationAdjust = Mathf.SmoothDamp(_currentRotationAdjust, inputRotation, ref _currentRotationVelocity, _rotationDamp);
        _currentRotation += _currentRotationAdjust;
        _currentRotation = Mathf.Clamp(_currentRotation, _maxLookAngle.x, _maxLookAngle.y);
        transform.rotation = Quaternion.Euler(_currentRotation, transform.eulerAngles.y, transform.eulerAngles.z);

        if(isControllingChaser && _controls.PlayerActions.Aim.IsPressed())
        {
            _currentFOVTarget = aimFOV;
            _reticle.SetActive(true);
        }
        else
        {
            _currentFOVTarget = normalFOV;
            _reticle.SetActive(false);
        }

        _currentFOVAdjust = Mathf.SmoothDamp(_currentFOVAdjust, _currentFOVTarget, ref _currentFOVVelocity, _zoomDamp);
        _camera.m_Lens.FieldOfView = _currentFOVAdjust;
    }

    private void DetachChaser()
    {
        isControllingChaser = false;
        _rotationDamp = 0;
    }

    private void AnimationIn()
    {
        _controls.PlayerActions.Disable();
    }

    private void AnimationOut()
    {
        _controls.PlayerActions.Enable();
    }
}
