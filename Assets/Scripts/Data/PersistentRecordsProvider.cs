using System.IO;
using UnityEngine.Serialization;
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
        var persistantDataFiles = new DirectoryInfo(Application.persistentDataPath).GetFiles();

        for (int i = 0; i < persistantDataFiles.Length; i++) 
        {
            if (persistantDataFiles[i].Extension != ".json")
            {
                continue;
            }

            var json = File.ReadAllText(persistantDataFiles[i].FullName);
            var record = JsonUtility.FromJson<MoveRecord>(json);

            if (record != null) 
            { 
                _moveRecordsStorage.RemoveRecord(record.RecordName);
                _moveRecordsStorage.AddRecord(record);
            }
        }
    }
}