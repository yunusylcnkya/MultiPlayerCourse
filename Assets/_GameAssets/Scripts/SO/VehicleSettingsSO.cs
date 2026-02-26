using UnityEngine;

[CreateAssetMenu(fileName = "Vehicle Settings", menuName = "Scriptable Objects/Vehicle Settings")]

public class VehicleSettingsSO : ScriptableObject
{
    [Header("Wheel Paddings")]
    [SerializeField] private float _wheelsPaddingX;
    [SerializeField] private float _wheelsPaddingZ;

    [Header("Suspension")]
    [SerializeField] private float _springRestLenght;
    [SerializeField] private float _springStrength;
    [SerializeField] private float _springDamper;

    public float WheelPaddingX => _wheelsPaddingX;
    public float WheelPaddingZ => _wheelsPaddingZ;
    public float SpringRestLenght => _springRestLenght;

    public float SpringStrenghth => _springStrength;
    public float SpringDamper => _springDamper;


}
