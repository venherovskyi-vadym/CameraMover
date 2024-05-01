using System;
using UnityEngine;
using UnityEngine.Serialization;
using System.IO;

public class RecordStrategy : ICameraStrategy
{
    private const float _minDiffMagnitudeToRecord = 0.0001f;
    private readonly Transform _camera;
    private readonly Transform _target;
    private readonly MoveRecord _moveRecord = new MoveRecord();
    private readonly MoveRecordsStorage _moveRecordsStorage;
    private readonly IInputSource _inputSource;
    private Vector3 _oldCameraPosition;
    private float _startTime;
    private bool _inited;

    public RecordStrategy(Transform camera, Transform target, MoveRecordsStorage moveRecordsStorage, IInputSource inputSource)
    {
        _camera = camera;
        _target = target;
        _moveRecordsStorage = moveRecordsStorage;
        _inputSource = inputSource;
    }

    public void Init(float time)
    {
        _moveRecord.TargetPosition = _target.position;
        _moveRecord.RecordName = DateTime.Now.ToString();
        _startTime = time;
        _oldCameraPosition = _camera.position;
        _inited = true;
        _moveRecord.AddEntry(new MoveEntry() { Position = _camera.position, Time = 0, Input = _inputSource.GetInput() });
    }

    public void Update(float time)
    {
        if (!_inited)
        {
            return;
        }

        var input = _inputSource.GetInput();
        //if ((_camera.position - _oldCameraPosition).magnitude > _minDiffMagnitudeToRecord)
        if (input.sqrMagnitude > _minDiffMagnitudeToRecord)
        {
            _moveRecord.AddEntry(new MoveEntry() { Position = _camera.position, Time = time - _startTime, Input = _inputSource.GetInput() });
            _oldCameraPosition = _camera.position;
        }
    }

    public void Finish()
    {
        _moveRecord.Save();
        _moveRecordsStorage.AddRecord(_moveRecord);
        var json = JsonUtility.ToJson(_moveRecord);
        var fileName = _moveRecord.RecordName.Replace(':', '-');
        var path = Path.Combine(Application.persistentDataPath, $"{fileName}.json");
        File.WriteAllText(path, json);
    }
}