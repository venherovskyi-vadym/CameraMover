using UnityEngine;

[CreateAssetMenu(menuName = "MoveSettings")]
public class MoveSettings : InputSourceBase
{
    [SerializeField] private string _horizontalAxis;
    [SerializeField] private string _verticalAxis;
    [SerializeField, Range(-5, 5)] private float _sensitivityX;
    [SerializeField, Range(-5, 5)] private float _sensitivityY;

    public override Vector3 GetInput()
    {
        if (!Input.GetMouseButton(0))
        {
            return Vector3.zero;
        }

        var delta = new Vector3(Input.GetAxis(_horizontalAxis), Input.GetAxis(_verticalAxis));
        delta.x *= _sensitivityX;
        delta.y *= _sensitivityY;

        return delta;
    }
}