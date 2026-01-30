using UnityEngine;

using CommonUtilLib.ThreadSafe;


public sealed class TileBaseSetBuffer : SingleTonForGameObject<TileBaseSetBuffer>
{
    [SerializeField] private TileBaseSet m_data;


    public void Awake()
    {
        SetInstance(this);
    }
    public void OnDestroy()
    {
        Dispose();
    }

    internal TileBaseSet Data
    {
        get
        {
            if(m_data == null)
            {
                throw new System.Exception("Data is now null");
            }

            return m_data;
        }
    }

    protected override void Dispose(bool bisDisposing)
    {
        m_data = null;
    }
}