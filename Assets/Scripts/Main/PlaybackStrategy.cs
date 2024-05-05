using System;
using UnityEngine;

public class PlaybackStrategy : ICameraStrategy
{
    private const float _resampleRate = 10f;

    private readonly Transform _camera;
    private readonly Transform _target;
    private readonly MoveRecord _moveRecord;
    private readonly InterpolatedMoveRecord _interpolatedMoveRecord;

    private float _startTime;
    private bool _isPlaying;
    private Vector3[] _interpolatedPositions;
    private bool[] _isMoveByFrame;
    private int[] _moveIndexByFrame;
    private int[] _noMoveIndexByFrame;

    public PlaybackStrategy(Transform camera, Transform target, MoveRecord moveRecord)
    {
        _camera = camera;
        _target = target;
        _moveRecord = moveRecord;
        _interpolatedMoveRecord = new InterpolatedMoveRecord(_moveRecord, _resampleRate);
        _interpolatedPositions = _interpolatedMoveRecord.Positions;
        var moveEntries = _moveRecord.GetMoveEntries();
        var noMoveEntries = _moveRecord.GetNoMoveEntries();
        var lastNoMoveEntry = noMoveEntries[noMoveEntries.Length - 1];
        var frameCount = Math.Max(moveEntries[moveEntries.Length - 1].Frame, lastNoMoveEntry.StartFrame + lastNoMoveEntry.Frames);
        _isMoveByFrame = new bool[frameCount];
        _moveIndexByFrame = new int[frameCount];
        _noMoveIndexByFrame = new int[frameCount];

        for (int i = 0; i < moveEntries.Length; i++) 
        {
            var moveEntry = moveEntries[i];
            _isMoveByFrame[moveEntry.Frame] = true;
            _moveIndexByFrame[moveEntry.Frame] = i;
        }

        for (int i = 0; i < noMoveEntries.Length; i++)
        {
            var noMoveEntry = noMoveEntries[i];
            
            for (int j = 0; j < noMoveEntry.Frames; j++) 
            {
                _noMoveIndexByFrame[noMoveEntry.StartFrame + j] = i;
            }
        }
    }

    public void Init(float time)
    {
        _startTime = time;
        _isPlaying = true;
        _target.position = _moveRecord.TargetPosition;
        _camera.position = _moveRecord.GetMoveEntries()[0].Position;
        _camera.LookAt(_target);
    }

    public void Update(float time, float timeDelta)
    {
        if (!_isPlaying)
        {
            return;
        }

        var currentTime = time - _startTime;
        var sampleIndexFloat = currentTime / _moveRecord.SampleRate;
        var sampleIndex = Mathf.FloorToInt(sampleIndexFloat);
        var lerpFactor = sampleIndexFloat - sampleIndex;

        if (sampleIndex >= _isMoveByFrame.Length - 1)
        {
            _isPlaying = false;
            return;
        }

        if (_isMoveByFrame[sampleIndex])
        {
            var index = _moveIndexByFrame[sampleIndex];
            var interpolatedPositionIndexFloat = (index + lerpFactor) * _resampleRate;
            var interpolatedPositionIndex = Mathf.FloorToInt(interpolatedPositionIndexFloat);
            var interpolatedLerpFactor = interpolatedPositionIndexFloat - interpolatedPositionIndex;
            var position = Vector3.Lerp(_interpolatedPositions[interpolatedPositionIndex], _interpolatedPositions[interpolatedPositionIndex + 1], interpolatedLerpFactor);
            _camera.position = position;
            _camera.LookAt(_target);
        }
    }

    public void Finish()
    {
        _isPlaying = false;
    }
}