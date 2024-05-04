using System.IO;
using UnityEditor;
using UnityEngine;

public class TrajectoryDrawer : MonoBehaviour
{
    [SerializeField] private string _recordName;
    [SerializeField] private bool _drawSimpleLine;
    [SerializeField, Range(0, 1)] private float _sphereRadius;
    [SerializeField, Range(0, 1)] private float _lineWidth;
    [SerializeField, Range(0, 1)] private float _tangentWeight;
    [SerializeField] private MoveRecord _record;

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

            var tangent1 = moveEntries[i].Position;
            var tangent2 = moveEntries[i + 1].Position;

            if (i > 0)
            {
                tangent1 += (moveEntries[i].Position - moveEntries[i - 1].Position) * _tangentWeight;
            }

            if (i < moveEntries.Length - 2)
            {
                tangent2 += (moveEntries[i + 1].Position - moveEntries[i + 2].Position) * _tangentWeight;
            }

            Handles.DrawBezier(moveEntries[i].Position, moveEntries[i + 1].Position, tangent1, tangent2, Color.red, Texture2D.whiteTexture, _lineWidth);
        }

        Gizmos.color = Color.green;
        for (int i = 0; i < moveEntries.Length; i++)
        {
            Gizmos.DrawWireSphere(moveEntries[i].Position, _sphereRadius);
        }

        Gizmos.color = oldGizmosColor;
    }
}