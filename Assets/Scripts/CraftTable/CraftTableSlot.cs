using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;


public sealed class CraftTableSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Image m_image_SlotItem;
    [SerializeField] private CanvasGroup m_canvasGroup;

    private ItemType m_holdItem = ItemType.None;
    public ItemType HoldItem => m_holdItem;
    private int m_count;


    public void OnBeginDrag(PointerEventData eventData)
    {
        CraftTableControl.Instance.DraggingItem = (m_holdItem, m_count, CraftTableControl.Instance.GetSlotIndex(this));
        CraftTableControl.Instance.BIsCraftTableSlotDraging = true;

        if (m_holdItem == ItemType.None)
        {
            return;
        }

        m_image_SlotItem.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        m_canvasGroup.blocksRaycasts = true;

        InventoryController.Instance.DragIcon.ShowIcon(true, InventoryController.Instance.ItemDatabase.AllItems[(int)m_holdItem].Icon);
    }

    public void OnDrag(PointerEventData eventData)
    {
        InventoryController.Instance.OnDrag(eventData.position);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if(CraftTableControl.Instance.BIsCraftTableSlotDraging)
        {
            var craftDraggingItem = CraftTableControl.Instance.DraggingItem;
            if (m_holdItem != ItemType.None)
            {
                CraftTableControl.Instance[craftDraggingItem.fromCraftTableSlotIndex].SetFromInventorySlot(m_holdItem, m_count);
            }
            else
            {
                CraftTableControl.Instance[craftDraggingItem.fromCraftTableSlotIndex].ResetFromInventorySlot();
            }

            if (craftDraggingItem.holdItem == ItemType.None)
            {
                ResetFromInventorySlot();
            }
            else
            {
                SetFromInventorySlot(craftDraggingItem.holdItem, craftDraggingItem.count);
            }

            CraftTableControl.Instance.BIsCraftTableSlotDraging = false;
            CraftTableControl.Instance.DraggingItem = (ItemType.None, 0, -1);

            return;
        }
        else if(InventoryController.Instance.DraggingIndex == -1)
        {
            return;
        }

        var draggingItem = InventoryController.Instance.DraggingItem;
        if (m_holdItem == ItemType.None)
        {
            m_holdItem = draggingItem.holdItem;
            m_count = draggingItem.count;

            SaveDataBuffer.Instance.Data.MinusInventoryItem(InventoryController.Instance.DraggingIndex, m_count);
        }
        else if(m_holdItem != ItemType.None && m_holdItem == draggingItem.holdItem)
        {
            m_count += draggingItem.count;

            SaveDataBuffer.Instance.Data.MinusInventoryItem(InventoryController.Instance.DraggingIndex, m_count);
        }
        else if(m_holdItem != ItemType.None && m_holdItem != draggingItem.holdItem)
        {
            SaveDataBuffer.Instance.Data.InventoryItems[InventoryController.Instance.DraggingIndex].ItemType = m_holdItem;
            SaveDataBuffer.Instance.Data.InventoryItems[InventoryController.Instance.DraggingIndex].Count = m_count;
            
            m_holdItem = draggingItem.holdItem;
            m_count = draggingItem.count;
        }
        SaveDataBuffer.Instance.SaveData();

        m_image_SlotItem.sprite = InventoryController.Instance.ItemDatabase.AllItems[(int)m_holdItem].Icon;
        InventoryController.Instance.RefreshAll();

        m_image_SlotItem.color = Color.white;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        InventoryController.Instance.EndDrag();

        m_canvasGroup.blocksRaycasts = true;
        InventoryController.Instance.DragIcon.ShowIcon(false);

        if(m_image_SlotItem.sprite == null)
        {
            m_image_SlotItem.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        }
        else
        {
            m_image_SlotItem.color = Color.white;
        }
    }

    internal void SetFromInventorySlot(in ItemType itemType, in int count)
    {
        m_holdItem = itemType;
        m_count = count;

        m_image_SlotItem.color = Color.white;
        m_image_SlotItem.sprite = InventoryController.Instance.ItemDatabase.AllItems[(int)m_holdItem].Icon;
    }
    internal void AddFromInventorySlot(in ItemType itemType, in int count)
    {
        m_holdItem = itemType;
        m_count += count;

        m_image_SlotItem.color = Color.white;
        m_image_SlotItem.sprite = InventoryController.Instance.ItemDatabase.AllItems[(int)m_holdItem].Icon;
    }
    internal void ResetFromInventorySlot()
    {
        m_holdItem = ItemType.None;
        m_count = 0;

        m_image_SlotItem.sprite = null;
    }

    internal void SetImageTransparency(in float transparency)
    {
        m_image_SlotItem.color = new Color(1.0f, 1.0f, 1.0f, transparency);
    }
}