using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Wheels")]
    [SerializeField] private List<WheelCollider> acceleratingWheels;
    [SerializeField] private List<WheelCollider> swivelWheels;
    [SerializeField] private List<WheelCollider> breakWheels;
    [SerializeField] private List<WheelControl> wheels;

    [Header("Parameters")]
    [SerializeField] private float motorForce;
    [SerializeField] private float breakForce;
    [SerializeField, Min(0)] private float maxSteerAngle = 45f;

    private float _horizontalInput;
    private float _verticalInput;
    private float _steerAngle;
    private float _breakForce;
    private bool _isBreaking;

    private void Update()
    {
        GetInput();
    }

    private void FixedUpdate()
    {
        HandleMotor();
        HandleSteering();
        UpdateWheelsVisuals();
    }

    private void GetInput()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");
        _isBreaking = Input.GetKey(KeyCode.Space);
    }

    private void HandleMotor()
    {
        foreach (var wheel in acceleratingWheels)
        {
            wheel.motorTorque = _verticalInput * motorForce;
        }

        _breakForce = _isBreaking ? breakForce : 0;

        foreach (var wheel in breakWheels)
        {
            wheel.brakeTorque = _breakForce;
        }
    }

    private void HandleSteering()
    {
        _steerAngle = maxSteerAngle * _horizontalInput;
        foreach (var wheel in swivelWheels)
        {
            wheel.steerAngle = _steerAngle;
        }
    }

    private void UpdateWheelsVisuals()
    {
        foreach (var wheel in wheels)
        {
            wheel.UpdateVisual();
        }
    }
}
