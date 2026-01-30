using UnityEngine;
using UnityEngine.UI;

public class InventoryDragIcon : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void ShowIcon(bool isShow, Sprite sprite = null)
    {
        gameObject.SetActive(isShow);
        if (sprite != null) iconImage.sprite = sprite;
    }

    public void UpdatePosition(Vector2 screenPosition)
    {
        // 마우스 위치로 아이콘 이동
        rectTransform.position = screenPosition;
    }
}