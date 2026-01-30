using UnityEngine;

using UnityEngine.Tilemaps;


/// <summary>
/// TileBaseSet에 넣는 TIleBase는 Background Tilemap에 배치되는 타일 중에서도 상호작용 시 Inventory에 넣을 수 있는 타일들만 넣어야 함
/// </summary>
[CreateAssetMenu(fileName = "TileBaseSet", menuName = "WitchMask/TileBaseSet")]
public sealed class TileBaseSet : ScriptableObject
{
    /// <summary>
    /// 각 Tilebase가 의미하는 아이템의 종류는 itemType의 각 Index와 일치
    /// </summary>
    public TileBase[] TileBases;
}

public static class TileBaseSetExtensions
{
    public static ItemType GetItemType(this TileBaseSet tileBaseSet, TileBase tileBase)
    {
        for (int index = 0; index < tileBaseSet.TileBases.Length; index++)
        {
            if (tileBaseSet.TileBases[index] == tileBase)
            {
                return (ItemType)index;
            }
        }

        return ItemType.None;
    }
}