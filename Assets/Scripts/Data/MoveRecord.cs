using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MoveRecord
{
    [SerializeField] private Vector3 _targetPosition;
    [SerializeField] private string _recordName;
    [SerializeField] private float _sampleRate;
    [SerializeField] private MoveEntry[] _serializedMoveEntries;
    [SerializeField] private NoMoveEntry[] _serializedNoMoveEntries;
    [NonSerialized, HideInInspector] private LinkedList<MoveEntry> _moveEntriesCache = new LinkedList<MoveEntry>();
    [NonSerialized, HideInInspector] private LinkedList<NoMoveEntry> _noMoveEntriesCache = new LinkedList<NoMoveEntry>();

    public Vector3 TargetPosition
    {
        get => _targetPosition;
        set => _targetPosition = value;
    }

    public float SampleRate
    {
        get => _sampleRate;
        set => _sampleRate = value;
    }

    public string RecordName
    {
        get => _recordName;
        set => _recordName = value;
    }

    public void AddEntry(MoveEntry entry)
    {
        if (_moveEntriesCache.Count > 0 && _moveEntriesCache.Last.Value.Position == entry.Position)
        {
            if (_noMoveEntriesCache.Count > 0 && _noMoveEntriesCache.Last.Value.StartFrame == _moveEntriesCache.Last.Value.Frame)
            {
                var lastNoMoveEntry = _noMoveEntriesCache.Last.Value;
                lastNoMoveEntry.Frames++;
                _noMoveEntriesCache.Last.Value = lastNoMoveEntry;
            }
            else
            {
                _noMoveEntriesCache.AddLast(new NoMoveEntry() { StartFrame = _moveEntriesCache.Last.Value.Frame, Frames = 1 });
            }

            return;
        }

        _moveEntriesCache.AddLast(entry);
    }

    public void Save()
    {
        _serializedMoveEntries = new MoveEntry[_moveEntriesCache.Count];
        _moveEntriesCache.CopyTo(_serializedMoveEntries, 0);
        _moveEntriesCache.Clear();
        _serializedNoMoveEntries = new NoMoveEntry[_noMoveEntriesCache.Count];
        _noMoveEntriesCache.CopyTo(_serializedNoMoveEntries, 0);
        _noMoveEntriesCache.Clear();
    }

    public MoveEntry[] GetMoveEntries() => _serializedMoveEntries;
    public NoMoveEntry[] GetNoMoveEntries() => _serializedNoMoveEntries;
}