using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PersistentRecordsProvider
{
    private readonly MoveRecordsStorage _moveRecordsStorage;

    public PersistentRecordsProvider(MoveRecordsStorage moveRecordsStorage)
    {
        _moveRecordsStorage = moveRecordsStorage;
    }

    public void AddRecordsFromPersistantData()
    {
        var moveRecords = GetRecordsFromPersistantData();

        if(moveRecords == null)
        {
            return;
        }

        foreach (var item in moveRecords)
        {
            _moveRecordsStorage.RemoveRecord(item.RecordName);
            _moveRecordsStorage.AddRecord(item);
        }
    }

    private MoveRecord[] GetRecordsFromPersistantData()
    {
        if (!Directory.Exists(Application.persistentDataPath))
        {
            return null;
        }

        List<MoveRecord> result = new List<MoveRecord>();
        var persistantDataFiles = new DirectoryInfo(Application.persistentDataPath).GetFiles();

        for (int i = 0; i < persistantDataFiles.Length; i++)
        {
            MoveRecord record = GetRecordsFromFileInfo(persistantDataFiles[i]);

            if (record != null)
            {
                result.Add(record);
            }
        }

        return result.ToArray();
    }


    /// Static modifier is for access from TrajectoryDrawer
    public static MoveRecord GetRecordsFromFileInfo(FileInfo fileInfo)
    {
        if (fileInfo.Extension != ".json")
        {
            return null;
        }

        var json = File.ReadAllText(fileInfo.FullName);
        MoveRecord record;

        try
        {
            record = JsonUtility.FromJson<MoveRecord>(json);
        }
        catch
        {
            return null;
        }

        return record;
    }
}