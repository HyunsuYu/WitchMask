using System.Collections.Generic;
using UnityEngine;

using CommonUtilLib.ThreadSafe;


public class InventoryController : SingleTonForGameObject<InventoryController>
{
    [SerializeField] private GameObject m_layout_InventoryBackground;
    [SerializeField] private GameObject inventoryPanel; 
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private int columnSize = 8;
    [SerializeField] private int rowSize = 4;
    [SerializeField] private ItemDatabase itemDatabase;
    [SerializeField] private InventoryDragIcon dragIconUI;

    [SerializeField] private GameObject m_gameObjectt_OpenInventoryBtn;


    public void Awake()
    {
        SetInstance(this);
    }

    public int DraggingIndex { get; private set; } = -1;
    public void BeginDrag(int index, Sprite iconSprite)
    {
        DraggingIndex = index;
        dragIconUI.ShowIcon(true, iconSprite);
    }
    public void OnDrag(Vector2 ScreenPos) => dragIconUI.UpdatePosition(ScreenPos);
    public void EndDrag()
    {
        DraggingIndex = -1;
        dragIconUI.ShowIcon(false);
    }

    private List<InventorySlotUI> m_slotUIList = new List<InventorySlotUI>();

    private void Start()
    {
        InitInventoryUI();
    }

    private void InitInventoryUI()
    {
        // 2. columnSize * rowSize 만큼 슬롯 생성
        int targetSlotCount = columnSize * rowSize;
        for (int i = 0; i < targetSlotCount; i++)
        {
            GameObject slotGo = Instantiate(slotPrefab, inventoryPanel.transform);
            InventorySlotUI slotUI = slotGo.GetComponent<InventorySlotUI>();
            
            slotUI.Init(i, this);
            m_slotUIList.Add(slotUI);
        }

        // 3. 데이터 로드 및 적용
        RefreshAll();

        // 시작 시 인벤토리는 닫힌 상태
        m_layout_InventoryBackground.SetActive(false);
    }

    public void RefreshAll()
    {
        // 싱글톤 인스턴스 확인
        if (SaveDataBuffer.Instance == null) return;

        // 세이브 데이터 배열 가져오기
        var inventoryData = SaveDataBuffer.Instance.Data.InventoryItems;
        int targetSlotCount = columnSize * rowSize;

        for (int i = 0; i < targetSlotCount; i++)
        {
            // 데이터가 존재하고, 현재 인덱스가 데이터 범위 내에 있는 경우
            if (inventoryData != null && i < inventoryData.Length)
            {
                ItemType type = inventoryData[i].ItemType;
                int count = inventoryData[i].Count;

                // 데이터베이스에서 시각 정보(아이콘 등) 가져오기
                var itemInfo = itemDatabase.GetItemInfo(type);
                m_slotUIList[i].UpdateSlot(itemInfo, count);
            }
            else
            {
                // 데이터가 없는 인덱스(14번 이후 등)는 빈 슬롯으로 초기화
                // ItemType.None을 가진 기본 ItemInfo를 전달
                m_slotUIList[i].UpdateSlot(default, 0);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            OpenInventory();
        }    
    }

    public void OpenInventory()
    {
        bool isActive = !m_layout_InventoryBackground.activeSelf;
        m_layout_InventoryBackground.SetActive(isActive);

        // 인벤토리가 열릴 때 최신 정보로 갱신
        if (isActive) RefreshAll();

        MaskSlotControl.Instance.ActiveMaskSlotUIs();
        m_gameObjectt_OpenInventoryBtn.SetActive(!isActive);
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}