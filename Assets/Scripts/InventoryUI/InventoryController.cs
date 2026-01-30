using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    private ItemDictionary itemDictionary;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private int columnSize;
    [SerializeField] private int rowSize;
    [SerializeField] private GameObject [] itemPrefabs;


    private void Start()
    {
        itemDictionary = FindAnyObjectByType<ItemDictionary>();

        for (int i = 0; i < rowSize; i++)
        {
            for (int j = 0; j < columnSize; j++)
            {
                Slot slot = Instantiate(slotPrefab, inventoryPanel.transform).GetComponent<Slot>();
                slot.slotPos = new Vector2Int(i, j);
            
                // 아이템 불러오기
                if(i < itemPrefabs.Length)
                {
                    GameObject item = Instantiate(itemPrefabs[i], slot.transform);
                    item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    slot.curItem = item;
                }
            }

        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        }    
    }

    /// <summary>
    /// 세이브 파일에 저장
    /// </summary>
    public List<SaveData.InventoryNode> GetInventoryItems()
    {
        List<SaveData.InventoryNode> invData = new List<SaveData.InventoryNode>();
        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if(slot.curItem != null)
            {
                Item item = slot.curItem.GetComponent<Item>();
                invData.Add(new SaveData.InventoryNode {ItemType = item.ID, slotIndex = slot.slotPos});
            }
        }

        return invData;
    }
}
