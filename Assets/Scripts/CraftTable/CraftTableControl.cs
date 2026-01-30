using System.Linq;

using UnityEngine;

using CommonUtilLib.ThreadSafe;


public sealed class CraftTableControl : SingleTonForGameObject<CraftTableControl>
{
    [SerializeField] private CraftTableSlot[] m_craftTableSlots;

    private bool m_bisCraftTableSlotDraging = false;
    private (ItemType holdItem, int count, int fromCraftTableSlotIndex) m_draggingItem = (ItemType.None, 0, -1);


    public void Awake()
    {
        SetInstance(this);   
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

    }
    #endregion

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

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}