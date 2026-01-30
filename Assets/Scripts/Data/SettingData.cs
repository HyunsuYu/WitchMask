using UnityEngine;


public struct SettingData
{
    public struct ScreenSetting
    {
        public enum ResolutionType
        {
            FHD_1920_1080,
            QHD_2560_1440,
            UHD_3840_2160
        }


        public ResolutionType Resolution;
        public bool BIsFullScreen;
    }
    public struct AudioSetting
    {
        public float MasterVolume;
    }


    public ScreenSetting Screen;
    public AudioSetting Audio;
}

public static class SettingDataExtension
{
    public static Vector2Int GetGetResolutionSize(this SettingData settingData)
    {
        switch(settingData.Screen.Resolution)
        {
            case SettingData.ScreenSetting.ResolutionType.FHD_1920_1080:
                return new Vector2Int(1920, 1080);

            case SettingData.ScreenSetting.ResolutionType.QHD_2560_1440:
                return new Vector2Int(2560, 1440);

            case SettingData.ScreenSetting.ResolutionType.UHD_3840_2160:
                return new Vector2Int(3840, 2160);
        }

        return Vector2Int.zero;
    }
}