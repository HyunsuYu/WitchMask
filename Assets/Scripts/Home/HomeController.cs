using CommonUtilLib.ThreadSafe;
using System;
using UnityEngine;


public class HomeController : SingleTonForGameObject<HomeController>
{
    [SerializeField] private GameObject m_Homelayout;
    [SerializeField] private GameObject[] m_gameObject_OpenHomeBtns;

    [SerializeField] private Sticker[] m_waterStickers;
    [SerializeField] private Sticker[] m_treeSrickers;
    private int m_curStickerIndex = 0;

    [SerializeField] private float m_delay = 1.0f;
    private float m_timer = 0.0f;

    private AudioSource m_audioSource;

    private bool m_bisEndPlay = false;


    public void Awake()
    {
        SetInstance(this);

        m_audioSource = GetComponent<AudioSource>();
    }
    public void Start()
    {
        UpdateOpenBtns();
    }
    public void Update()
    {
        if(BisOpened && !m_bisEndPlay)
        {
            m_timer += Time.deltaTime;

            if(m_timer >= m_delay)
            {
                bool bisPlay = false;
                int completedFurnitureIndex = SaveDataBuffer.Instance.Data.CompletedCraftItemIndex;
                if (m_curStickerIndex < m_waterStickers.Length && m_curStickerIndex <= completedFurnitureIndex)
                {
                    m_waterStickers[m_curStickerIndex].PlayAttachAnimation();
                    bisPlay = true;
                }
                if (m_curStickerIndex < m_treeSrickers.Length && m_curStickerIndex + 10 <= completedFurnitureIndex)
                {
                    m_treeSrickers[m_curStickerIndex].PlayAttachAnimation();
                    bisPlay = true;
                }

                m_audioSource.Play(20000);

                if(m_curStickerIndex == 5)
                {
                    m_curStickerIndex++;
                    m_waterStickers[m_curStickerIndex].PlayAttachAnimation();
                }

                m_timer = 0.0f;
                m_curStickerIndex++;

                if(!bisPlay)
                {
                    m_bisEndPlay = true;
                }
            }
        }
    }

    internal bool BisOpened
    {
        get
        {
            if(m_Homelayout == null)
            {
                return false;
            }

            return m_Homelayout.activeSelf;
        }
    }

    #region Unity Callbacks
    public void OpenHome()
    {
        m_Homelayout.SetActive(!m_Homelayout.activeSelf);
        UpdateOpenBtns();

        if (BisOpened)
        {
            Invoke(nameof(ResetSrickers), 0.1f);

            m_curStickerIndex = 0;
            m_timer = 0.0f;
            m_bisEndPlay = false;
        }
    }
    #endregion

    internal void UpdateOpenBtns()
    {
        for (int index = 0; index < Enum.GetValues(typeof(SaveData.MaskType)).Length; index++)
        {
            if (m_gameObject_OpenHomeBtns[index] == null)
            {
                continue;
            }

            m_gameObject_OpenHomeBtns[index].SetActive(false);
            if ((int)SaveDataBuffer.Instance.Data.CurMask == index)
            {
                m_gameObject_OpenHomeBtns[index].SetActive(!BisOpened);
            }
        }
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }

    private void ResetSrickers()
    {
        foreach (var sticker in m_waterStickers)
        {
            sticker.Init();
        }
        foreach (var sticker in m_treeSrickers)
        {
            sticker.Init();
        }
    }
}
