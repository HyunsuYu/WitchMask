using CommonUtilLib.ThreadSafe;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;


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

    [SerializeField] private EventSystem m_eventSystem;
    [SerializeField] private GraphicRaycaster m_graphicsRaycaster;


    public void Awake()
    {
        SetInstance(this);
    }

    public int DraggingIndex { get; private set; } = -1;
    internal (ItemType holdItem, int count) DraggingItem
    {
        get
        {
            var data = SaveDataBuffer.Instance.Data.InventoryItems[DraggingIndex];
            return (data.ItemType, data.Count);
        }
    }

    internal ItemDatabase ItemDatabase
    {
        get
        {
            return itemDatabase;
        }
    }
    internal InventoryDragIcon DragIcon
    {
        get
        {
            return dragIconUI;
        }
    }

    public void BeginDrag(int index, Sprite iconSprite)
    {
        Debug.Log("BeginDrag: " + index);
        DraggingIndex = index;
        dragIconUI.ShowIcon(true, iconSprite);
    }
    public void OnDrag(Vector2 ScreenPos) 
    {
        dragIconUI.UpdatePosition(ScreenPos);

        // if(Input.GetMouseButton(1) && SaveDataBuffer.Instance.Data.InventoryItems[DraggingIndex].Count >= 1)
        // {
        //     PointerEventData ped = new PointerEventData(null);
        //     ped.position = Input.mousePosition;
        //     List<RaycastResult> results = new List<RaycastResult>();
        //     m_graphicsRaycaster.Raycast(ped, results);

        //     foreach (RaycastResult result in results)
        //     {
        //         CraftTableSlot slotUI = result.gameObject.GetComponent<CraftTableSlot>();
        //         if (slotUI != null)
        //         {
        //             Debug.Log("A");
        //             slotUI.AddFromInventorySlot(SaveDataBuffer.Instance.Data.InventoryItems[DraggingIndex].ItemType, 1);
        //             SaveDataBuffer.Instance.Data.InventoryItems[DraggingIndex] = new SaveData.InventoryNode()
        //             {
        //                 ItemType = SaveDataBuffer.Instance.Data.InventoryItems[DraggingIndex].ItemType,
        //                 Count = SaveDataBuffer.Instance.Data.InventoryItems[DraggingIndex].Count - 1
        //             };
        //             break;
        //         }
        //     }

        //     SaveDataBuffer.Instance.SaveData();
        //     RefreshAll();
        // }
    }
    public void EndDrag()
    {
        Debug.Log("EndDrag"+ DraggingIndex);
        DraggingIndex = -1;
        dragIconUI.ShowIcon(false);
    }

    private List<InventorySlotUI> m_slotUIList = new List<InventorySlotUI>();

    private void Start()
    {
        InitInventoryUI();

        dragIconUI.ShowIcon(false);
    }

    private void InitInventoryUI()
    {
        int targetSlotCount = columnSize * rowSize;
        for (int i = 0; i < targetSlotCount; i++)
        {
            GameObject slotGo = Instantiate(slotPrefab, inventoryPanel.transform);
            InventorySlotUI slotUI = slotGo.GetComponent<InventorySlotUI>();
            
            slotUI.Init(i, this);
            m_slotUIList.Add(slotUI);
        }

        RefreshAll();

        m_layout_InventoryBackground.SetActive(false);
    }

    public void RefreshAll()
    {
        if (SaveDataBuffer.Instance == null) return;

        var inventoryData = SaveDataBuffer.Instance.Data.InventoryItems;
        int targetSlotCount = columnSize * rowSize;

        for (int i = 0; i < targetSlotCount; i++)
        {
            if (inventoryData != null && i < inventoryData.Length)
            {
                ItemType type = inventoryData[i].ItemType;
                int count = inventoryData[i].Count;

                var itemInfo = itemDatabase.GetItemInfo(type);
                m_slotUIList[i].UpdateSlot(itemInfo, count);
            }
            else
            {

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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SoundManager.Instance.SetActiveSoundPanel();  
        }
    }

    public void OpenInventory()
    {
        bool isActive = !m_layout_InventoryBackground.activeSelf;
        m_layout_InventoryBackground.SetActive(isActive);

        if (isActive) RefreshAll();

        MaskSlotControl.Instance.ActiveMaskSlotUIs();
        m_gameObjectt_OpenInventoryBtn.SetActive(!isActive);
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}