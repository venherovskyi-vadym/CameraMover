using System.IO;
using UnityEditor;
using UnityEngine;

public class TrajectoryDrawer : MonoBehaviour
{
    [SerializeField] private string _recordName;
    [Header("original data")]
    [SerializeField] private bool _drawSimpleLine;
    [SerializeField] private bool _drawEntryIndex;
    [SerializeField, Range(0, 1)] private float _sphereRadius;
    [SerializeField] private MoveRecord _record;
    [Header("interpolated data")]
    [SerializeField] private bool _drawInterpolatedSimpleLine;
    [SerializeField, Range(0, 10)] private float _resampleRatio;
    [SerializeField, Range(0, 1)] private float _interpolatedSphereRadius;

    private InterpolatedMoveRecord _interpolatedMoveRecord;

    private void OnValidate()
    {
        TryLoadRecord();
    }

    private void OnEnable()
    {
        TryLoadRecord();
    }

    private void TryLoadRecord()
    {
        _record = null;

        if (string.IsNullOrEmpty(_recordName))
        {
            return;
        }

        var path = Path.Combine(Application.persistentDataPath, _recordName);

        if (Directory.Exists(Application.persistentDataPath) && File.Exists(path))
        {
            _record = PersistentRecordsProvider.GetRecordsFromFileInfo(new FileInfo(path));
            _interpolatedMoveRecord = new InterpolatedMoveRecord(_record, _resampleRatio);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_record == null || _record.GetMoveEntries() == null)
        {
            return;
        }

        var moveEntries = _record.GetMoveEntries();
        var oldGizmosColor = Gizmos.color;
        Gizmos.color = Color.red;

        for (int i = 0; i < moveEntries.Length - 1; i++)
        {
            if (_drawSimpleLine)
            {
                Gizmos.DrawLine(moveEntries[i].Position, moveEntries[i + 1].Position);
            }
        }

        for (int i = 0; i < moveEntries.Length; i++)
        {
            Gizmos.DrawWireSphere(moveEntries[i].Position, _sphereRadius);
            if (_drawEntryIndex)
            {
                Handles.Label(moveEntries[i].Position, i.ToString());
            }
        }

        Gizmos.color = Color.green;
        if (_interpolatedMoveRecord != null)
        {

            for (int i = 0; i < _interpolatedMoveRecord.Positions.Length - 1; i++)
            {
                if (_drawInterpolatedSimpleLine)
                {
                    Gizmos.DrawLine(_interpolatedMoveRecord.Positions[i], _interpolatedMoveRecord.Positions[i + 1]);
                }
            }
        }

        for (int i = 0; i < _interpolatedMoveRecord.Positions.Length; i++)
        {
            Gizmos.DrawWireSphere(_interpolatedMoveRecord.Positions[i], _interpolatedSphereRadius);
        }

        Gizmos.color = oldGizmosColor;
    }
}