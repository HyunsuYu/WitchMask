using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryTestUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Dropdown itemDropdown;
    [SerializeField] private Button addButton;
    [SerializeField] private Button removeButton;

    [SerializeField] private ItemDatabase itemDatabase;
    [SerializeField] private InventoryController inventoryController;

    private List<ItemType> m_availableTypes = new List<ItemType>();

    private void Awake()
    {
        InitDropdown();

        // 버튼 이벤트 연결
        addButton.onClick.AddListener(OnClickAdd);
        removeButton.onClick.AddListener(OnClickRemove);
    }

    private void InitDropdown()
    {
        itemDropdown.ClearOptions();
        m_availableTypes.Clear();

        List<string> options = new List<string>();

        // 데이터베이스에 있는 모든 아이템 종류를 드롭다운에 추가
        foreach (var item in itemDatabase.AllItems)
        {
            if (item.Type == ItemType.None) continue;
            options.Add(item.Name);
            m_availableTypes.Add(item.Type);
        }

        itemDropdown.AddOptions(options);
        itemDropdown.RefreshShownValue();
    }

    /// <summary>
    /// 추가 테스트
    /// </summary> <summary>
    public void OnClickAdd()
    {
        if (m_availableTypes.Count == 0) return;

        // 선택된 아이템 타입 가져오기
        ItemType selectedType = m_availableTypes[itemDropdown.value];

        // 데이터 추가 (SaveDataBuffer를 통해 수정)
        SaveDataBuffer.Instance.Data.AddInventoryItem(selectedType, 1);

        // UI 즉시 갱신
        inventoryController.RefreshAll();
    }

    /// <summary>
    ///  삭제 테스트
    /// </summary> <summary>
    private void OnClickRemove()
    {
        if (m_availableTypes.Count == 0) return;

        ItemType selectedType = m_availableTypes[itemDropdown.value];

        // 데이터 삭제 (앞에서부터 순회하며 삭제하는 메서드 사용)
        SaveDataBuffer.Instance.Data.MinusInventoryItem(selectedType, 1);

        // UI 즉시 갱신
        inventoryController.RefreshAll();
    }
}