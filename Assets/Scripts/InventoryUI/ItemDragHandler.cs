using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 아이템 드래그 위치 옮기기
/// </summary>
public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Transform originalParent;   // Slot 오브젝트
    private CanvasGroup canvasGroup;
    
    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        transform.SetParent(transform.root);
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        // Swap Slot
        InventorySlotUI dropSlot = eventData.pointerEnter?.GetComponent<InventorySlotUI>();
        if(dropSlot == null)
        {
            GameObject dropItem = eventData.pointerEnter;
            if (dropItem != null)
            {
                dropSlot = dropItem.GetComponentInParent<InventorySlotUI>();
            }
        }
        InventorySlotUI originalSlot = originalParent.GetComponent<InventorySlotUI>();

        if (dropSlot != null)
        {
            // // Slot has item -> Swap item 
            // if (dropSlot.curItem != null)
            // {
            //     dropSlot.curItem.transform.SetParent(originalSlot.transform);
            //     originalSlot.curItem = dropSlot.curItem;
            //     dropSlot.curItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            // }
            // else
            // {
            //     originalSlot.curItem = null;
            // }
            
            // // 대상 슬롯이 비어있음: 아이템을 새 슬롯으로 그냥 옮김
            // transform.SetParent(dropSlot.transform);
            // dropSlot.curItem = gameObject;

        }
        else
        {
            transform.SetParent(originalParent);
        }

        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }
}
