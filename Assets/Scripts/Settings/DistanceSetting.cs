using UnityEngine;

[CreateAssetMenu(menuName = "DistanceSetting")]
public class DistanceSetting : ScriptableObject, IDistanceSetting
{
    [SerializeField] private float _distanceToTarget;
    public float Distance => _distanceToTarget;
}