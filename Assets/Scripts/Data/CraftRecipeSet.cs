using System;

using UnityEngine;


[CreateAssetMenu(fileName = "CraftRecipeSet", menuName = "WitchMask/CraftRecipeSet")]
public sealed class CraftRecipeSet : ScriptableObject
{
    [Serializable] public struct RecipeData
    {
        public ItemType[] ConsumedItems;
    }


    public RecipeData[] Recipes;
}