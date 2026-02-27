using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVehicleVisualController : MonoBehaviour
{
    [SerializeField] private PlayerVehicleController _playerVehicleController;
    [SerializeField] private Transform _wheelFrontLeft, _wheelFrontRigft, _wheelBackLeft, _wheelBackRight;
    [SerializeField] private float _wheelsSpinSpeed, _wheelYWhenSpringMin, _wheelYWhenSpringMax;


    private Quaternion _wheelFrontLeftRoll;
    private Quaternion _wheelFrontRightRoll;

    private float _springRestLength;
    private float _forwardSpeed;

    private float _steerInput;
    private float _steerAngle;


    private Dictionary<WheelType, float> _springCurrentLength = new()
    {
      {WheelType.FrontLeft,0f},
      {WheelType.FrontRight,0f},
      {WheelType.BackLeft,0f},
      {WheelType.BackRight,0f}
    };


    void Start()
    {
        _wheelFrontLeftRoll = _wheelFrontLeft.localRotation;
        _wheelFrontRightRoll = _wheelFrontRigft.localRotation;
        _springRestLength = _playerVehicleController.Settings.SpringRestLenght;
        _steerAngle = _playerVehicleController.Settings.SteerAngle;
    }


    private void Update()
    {
        //Update Visual States
        UpdateVisualStates();
        //Rotate Wheels
        RotateWheels();
        //Set Suspension
        SetSuspension();
    }

    private void SetSuspension()
    {
        float springFrontLeftRatio = _springCurrentLength[WheelType.FrontLeft] / _springRestLength;
        float springFrontRightRatio = _springCurrentLength[WheelType.FrontRight] / _springRestLength;
        float springBackLeftRatio = _springCurrentLength[WheelType.BackLeft] / _springRestLength;
        float springBackRightRatio = _springCurrentLength[WheelType.BackRight] / _springRestLength;

        _wheelFrontLeft.localPosition = new Vector3(_wheelFrontLeft.localPosition.x,
        _wheelYWhenSpringMin + (_wheelYWhenSpringMax - _wheelYWhenSpringMin) * springFrontLeftRatio, _wheelFrontLeft.localPosition.z);

        _wheelFrontRigft.localPosition = new Vector3(_wheelFrontRigft.localPosition.x,
      _wheelYWhenSpringMin + (_wheelYWhenSpringMax - _wheelYWhenSpringMin) * springFrontRightRatio, _wheelFrontRigft.localPosition.z);

        _wheelBackLeft.localPosition = new Vector3(_wheelBackLeft.localPosition.x,
      _wheelYWhenSpringMin + (_wheelYWhenSpringMax - _wheelYWhenSpringMin) * springBackLeftRatio, _wheelBackLeft.localPosition.z);

        _wheelBackRight.localPosition = new Vector3(_wheelBackRight.localPosition.x,
      _wheelYWhenSpringMin + (_wheelYWhenSpringMax - _wheelYWhenSpringMin) * springBackRightRatio, _wheelBackRight.localPosition.z);

    }

    private void UpdateVisualStates()
    {
        _steerInput = Input.GetAxis("Horizontal");

        _forwardSpeed = Vector3.Dot(_playerVehicleController.Forward, _playerVehicleController.Velocity);

        _springCurrentLength[WheelType.FrontLeft] = _playerVehicleController.GetSpringCurrentLength(WheelType.FrontLeft);
        _springCurrentLength[WheelType.FrontRight] = _playerVehicleController.GetSpringCurrentLength(WheelType.FrontRight);
        _springCurrentLength[WheelType.BackLeft] = _playerVehicleController.GetSpringCurrentLength(WheelType.BackLeft);
        _springCurrentLength[WheelType.BackRight] = _playerVehicleController.GetSpringCurrentLength(WheelType.BackRight);
    }

    private void RotateWheels()
    {
        if (_springCurrentLength[WheelType.FrontLeft] < _springRestLength)
        {
            _wheelFrontLeftRoll *= Quaternion.AngleAxis(_forwardSpeed * _wheelsSpinSpeed * Time.deltaTime, Vector3.right);
        }

        if (_springCurrentLength[WheelType.FrontRight] < _springRestLength)
        {
            _wheelFrontRightRoll *= Quaternion.AngleAxis(_forwardSpeed * _wheelsSpinSpeed * Time.deltaTime, Vector3.right);
        }

        if (_springCurrentLength[WheelType.BackLeft] < _springRestLength)
        {
            _wheelBackLeft.localRotation *= Quaternion.AngleAxis(_forwardSpeed * _wheelsSpinSpeed * Time.deltaTime, Vector3.right);
        }

        if (_springCurrentLength[WheelType.BackRight] < _springRestLength)
        {
            _wheelBackRight.localRotation *= Quaternion.AngleAxis(_forwardSpeed * _wheelsSpinSpeed * Time.deltaTime, Vector3.right);
        }

        _wheelFrontLeft.localRotation = Quaternion.AngleAxis(_steerInput * _steerAngle, Vector3.up) * _wheelFrontLeftRoll;
        _wheelFrontRigft.localRotation = Quaternion.AngleAxis(_steerInput * _steerAngle, Vector3.up) * _wheelFrontRightRoll;

    }
}
