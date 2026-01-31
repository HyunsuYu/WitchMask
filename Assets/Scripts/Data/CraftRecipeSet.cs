using System;

using UnityEngine;


[CreateAssetMenu(fileName = "CraftRecipeSet", menuName = "WitchMask/CraftRecipeSet")]
public sealed class CraftRecipeSet : ScriptableObject
{
    [Serializable] public struct RecipeData
    {
        public string Name;
        public string Hint;
        public ItemType[] ConsumedItems;
        public Sprite FurnitureSprite;
    }


    public RecipeData[] Recipes;
}