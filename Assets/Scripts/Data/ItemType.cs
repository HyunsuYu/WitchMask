using UnityEngine;


public enum ItemType
{
    // 지상(꽃밭)
    /// <summary>
    /// 튤립
    /// </summary>
    Flower_Red,
    /// <summary>
    /// 네모필라 멘지에시 꽃
    /// </summary>
    Flower_Blue,
    /// <summary>
    /// 황매화 
    /// </summary>
    Flower_Yellow,
    /// <summary>
    /// 코스모스
    /// </summary>
    Flower_White,
    /// <summary>
    /// 꿀
    /// </summary>
    Honey,
    /// <summary>
    /// 단단한 씨앗
    /// </summary>
    Seed,
    /// <summary>
    /// 무지개 나비 날개 - 움직이는 오브젝트
    /// </summary>
    ButterflyWing,

    // 물속
    /// <summary>
    /// 진주
    /// </summary>
    Pearl,
    /// <summary>
    /// 조개
    /// </summary>
    Seashell,
    /// <summary>
    /// 미역
    /// </summary>
    SeaWeed,
    /// <summary>
    /// 소라
    /// </summary>
    Conch,
    /// <summary>
    /// 산호 조각
    /// </summary>
    Coral,
    /// <summary>
    /// 비늘 - 움직임
    /// </summary>
    FishSkin,
    /// <summary>
    /// 물방울
    /// </summary>
    Bubble,

    // None
    None = int.MaxValue
}