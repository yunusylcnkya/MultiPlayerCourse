using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVehicleController : MonoBehaviour
{
    public class SpringData
    {
        public float _currentLength;
        public float _currentVelocity;
    }

    private static readonly WheelType[] _wheels = new WheelType[]{
        WheelType.FrontLeft, WheelType.FrontRight,WheelType.BackLeft,WheelType.BackRight
    };


    [Header("References")]
    [SerializeField] private VehicleSettingsSO _vehicleSettings;
    [SerializeField] private Rigidbody _vehicleRigidbody;
    [SerializeField] private BoxCollider _vehicleCollider;

    private Dictionary<WheelType, SpringData> _springDatas;
    private float _steerInput;
    private float _accelerationInput;

    private void Awake()
    {
        _springDatas = new Dictionary<WheelType, SpringData>();

        foreach (WheelType wheelType in _wheels)
        {
            _springDatas.Add(wheelType, new());
        }
    }


    private void Update()
    {
        //STEER INPUT 
        SetSteerInput(Input.GetAxis("Horizontal"));
        //ACCELERATION INPUT
        SetAccelerateInput(Input.GetAxis("Vertical"));
    }


    private void FixedUpdate()
    {
        //SÜSPANSİYON
        UpdateSuspension();
        //STEERİNG -YER YÖN
        //ACCELERATİON-HIZLANMA
        //BRAKES- FREN
        //AIR RESISTANCE -HAVA DİRENCİ 
    }

    private void SetSteerInput(float steerInput)
    {
        _steerInput = Mathf.Clamp(steerInput, -1f, 1f);
    }

    private void SetAccelerateInput(float accelerateInput)
    {
        _accelerationInput = Mathf.Clamp(accelerateInput, 1f, -1f);
    }

    private void UpdateSuspension()
    {
        foreach (WheelType id in _springDatas.Keys)
        {
            CastSpring(id);
            float currentVelocity = _springDatas[id]._currentVelocity;
            float currentLength = _springDatas[id]._currentLength;

            float force = SpringMathExtensions.CalculateForceDamped(currentLength, currentVelocity, _vehicleSettings.SpringRestLenght, _vehicleSettings.SpringStrenghth, _vehicleSettings.SpringDamper);

            _vehicleRigidbody.AddForceAtPosition(force * transform.up, GetSpringPosition(id));
        }
    }

    private void CastSpring(WheelType wheelType)
    {
        Vector3 position = GetSpringPosition(wheelType);
        float previousLenght = _springDatas[wheelType]._currentLength;
        float currentLenght;
        if (Physics.Raycast(position, -transform.up, out var hit, _vehicleSettings.SpringRestLenght))
        {
            currentLenght = hit.distance;

        }
        else
        {
            currentLenght = _vehicleSettings.SpringRestLenght;
        }
        _springDatas[wheelType]._currentVelocity = (currentLenght - previousLenght) / Time.fixedDeltaTime;
        _springDatas[wheelType]._currentLength = currentLenght;
    }

    private Vector3 GetSpringPosition(WheelType wheelType)
    {
        return transform.localToWorldMatrix.MultiplyPoint3x4(GetSpringRelativePosition(wheelType));
    }

    private Vector3 GetSpringRelativePosition(WheelType wheelType)

    {
        Vector3 boxSize = _vehicleCollider.size;
        float boxBottom = boxSize.y * -0.5f;
        float paddingX = _vehicleSettings.WheelPaddingX;
        float paddingZ = _vehicleSettings.WheelPaddingZ;

        return wheelType switch
        {
            WheelType.FrontLeft => new Vector3(boxSize.x * (paddingX - 0.5f), boxBottom, boxSize.z * (0.5f - paddingZ)),
            WheelType.FrontRight => new Vector3(boxSize.x * (0.5f - paddingX), boxBottom, boxSize.z * (0.5f - paddingZ)),

            WheelType.BackLeft => new Vector3(boxSize.x * (paddingX - 0.5f), boxBottom, boxSize.z * (paddingZ - 0.5f)),
            WheelType.BackRight => new Vector3(boxSize.x * (0.5f - paddingX), boxBottom, boxSize.z * (paddingZ - 0.5f)),

            _ => default
        };

    }

}

public static class SpringMathExtensions
{
    public static float CalculateForceDamped(float currentLength, float lengthVelocity, float restLength, float strength, float damper)
    {
        float lengthOffset = restLength - currentLength;
        return (lengthOffset * strength) - (lengthVelocity * damper);

    }
}
