using UnityEngine;

public class PlaybackStrategy : ICameraStrategy
{
    private readonly Transform _camera;
    private readonly Transform _target;
    private readonly DistanceSetting _distanceSetting;
    private readonly MoveRecord _moveRecord;

    private float _startTime;
    private bool _isPlaying;
    private int _moveEntryIndex;

    public PlaybackStrategy(Transform camera, Transform target, DistanceSetting distanceSetting, MoveRecord moveRecord)
    {
        _camera = camera;
        _target = target;
        _distanceSetting = distanceSetting;
        _moveRecord = moveRecord;
    }

    public void Init(float time)
    {
        _startTime = time;
        _isPlaying = true;
        _target.position = _moveRecord.TargetPosition;
        _camera.position = _moveRecord.GetMoveEntries()[0].Position;
        _camera.LookAt(_target);
    }

    public void Update(float time)
    {
        if (!_isPlaying) 
        {
            return;
        }

        var currentTime = time - _startTime;
        var entries = _moveRecord.GetMoveEntries();
        for (int i = _moveEntryIndex + 1; i < entries.Length - 1; i++)
        {
            if (currentTime > entries[i].Time && currentTime <= entries[i + 1].Time)
            {
                var lerpFactor = Mathf.InverseLerp(entries[i].Time, entries[i + 1].Time, currentTime);
                var position = Vector3.Lerp(entries[i].Position, entries[i + 1].Position, lerpFactor);
                _camera.position = position;
                _camera.LookAt(_target);
                _moveEntryIndex = i;
                break;
            }
        }
    }

    public void Finish()
    {
        _isPlaying = false;
    }
}