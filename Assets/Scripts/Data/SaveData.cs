using UnityEngine;


[System.Serializable]
public struct SaveData
{
    [System.Serializable]
    public struct InventoryNode
    {
        public ItemType ItemType;
        public int Count;
        public Vector2Int slotIndex;
    }


    /// <summary>
    /// 세이브 시점 플레이어 위치
    /// </summary>
    public Vector2Int PlayerPos;

    /// <summary>
    /// 인벤토리 아이템 목록. 크기는 반드시 40으로. 실제 UI 배치는 8 * 5로 배치
    /// </summary>
    public InventoryNode[] InventoryItems;
}

internal static class SaveDataExtension
{
    public static void AddInventoryItem(this SaveData saveData, in Vector2Int inventoryPos, in ItemType itemType, in int count = 1)
    {
        // TODO : Need to implement
    }
    public static void MinusInventoryItem(this SaveData saveData, in Vector2Int inventoryPos, in ItemType itemType, in int count = 1)
    {
        // TODO : Need to implement
    }
    public static void SwapInventoryItem(this SaveData saveData, in Vector2Int pos_0, in Vector2Int pos_1)
    {
        // TODO : Need to implement
    }
}