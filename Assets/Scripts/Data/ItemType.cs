using UnityEngine;


public enum ItemType
{
    // 지상(꽃밭)
    /// <summary>
    /// 튤립
    /// </summary>
    Flower_Red,
    /// <summary>
    /// 수국
    /// </summary>
    Flower_Blue,
    /// <summary>
    /// 민들레
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

    // 지상(숲)
    /// <summary>
    /// 나뭇잎
    /// </summary>
    FreshLeaf,
    /// <summary>
    /// 반딧불이
    /// </summary>
    Firefly,
    /// <summary>
    /// 버섯
    /// </summary>
    Mushroom,
    /// <summary>
    /// 마른 잔가지
    /// </summary>
    TreeBranch,
    /// <summary>
    /// 튼튼한 나무
    /// </summary>
    Tree,
    /// <summary>
    /// 돌
    /// </summary>
    Stone,
    /// <summary>
    /// 깃털
    /// </summary>
    Feather,
    /// <summary>
    /// 짐승의 털
    /// </summary>
    BeastHair,

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

    // 땅속
    /// <summary>
    /// 수정 파편
    /// </summary>
    Cristal,
    /// <summary>
    /// 거미줄
    /// </summary>
    Web,
    /// <summary>
    /// 박쥐 날개
    /// </summary>
    BetWing,
    /// <summary>
    /// 검은 석탄
    /// </summary>
    Coal,
    /// <summary>
    /// 발광 이끼
    /// </summary>
    Moss,
    /// <summary>
    /// 철광석
    /// </summary>
    Ironstone,
    /// <summary>
    /// 진흙
    /// </summary>
    Mud,

    // None
    None = int.MaxValue
}