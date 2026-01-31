using UnityEngine;

public class CombinationCreater : MonoBehaviour
{
    [SerializeField] private GameObject windowPrefab;

    private void Start()
    {
        int createCount = CraftTableControl.Instance.CraftRecipeSet.Recipes.Length;
        for (int i = 0; i < createCount; i++)
        {
            GameObject windowObj = Instantiate(windowPrefab, transform);
            windowObj.GetComponent<CombinationWin>().RefreshRecipe(i);
        }
    }
}
