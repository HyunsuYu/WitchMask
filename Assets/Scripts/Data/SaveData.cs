using UnityEngine;


[System.Serializable]
public struct SaveData
{
    public enum MaskType
    {
        HoneyBee,
        Deer,
        Fish,
        Mole
    }

    [System.Serializable]
    public struct InventoryNode
    {
        public ItemType ItemType;
        public int Count;
    }


    /// <summary>
    /// 세이브 시점 플레이어 위치
    /// </summary>
    public Vector2Int PlayerPos;

    /// <summary>
    /// 인벤토리 아이템 목록. 크기는 반드시 40으로. 실제 UI 배치는 8 * 5로 배치
    /// </summary>
    public InventoryNode[] InventoryItems;

    // TODO : SaveDataBuffer에 반영해야함
    public MaskType CurMask;
}

internal static class SaveDataExtension
{
    public static void AddInventoryItem(this SaveData saveData, in ItemType itemType, in int count = 1)
    {
        // TODO : Need to implement
    }
    /// <summary>
    /// 만약 inventoryIndex에 있는 아이템이 ItemType.None이면 null return
    /// </summary>
    /// <param name="saveData"></param>
    /// <param name="inventoryIndex"></param>
    /// <param name="count"></param>
    /// <returns></returns> <summary>
    /// 
    /// </summary>
    /// <param name="itemType"></param>
    /// <param name="saveData"></param>
    /// <param name="inventoryIndex"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static (ItemType itemType, int minusCount)? MinusInventoryItem(this SaveData saveData, in int inventoryIndex, in int count = 1)
    {

        
        return (ItemType.None, count);
    }
    public static void MoveInventoryItem(this SaveData saveData, in int fromIndex, in int toIndex)
    {
        // TODO : Need to implement
    }
    public static void SwapInventoryItem(this SaveData saveData, in int itemIndex_0, in int itemIndex_1)
    {
        // TODO : Need to implement
    }
}