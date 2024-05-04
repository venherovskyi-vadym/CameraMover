using UnityEngine;

[CreateAssetMenu(menuName = "MouseMoveSettings")]
public class MouseMoveSettings : InputSourceBase
{
    [SerializeField] private bool _inverseX;
    [SerializeField] private bool _inverseY;
    [SerializeField, Range(0, 100)] private float _sensitivityX;
    [SerializeField, Range(0, 100)] private float _sensitivityY;

    private Vector3 _oldMousePos;

    public override Vector3 GetInput()
    {
        if (!Input.GetMouseButton(0) || Input.GetMouseButtonDown(0))
        {
            _oldMousePos = Input.mousePosition;
            return Vector3.zero;
        }

        var delta = Input.mousePosition - _oldMousePos;
        delta.x *= _sensitivityX / Screen.width;
        delta.y *= _sensitivityY / Screen.height;

        if (_inverseX)
        {
            delta.x *= -1;
        }

        if (_inverseY)
        {
            delta.y *= -1;
        }

        _oldMousePos = Input.mousePosition;

        return delta;
    }
}