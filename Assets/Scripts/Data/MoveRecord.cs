using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class MoveRecord
{
    [SerializeField] private Vector3 _targetPosition;
    [SerializeField] private string _recordName;
    [SerializeField] private MoveEntry[] _serializedMoveEntries;
    [NonSerialized, HideInInspector] private LinkedList<MoveEntry> _moveEntriesCache = new LinkedList<MoveEntry>();

    public Vector3 TargetPosition 
    { 
        get => _targetPosition;
        set => _targetPosition = value;
    }

    public string RecordName
    {
        get => _recordName;
        set => _recordName = value;
    }

    public void AddEntry(MoveEntry entry)
    {
        _moveEntriesCache.AddLast(entry);
    }

    public void Save()
    {
        _serializedMoveEntries = new MoveEntry[_moveEntriesCache.Count];
        _moveEntriesCache.CopyTo(_serializedMoveEntries, 0);
    }

    public MoveEntry[] GetMoveEntries() => _serializedMoveEntries;
}