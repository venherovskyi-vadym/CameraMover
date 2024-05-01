using UnityEngine;

public class UserControlStrategy : ICameraStrategy
{
    private readonly Transform _camera;
    private readonly Transform _target;
    private readonly IInputSource _inputSource;
    private readonly IDistanceSetting _distanceSetting;

    public UserControlStrategy(Transform camera, Transform target, InputSourceBase inputSource, IDistanceSetting distanceSetting)
    {
        _camera = camera;
        _target = target;
        _inputSource = inputSource;
        _distanceSetting = distanceSetting;
    }

    public void Init(float time)
    {
    }

    public void Update(float time)
    {
        if (_camera == null || _target == null)
        {
            return;
        }

        _camera.Translate(_inputSource.GetInput() * _distanceSetting.Distance);

        var fromTargetToCamera = _target.position - _camera.position;
        _camera.position = _target.position - fromTargetToCamera.normalized * _distanceSetting.Distance;
        _camera.LookAt(_target);
    }
    public void Finish()
    {
    }
}