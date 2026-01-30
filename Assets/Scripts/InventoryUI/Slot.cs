using UnityEngine;
using UnityEngine.UI;
using TMPro;        

/// <summary>
/// 해당 슬롯 UI 관리
/// </summary> <summary>
public class Slot : MonoBehaviour
{
    public Vector2Int slotPos;
    public GameObject curItem; 

    [SerializeField] private Image iconImage;    // 슬롯의 아이콘 이미지 컴포넌트
    [SerializeField] private TextMeshProUGUI countText; // 갯수 표시 텍스트

    public void UpdateSlotUI(Sprite icon, int count)
    {
        if (count <= 0)
        {
            ClearSlot();
            return;
        }

        iconImage.sprite = icon;
        iconImage.enabled = true; // 아이콘 보이기
        
        countText.text = count > 1 ? count.ToString() : ""; // 1개면 안 보이고 2개부터 표시
    }

    public void ClearSlot()
    {
        iconImage.sprite = null;
        iconImage.enabled = false;
        countText.text = "";
    }
}