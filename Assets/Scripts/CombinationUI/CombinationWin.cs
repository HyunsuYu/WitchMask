using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombinationWin : MonoBehaviour
{
    [SerializeField] private TMP_Text TitleText;
    [SerializeField] private TMP_Text HintText;
    [SerializeField] private Image m_resultImg;
    [SerializeField] private GameObject m_ClearMark;

    private int m_recipeIndex;

    public void RefreshRecipe(int recipeIndex)
    {
        m_recipeIndex = recipeIndex;

        // 1. CraftTableControl 싱글톤을 통해 레시피 데이터 접근
        var recipeSet = CraftTableControl.Instance.CraftRecipeSet;
        TitleText.text = recipeSet.Recipes[recipeIndex].Name;
        HintText.text = recipeSet.Recipes[recipeIndex].Hint;
        m_resultImg.sprite = recipeSet.Recipes[recipeIndex].FurnitureSprite;

        if (recipeSet == null || recipeIndex >= recipeSet.Recipes.Length)
        {
            Debug.LogWarning($"Recipe index {recipeIndex} is out of range.");
            return;
        }

        var recipeData = recipeSet.Recipes[recipeIndex];

        // 2. 재료 그룹화 (ItemType별 카운트)
        var groupedMaterials = recipeData.ConsumedItems
            .Where(type => type != ItemType.None)
            .GroupBy(type => type)
            .Select(group => new { Type = group.Key, Count = group.Count() })
            .ToList();

        // 3. 자식 슬롯들 가져오기
        CombinationItem[] combinationSlots = GetComponentsInChildren<CombinationItem>(true);

        // 4. 슬롯 초기화 및 데이터 할당
        for (int i = 0; i < combinationSlots.Length; i++)
        {
            if (i < groupedMaterials.Count)
            {
                var mat = groupedMaterials[i];

                var info = InventoryController.Instance.ItemDatabase.GetItemInfo(mat.Type);

                combinationSlots[i].gameObject.SetActive(true);
                combinationSlots[i].SetHint(info.Icon, mat.Count);
            }
            else
            {
                combinationSlots[i].gameObject.SetActive(false);
            }
        }
    }
    public void FixedUpdate()
    {
        // Celar Mark
        if (m_recipeIndex <= SaveDataBuffer.Instance.Data.CompletedCraftItemIndex)
        {
            m_ClearMark.SetActive(true);
        }
        else
        {
            m_ClearMark?.SetActive(false);
        }
    }
}