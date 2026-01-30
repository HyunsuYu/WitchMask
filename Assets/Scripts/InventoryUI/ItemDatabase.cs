using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    [System.Serializable]
    public struct ItemInfo
    {
        public ItemType Type;
        public Sprite Icon;
        public string Name;
    }

    public ItemInfo[] AllItems;

    private Dictionary<ItemType, ItemInfo> m_itemCache;
    // 데이터를 빠르게 찾기 위해 딕셔너리로 변환
    public void Initialize()
    {
        if (m_itemCache != null) return;
        m_itemCache = new Dictionary<ItemType, ItemInfo>();
        foreach (var item in AllItems)
        {
            if (!m_itemCache.ContainsKey(item.Type))
                m_itemCache.Add(item.Type, item);
        }
    }

    public ItemInfo GetItemInfo(ItemType type)
    {
        Initialize();
        if (m_itemCache.TryGetValue(type, out var info)) return info;
        return default; // 못 찾았을 때
    }
}