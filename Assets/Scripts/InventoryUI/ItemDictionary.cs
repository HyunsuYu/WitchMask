using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 아이템 프리펩 저장
/// </summary> <summary>
public class ItemDictionary : MonoBehaviour
{
    public List<Item> ItemPrefabs;
    private Dictionary<ItemType, GameObject> itemDictionaty;

    private void Awake()
    {
        itemDictionaty = new Dictionary<ItemType, GameObject>();

        for (int i = 0; i < ItemPrefabs.Count; i++)
        {
            if (ItemPrefabs[i] != null)
            {
                ItemPrefabs[i].ID = (ItemType)i;
            }
        }

        foreach (Item item in ItemPrefabs)
        {
            itemDictionaty[item.ID] = item.gameObject;
        }
    }

    public GameObject GetItemPrefab(ItemType itemID)
    {
        itemDictionaty.TryGetValue(itemID, out GameObject prefab);

        if (prefab == null)
        {
            Debug.LogError($"{itemID}가 딕셔너리 내에 없습니다.");
        }

        return prefab;
    }
}
