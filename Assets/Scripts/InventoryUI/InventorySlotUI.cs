using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private CanvasGroup canvasGroup; // 드래그 시 레이캐스트 차단용
    [SerializeField] private TMP_Text countText;
    
    private int myIndex;
    private InventoryController controller;

    public void Init(int index, InventoryController controller)
    {
        myIndex = index;
        this.controller = controller;
    }

    // 드래그 시작
    public void OnBeginDrag(PointerEventData eventData)
    {
        var node = SaveDataBuffer.Instance.Data.InventoryItems[myIndex];
        if (node.ItemType == ItemType.None) return;

        controller.BeginDrag(myIndex, iconImage.sprite);
        iconImage.color = new Color(1, 1, 1, 0.5f);
        canvasGroup.blocksRaycasts = false;
    }

    // 드래그 중
    public void OnDrag(PointerEventData eventData)
    {
        controller.OnDrag(eventData.position);
    }

    // 드롭 (이 슬롯 위에 아이템을 놓았을 때)
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop to slot: " + myIndex);
        int fromIndex = controller.DraggingIndex;
        int toIndex = myIndex;

        if (fromIndex != -1 && fromIndex != toIndex)
        {
            // [유저님이 원하신 직접 접근 방식]
            SaveData data = SaveDataBuffer.Instance.Data;
            
            // 데이터 이동/스왑 로직 실행
            data.MoveInventoryItem(fromIndex, toIndex);
            
            // 데이터 물리 저장 및 UI 전체 새로고침
            SaveDataBuffer.Instance.SaveData();
            controller.RefreshAll();
        }

        // controller.EndDrag();
    }

    // --- 드래그 끝 ---
    public void OnEndDrag(PointerEventData eventData)
    {
        controller.EndDrag();
        iconImage.color = Color.white;
        canvasGroup.blocksRaycasts = true;
    }

    
    public void UpdateSlot(ItemDatabase.ItemInfo info, int count)
    {
        // 아이템이 없으면 비우기
        if (info.Type == ItemType.None || count <= 0)
        {
            iconImage.gameObject.SetActive(false);
            countText.text = "";
            // Debug.Log("A");
            return;
        }

        // 아이템이 있으면 표시
        iconImage.gameObject.SetActive(true);
        iconImage.sprite = info.Icon;
        countText.text = count > 1 ? count.ToString() : "";
    }
}