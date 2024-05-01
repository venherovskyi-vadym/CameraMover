using UnityEngine;
using TMPro;

public class RecordSelector : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _dropdown;
    private MoveRecordsStorage _moveRecordsStorage;

    public void FillDropDownFrom(MoveRecordsStorage moveRecordsStorage)
    {
        _moveRecordsStorage = moveRecordsStorage;
        _dropdown.ClearOptions();
        var options =_moveRecordsStorage.MoveRecords.ConvertAll(ca => new TMP_Dropdown.OptionData(ca.RecordName));
        _dropdown.AddOptions(options);
    }

    public MoveRecord GetRecord()
    {
        if (_dropdown.value < 0 || _dropdown.value >= _moveRecordsStorage.MoveRecords.Count)
        {
            return null;
        }

        return _moveRecordsStorage.MoveRecords[_dropdown.value];
    }
}