using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RecordStrategy : ICameraStrategy
{
    private readonly Transform _camera;
    private readonly Transform _target;
    private readonly MoveRecord _moveRecord = new MoveRecord();
    private readonly MoveRecordsStorage _moveRecordsStorage;
    private bool _inited;
    private char[] _invalidFileNameChars;

    public RecordStrategy(Transform camera, Transform target, MoveRecordsStorage moveRecordsStorage)
    {
        _camera = camera;
        _target = target;
        _moveRecordsStorage = moveRecordsStorage;
        var invalidFileNameChars = new List<char>(Path.GetInvalidPathChars());

        foreach (var invalidChar in Path.GetInvalidFileNameChars())
        {
            if (invalidFileNameChars.Contains(invalidChar))
            {
                continue;
            }

            invalidFileNameChars.Add(invalidChar);
        }

        _invalidFileNameChars = invalidFileNameChars.ToArray();
    }

    public void Init(float time)
    {
        _moveRecord.TargetPosition = _target.position;
        _moveRecord.SampleRate = Time.fixedDeltaTime;
        _moveRecord.RecordName = DateTime.Now.ToString();
        _inited = true;
        _moveRecord.AddEntry(new MoveEntry() { Position = _camera.position, Frame = 0 });
    }

    public void Update(float time, float timeDelta)
    {
        if (!_inited)
        {
            return;
        }

        _moveRecord.AddEntry(new MoveEntry() { Position = _camera.position, Frame = (int)(time / _moveRecord.SampleRate) });
    }

    public void Finish()
    {
        _moveRecord.Save();
        _moveRecordsStorage.AddRecord(_moveRecord);
        var json = JsonUtility.ToJson(_moveRecord);
        var fileName = _moveRecord.RecordName;


        foreach (var invalidChar in _invalidFileNameChars)
        {
            fileName = fileName.Replace(invalidChar, '-');
        }

        if (!Directory.Exists(Application.persistentDataPath))
        {
            Directory.CreateDirectory(Application.persistentDataPath);
        }

        var path = Path.Combine(Application.persistentDataPath, $"{fileName}.json");
        File.WriteAllText(path, json);
    }
}