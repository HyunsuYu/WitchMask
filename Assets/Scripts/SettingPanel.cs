using UnityEngine;
using TMPro; 
using UnityEngine.UI;
using System.Linq;

public class SettingPanel : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullScreenToggle;
    [SerializeField] private Button applyButton;

    private SettingData currentSetting;

    private void Start()
    {
        LoadSettings();
        
        resolutionDropdown.value = (int)currentSetting.Screen.Resolution;
        fullScreenToggle.isOn = currentSetting.Screen.BIsFullScreen;

        resolutionDropdown.ClearOptions();
        string[] enumNames = System.Enum.GetNames(typeof(SettingData.ScreenSetting.ResolutionType));
        resolutionDropdown.AddOptions(enumNames.ToList());

        gameObject.SetActive(false);
    }

    private void LoadSettings()
    {
        // 실제 프로젝트라면 PlayerPrefs나 JSON에서 불러오는 로직이 들어갑니다.
        currentSetting.Screen.Resolution = SettingData.ScreenSetting.ResolutionType.FHD_1920_1080;
        currentSetting.Screen.BIsFullScreen = Screen.fullScreen;
    }

    public void ApplySettings()
    {
        // 드롭다운의 선택된 인덱스를 Enum으로 캐스팅
        currentSetting.Screen.Resolution = (SettingData.ScreenSetting.ResolutionType)resolutionDropdown.value;
        currentSetting.Screen.BIsFullScreen = fullScreenToggle.isOn;

        // Extension 메서드를 사용하여 해상도 사이즈 가져오기
        Vector2Int res = currentSetting.GetGetResolutionSize();

        // 실제 유니티 엔진에 적용
        Screen.SetResolution(res.x, res.y, currentSetting.Screen.BIsFullScreen);
        
        // Debug.Log($"설정 적용: {res.x}x{res.y}, 전체화면: {currentSetting.Screen.BIsFullScreen}");
        
        SaveDataBuffer.Instance.SaveData();
    }
}