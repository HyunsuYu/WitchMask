using CommonUtilLib.ThreadSafe;
using Newtonsoft.Json;
using System.IO;
using UnityEditor.Overlays;
using UnityEngine;


public sealed class SettingDataBuffer : SingleTonForGameObject<SettingDataBuffer>
{
    private SettingData? m_settingData;

    [SerializeField] private string m_fileName = "SettingData.json";


    public void Awake()
    {
        SetInstance(this);

        LoadData();
    }
    public void OnDestroy()
    {
        Dispose();
    }

    internal SettingData Data
    {
        get
        {
            if (!m_settingData.HasValue)
            {
                throw new System.Exception("Data is now null");
            }
            return m_settingData.Value;
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
        if (!File.Exists(DataPath))
        {
            m_settingData = new SettingData()
            {
                Screen = new SettingData.ScreenSetting()
                {
                    Resolution = SettingData.ScreenSetting.ResolutionType.FHD_1920_1080,
                    BIsFullScreen = true
                },
                Audio = new SettingData.AudioSetting()
                {
                    MasterVolume = 0.8f
                }
            };
            SaveData();
        }
        else
        {
            string jsonData = File.ReadAllText(DataPath);
            m_settingData = JsonConvert.DeserializeObject<SettingData>(jsonData);
        }
    }
    internal void SaveData()
    {
        if (!m_settingData.HasValue)
        {
            Debug.LogWarning("SettingDataBuffer.SaveData : Cut data is null");
            return;
        }

        string jsonData = JsonConvert.SerializeObject(m_settingData.Value);

        string directory = Path.GetDirectoryName(DataPath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(DataPath, jsonData);
    }

    protected override void Dispose(bool bisDisposing)
    {
        m_settingData = null;
    }
}