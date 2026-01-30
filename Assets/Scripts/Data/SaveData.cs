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
    private const int MAX_STACK = 20; // 슬롯당 최대 개수

    public static void AddInventoryItem(this SaveData saveData, in ItemType itemType, in int count = 1)
    {
        if (itemType == ItemType.None || count <= 0) return;
        int remainingCount = count;

        // 1단계: 이미 해당 아이템이 있는 슬롯 중, 여유 공간이 있는 슬롯에 먼저 채웁니다.
        for (int i = 0; i < saveData.InventoryItems.Length; i++)
        {
            if (saveData.InventoryItems[i].ItemType == itemType)
            {
                int currentCount = saveData.InventoryItems[i].Count;
                int space = MAX_STACK - currentCount;

                if (space > 0)
                {
                    int addAmount = Mathf.Min(space, remainingCount);
                    saveData.InventoryItems[i].Count += addAmount;
                    remainingCount -= addAmount;
                }
            }

            // 모두 채웠으면 종료
            if (remainingCount <= 0)
            {
                SaveDataBuffer.Instance.SaveData();
                return;
            }
        }

        // 2단계: 남은 아이템이 있다면 빈 슬롯(ItemType.None)을 찾아 채움
        for (int i = 0; i < saveData.InventoryItems.Length; i++)
        {
            if (saveData.InventoryItems[i].ItemType == ItemType.None)
            {
                int addAmount = Mathf.Min(MAX_STACK, remainingCount);
                saveData.InventoryItems[i].ItemType = itemType;
                saveData.InventoryItems[i].Count = addAmount;
                remainingCount -= addAmount;
            }

            // 모두 채웠으면 종료
            if (remainingCount <= 0)
            {
                SaveDataBuffer.Instance.SaveData();
                return;
            }
        }

        // 만약 여기까지 왔는데 remainingCount > 0 이라면 인벤토리가 꽉 찬 것
        if (remainingCount > 0)
        {
            Debug.Log($"인벤토리 공간 부족! 남은 수량: {remainingCount}");
        }
    }

    /// <summary>
    /// 특정 인덱스의 아이템 수량을 줄입니다.
    /// </summary>
    /// <returns>뺀 아이템의 종류와 실제 차감된 개수. 슬롯이 비어있으면 null</returns>
    public static (ItemType itemType, int minusCount)? MinusInventoryItem(this SaveData saveData, in int inventoryIndex, in int count = 1)
    {
        // 1. 인덱스 유효성 검사
        if (inventoryIndex < 0 || inventoryIndex >= saveData.InventoryItems.Length)
        {
            Debug.LogWarning($"유효하지 않은 인벤토리 인덱스: {inventoryIndex}");
            return null;
        }

        // 2. 해당 슬롯이 비어있는지 확인
        var node = saveData.InventoryItems[inventoryIndex];
        if (node.ItemType == ItemType.None || node.Count <= 0)
        {
            return null;
        }

        // 3. 실제 뺄 수 있는 양 계산 (가진 것보다 많이 뺄 수 없음)
        int actualMinus = Mathf.Min(node.Count, count);
        ItemType removedType = node.ItemType;

        // 4. 데이터 갱신
        saveData.InventoryItems[inventoryIndex].Count -= actualMinus;

        // 5. 개수가 0이 되면 슬롯 비우기
        if (saveData.InventoryItems[inventoryIndex].Count <= 0)
        {
            saveData.InventoryItems[inventoryIndex].ItemType = ItemType.None;
            saveData.InventoryItems[inventoryIndex].Count = 0;
        }
        
        SaveDataBuffer.Instance.SaveData();
        return (removedType, actualMinus);
    }

    /// <summary>
    /// 인벤토리의 앞쪽 슬롯부터 순회하며 해당 종류의 아이템을 차감
    /// </summary>
    public static void MinusInventoryItem(this SaveData saveData, in ItemType itemType, in int count)
    {
        int remainingToRemove = count;

        // index 0부터 정방향 순회
        for (int i = 0; i < saveData.InventoryItems.Length; i++)
        {
            // 대상 아이템을 찾았을 때
            if (saveData.InventoryItems[i].ItemType == itemType)
            {
                int currentCount = saveData.InventoryItems[i].Count;
                // 현재 슬롯에서 뺄 수 있는 최대치 계산
                int removeAmount = Mathf.Min(currentCount, remainingToRemove);

                // 데이터 차감
                saveData.InventoryItems[i].Count -= removeAmount;
                remainingToRemove -= removeAmount;

                // 개수가 0이 되면 슬롯 비우기
                if (saveData.InventoryItems[i].Count <= 0)
                {
                    saveData.InventoryItems[i].ItemType = ItemType.None;
                    saveData.InventoryItems[i].Count = 0; // 명시적 초기화
                }
            }

            // 모두 지웠다면 루프 조기 종료
            if (remainingToRemove <= 0) break;
        }

        if (remainingToRemove > 0)
        {
            Debug.LogWarning($"{itemType} 아이템이 부족합니다. 남은 차감 수량: {remainingToRemove}");
        }

        SaveDataBuffer.Instance.SaveData();
    }
    
    /// <summary>
    /// 인벤토리 아이템을 이동
    /// </summary>
   public static void MoveInventoryItem(this SaveData saveData, in int fromIndex, in int toIndex)
    {
        // 1. 인덱스 유효성 검사
        if (fromIndex == toIndex) return;
        if (fromIndex < 0 || fromIndex >= saveData.InventoryItems.Length) return;
        if (toIndex < 0 || toIndex >= saveData.InventoryItems.Length) return;

        var fromNode = saveData.InventoryItems[fromIndex];
        var toNode = saveData.InventoryItems[toIndex];

        // 출발지에 아이템이 없으면 무시
        if (fromNode.ItemType == ItemType.None) return;

        // 상황 1: 목적지가 비어있는 경우 -> 그대로 이동
        if (toNode.ItemType == ItemType.None)
        {
            saveData.InventoryItems[toIndex] = fromNode;
            saveData.InventoryItems[fromIndex] = new SaveData.InventoryNode { ItemType = ItemType.None, Count = 0 };
        }
        // 상황 2: 목적지에 같은 종류의 아이템이 있는 경우 -> 합치기(Stack)
        else if (toNode.ItemType == fromNode.ItemType)
        {
            int spaceLeft = MAX_STACK - toNode.Count;
            int amountToMove = Mathf.Min(spaceLeft, fromNode.Count);

            saveData.InventoryItems[toIndex].Count += amountToMove;
            saveData.InventoryItems[fromIndex].Count -= amountToMove;

            // 출발지 아이템을 다 옮겼다면 비우기
            if (saveData.InventoryItems[fromIndex].Count <= 0)
            {
                saveData.InventoryItems[fromIndex].ItemType = ItemType.None;
                saveData.InventoryItems[fromIndex].Count = 0;
            }
        }
        // 상황 3: 목적지에 다른 아이템이 있는 경우 -> 서로 교체(Swap)
        else
        {
            saveData.SwapInventoryItem(fromIndex, toIndex);
        }

        for (int i = 0; i < saveData.InventoryItems.Length; i++)
        {
            Debug.Log($"슬롯 {i}: {saveData.InventoryItems[i].ItemType} x {saveData.InventoryItems[i].Count}");
        }

        SaveDataBuffer.Instance.SaveData();
    }

    /// <summary>
    /// 인벤토리 아이템을 교체
    /// </summary>
    public static void SwapInventoryItem(ref this SaveData saveData, in int itemIndex_0, in int itemIndex_1)
    {
        // 유효성 검사
        if (itemIndex_0 < 0 || itemIndex_0 >= saveData.InventoryItems.Length) return;
        if (itemIndex_1 < 0 || itemIndex_1 >= saveData.InventoryItems.Length) return;

        // 데이터 교체
        var temp = saveData.InventoryItems[itemIndex_0];
        saveData.InventoryItems[itemIndex_0] = saveData.InventoryItems[itemIndex_1];
        saveData.InventoryItems[itemIndex_1] = temp;
    }
}