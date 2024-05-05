using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MoveRecordsStorage")]
public class MoveRecordsStorage : ScriptableObject
{
    [SerializeField] private List<MoveRecord> _moveRecords;

    public List<MoveRecord> MoveRecords => _moveRecords;

    public void AddRecord(MoveRecord record)
    {
        _moveRecords.Add(record);
    }

    public void RemoveRecord(string recordName)
    {
        _moveRecords.RemoveAll(ra => ra.RecordName.Equals(recordName));
    }

    public void ClearRecords() => _moveRecords.Clear();
}