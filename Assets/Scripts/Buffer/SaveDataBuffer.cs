using System.IO;

using UnityEngine;

using Newtonsoft.Json;

using CommonUtilLib.ThreadSafe;


public sealed class SaveDataBuffer : SingleTonForGameObject<SaveDataBuffer>
{
    private SaveData? m_saveData;

    [SerializeField] private string m_fileName = "SaveData.json";


    public void Awake()
    {
        SetInstance(this);

        LoadData();
    }
    public void OnDestroy()
    {
        Dispose();
    }

    internal SaveData Data
    {
        get
        {
            if(!m_saveData.HasValue)
            {
                throw new System.Exception("Data is now null");
            }
            return m_saveData.Value;
        }
    }

    private string DataPath
    { 
        get
        {
            return Path.Combine(Application.streamingAssetsPath, m_fileName);
        }
    }

    internal void LoadData()
    {
        if(!File.Exists(DataPath))
        {
            m_saveData = new SaveData()
            {
                PlayerPos = Vector2Int.zero,
                InventoryItems = new SaveData.InventoryNode[40]
            };
            SaveData();
        }
        else
        {
            string jsonData = File.ReadAllText(DataPath);
            m_saveData = JsonConvert.DeserializeObject<SaveData>(jsonData);
        }
    }
    internal void SaveData()
    {
        if(!m_saveData.HasValue)
        {
            Debug.LogWarning("SaveDataBuffer.SaveData : Cut data is null");
            return;
        }

        string jsonData = JsonConvert.SerializeObject(m_saveData.Value);

        string directory = Path.GetDirectoryName(DataPath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(DataPath, jsonData);
    }

    protected override void Dispose(bool bisDisposing)
    {
        m_saveData = null;
    }
}