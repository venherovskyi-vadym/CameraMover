using UnityEngine;

[CreateAssetMenu(menuName = "MouseMoveSettings")]
public class MouseMoveSettings : InputSourceBase
{
    [SerializeField, Range(-5, 5)] private float _sensitivityX;
    [SerializeField, Range(-5, 5)] private float _sensitivityY;

    [SerializeField] private Vector3 _oldMousePos;

    public override Vector3 GetInput()
    {
        if (!Input.GetMouseButton(0))
        {
            _oldMousePos = Input.mousePosition;
            return Vector3.zero;
        }

        var delta = Input.mousePosition - _oldMousePos;
        delta.x *= _sensitivityX / Screen.width;
        delta.y *= _sensitivityY / Screen.height;

        _oldMousePos = Input.mousePosition;

        return delta;
    }
}