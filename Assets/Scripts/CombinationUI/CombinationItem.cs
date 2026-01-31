using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombinationItem : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text itemCountText;
    
    public void SetHint(Sprite sprite, int count)
    {
        itemImage.sprite = sprite;
        itemCountText.text = count.ToString();
    }
}
