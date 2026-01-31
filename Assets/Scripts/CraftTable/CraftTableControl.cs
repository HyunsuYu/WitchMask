using System.Linq;

using UnityEngine;

using CommonUtilLib.ThreadSafe;
using UnityEngine.UI;
using TMPro;


public sealed class CraftTableControl : SingleTonForGameObject<CraftTableControl>
{
    [SerializeField] private CraftTableSlot[] m_craftTableSlots;
    [SerializeField] private CraftRecipeSet m_craftRecipeSet;
    public CraftRecipeSet CraftRecipeSet { get { return m_craftRecipeSet; } }

    private bool m_bisCraftTableSlotDraging = false;
    private (ItemType holdItem, int count, int fromCraftTableSlotIndex) m_draggingItem = (ItemType.None, 0, -1);

    [SerializeField] private Animator m_animator;

    [SerializeField] private Button m_button;
    [SerializeField] private TMP_Text m_text_button;

    [SerializeField] private Color m_color_Fail;
    [SerializeField] private Color m_color_Good;
    private bool m_bisCorrectCombination = false;


    public void Awake()
    {
        SetInstance(this);   
    }
    public void FixedUpdate()
    {
        // 1. 현재 6개 슬롯의 아이템 상태를 배열로 수집
        // m_craftTableSlots의 순서(0~5)가 레시피의 ConsumedItems 순서와 1:1 매칭됩니다.
        ItemType[] currentLayout = m_craftTableSlots.Select(slot => slot.HoldItem).ToArray();

        // 2. 레시피 데이터베이스 순회 비교
        m_bisCorrectCombination = false;
        m_button.interactable = false;
        m_text_button.color = m_color_Fail;
        foreach (var recipe in m_craftRecipeSet.Recipes)
        {
            if (CheckRecipeMatch(currentLayout, recipe.ConsumedItems))
            {
                m_bisCorrectCombination = true;
                m_button.interactable = true;
                m_text_button.color = m_color_Good;
                return;
            }
        }
    }

    internal bool BIsCraftTableSlotDraging
    {
        get
        {
            return m_bisCraftTableSlotDraging;
        }
        set
        {
            m_bisCraftTableSlotDraging = value;
        }
    }
    internal (ItemType holdItem, int count, int fromCraftTableSlotIndex) DraggingItem
    {
        get
        {
            return m_draggingItem;
        }
        set
        {
            m_draggingItem = value;
        }
    }

    #region Unity Callbacks
    public void TryCraft()
    {
        // 1. 현재 6개 슬롯의 아이템 상태를 배열로 수집
        // m_craftTableSlots의 순서(0~5)가 레시피의 ConsumedItems 순서와 1:1 매칭됩니다.
        ItemType[] currentLayout = m_craftTableSlots.Select(slot => slot.HoldItem).ToArray();

        // 2. 레시피 데이터베이스 순회 비교
        foreach (var recipe in m_craftRecipeSet.Recipes)
        {
            if (CheckRecipeMatch(currentLayout, recipe.ConsumedItems))
            {
                OnCraftSuccess(recipe);
                return;
            }
        }

        Debug.Log("일치하는 조합법이 없습니다.");
    }
    #endregion

    private bool CheckRecipeMatch(ItemType[] current, ItemType[] recipe)
    {
        // 레시피 설정이 잘못되어 6개가 아니면 무시
        if (recipe == null || recipe.Length != 6) return false;

        // SequenceEqual은 [순서, 종류, 개수]가 모두 일치해야 true를 반환합니다.
        // 예: [물, 물, 불, None, None, None] 순서가 정확해야 함
        return current.SequenceEqual(recipe);
    }

    private void OnCraftSuccess(CraftRecipeSet.RecipeData recipe)
    {
        Debug.Log($"조합 완료: {recipe.Name}");
        SoundManager.Instance.PlaySfx(SoundManager.SFX.CraftSFX00);

        // 조합 성공 후 모든 슬롯 초기화
        foreach (var slot in m_craftTableSlots)
        {
            slot.ResetFromInventorySlot();
        }

        // TODO: 결과물 아이템(recipe.ResultItem) 생성 또는 지급 로직
        m_animator.SetTrigger("Combine");
    }

    private void OnCraftFailure()
    {
        Debug.Log("조합법이 틀렸습니다.");
        // TODO: 실패 사운드나 연출
    }

    internal CraftTableSlot this[in int index]
    {
        get
        {
            return m_craftTableSlots[index];
        }
    }

    internal int GetSlotIndex(in CraftTableSlot craftTableSlot)
    {
        return m_craftTableSlots.ToList().IndexOf(craftTableSlot);
    }

    internal void FlushItems()
    {
        
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}