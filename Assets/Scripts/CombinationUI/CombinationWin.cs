using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

    public class CombinationWin : MonoBehaviour
    {
        [SerializeField] private TMP_Text m_titleText;
        [SerializeField] private TMP_Text m_tintText;
        [SerializeField] private Image m_resultImage;

        public void RefreshRecipe(int recipeIndex)
        {
            // 1. CraftTableControl 싱글톤을 통해 레시피 데이터 접근
            var recipeSet = CraftTableControl.Instance.CraftRecipeSet;
            m_titleText.text = recipeSet.Recipes[recipeIndex].Name;
            m_tintText.text = recipeSet.Recipes[recipeIndex].Hint;
            m_resultImage.sprite = recipeSet.Recipes[recipeIndex].Image;
            
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
}