using UnityEngine;

using CommonUtilLib.ThreadSafe;


public sealed class CraftRecipeBuffer : SingleTonForGameObject<CraftRecipeBuffer>
{
    [SerializeField] private CraftRecipeSet m_data;


    public void Awake()
    {
        SetInstance(this);
    }
    public void OnDestroy()
    {
        Dispose();
    }

    internal CraftRecipeSet Data
    {
        get
        {
            return m_data;
        }
    }

    protected override void Dispose(bool bisDisposing)
    {
        m_data = null;
    }
}