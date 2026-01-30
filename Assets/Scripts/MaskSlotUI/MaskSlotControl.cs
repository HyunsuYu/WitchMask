using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using CommonUtilLib.ThreadSafe;


public sealed class MaskSlotControl : SingleTonForGameObject<MaskSlotControl>
{
    [Serializable] public struct MaskSlot
    {
        public Image Image_Mask;
        public RectTransform RectTransform_Mask;

        public Vector2 MaskOffset;

        public RectTransform RectTransform_Marker;

        public SaveData.MaskType MaskType;
    }


    private int m_movingSlotIndex = -1;

    [Header("Base")]
    [SerializeField] private GameObject m_layout_MaskSlotUI;

    [SerializeField] private Image m_image_MaskSlotMetaball;
    [SerializeField] private Image m_image_CurMaskSlot;
    private Material m_material;

    [SerializeField] private MaskSlot[] m_maskSlots;
    [SerializeField] private Sprite[] m_maskSprites;

    [Header("Move")]
    [Range(0.0f, 10.0f)]
    [SerializeField] private float m_moveSpeedScale;
    private float m_moveTimer = 0.0f;

    private bool m_bisMaskSwapStarted = false;
    private bool m_bisGoforward = false;

    [Header("Cur Selected Mask Slot")]
    [SerializeField] private RectTransform m_rectTransform_CurSelectedMaskSlotMarker;
    [SerializeField] private Vector2 m_curMaskSlotPos;
    [SerializeField] private float m_curMaskSlotMass;


    public void Awake()
    {
        SetInstance(this);

        m_material = m_image_MaskSlotMetaball.material;
        m_material.SetVector("_CurMaskSlotPos", new Vector4()
        {
            x = m_curMaskSlotPos.x,
            y = m_curMaskSlotPos.y,
            w = 0.0f,
            z = 0.0f,
        });
        m_material.SetFloat("_CurMaskSlotMass", m_curMaskSlotMass);

        for(int index = 0; index < m_maskSlots.Length; index++)
        {
            m_maskSlots[index].Image_Mask.sprite = m_maskSprites[index];
        }
    }
    public void Start()
    {
        SaveData.MaskType curSelectedMask = SaveDataBuffer.Instance.Data.CurMask;
        List<SaveData.MaskType> unusedMasks = new List<SaveData.MaskType>();
        foreach(SaveData.MaskType maskType in Enum.GetValues(typeof(SaveData.MaskType)))
        {
            if(curSelectedMask != maskType)
            {
                unusedMasks.Add(maskType);
            }
        }

        for(int index = 0; index < m_maskSlots.Length; index++)
        {
            m_maskSlots[index].MaskType = unusedMasks[index];
            m_maskSlots[index].Image_Mask.sprite = m_maskSprites[(int)unusedMasks[index]];
        }
        m_image_CurMaskSlot.sprite = m_maskSprites[(int)curSelectedMask];
    }
    public void Update()
    {
        if(m_bisMaskSwapStarted)
        {
            if(m_bisGoforward)
            {
                m_moveTimer += m_moveSpeedScale * Time.deltaTime;
            }
            else
            {
                m_moveTimer -= m_moveSpeedScale * Time.deltaTime;
            }

            m_maskSlots[m_movingSlotIndex].RectTransform_Mask.position = Vector2.Lerp(m_maskSlots[m_movingSlotIndex].RectTransform_Marker.position,
                                                                                              m_rectTransform_CurSelectedMaskSlotMarker.position, m_moveTimer);
            m_material.SetVector("_SlotPos", Vector4.Lerp(m_curMaskSlotPos, m_maskSlots[m_movingSlotIndex].MaskOffset, 1.0f - m_moveTimer));

            if(m_bisGoforward && m_moveTimer > 1.0f)
            {
                m_bisGoforward = false;

                SaveData.MaskType prevMask = SaveDataBuffer.Instance.Data.CurMask;
                SaveDataBuffer.Instance.Data = new SaveData()
                {
                    PlayerPos = SaveDataBuffer.Instance.Data.PlayerPos,
                    InventoryItems = SaveDataBuffer.Instance.Data.InventoryItems,
                    CurMask = m_maskSlots[m_movingSlotIndex].MaskType
                };
                SaveDataBuffer.Instance.SaveData();
                m_maskSlots[m_movingSlotIndex].MaskType = prevMask;

                m_maskSlots[m_movingSlotIndex].Image_Mask.sprite = m_maskSprites[(int)prevMask];
                m_image_CurMaskSlot.sprite = m_maskSprites[(int)SaveDataBuffer.Instance.Data.CurMask];
            }
            else if(!m_bisGoforward && m_moveTimer < 0.0f)
            {
                m_movingSlotIndex = -1;
                m_bisMaskSwapStarted = false;
                m_bisGoforward = true;

                m_material.SetInteger("_BIsSlotActive", 0);
            }
        }
    }

    #region Unity Callbacks
    public void SwapMask(int index)
    {
        if(m_bisMaskSwapStarted)
        {
            return;
        }

        m_movingSlotIndex = index;
        m_bisMaskSwapStarted = true;

        m_material.SetInteger("_BIsSlotActive", 1);
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new NotImplementedException();
    }
    #endregion

    internal void ActiveMaskSlotUIs()
    {
        m_layout_MaskSlotUI.SetActive(!m_layout_MaskSlotUI.activeSelf);
    }
}