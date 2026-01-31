using UnityEngine;
using UnityEngine.UI;


public sealed class Sticker : MonoBehaviour
{
    [SerializeField] private RectTransform m_rectTrnasform_Front;
    [SerializeField] private RectTransform m_rectTrasnform_Back;

    [SerializeField] private Vector2Int m_size;
    [SerializeField] private float m_rate;


    public void Awake()
    {
        m_rectTrasnform_Back.sizeDelta = m_size;
        m_rectTrnasform_Front.sizeDelta = m_size;
    }
    public void FixedUpdate()
    {
        m_rectTrnasform_Front.sizeDelta = new Vector2()
        {
            x = m_size.x * m_rate,
            y = m_size.y
        };
        m_rectTrasnform_Back.sizeDelta = new Vector2()
        {
            x = m_size.x - m_size.x * m_rate,
            y = m_size.y
        };
        m_rectTrasnform_Back.anchoredPosition = new Vector2()
        {
            x = m_size.x - m_rectTrasnform_Back.sizeDelta.x,
            y = 0.0f
        };
    }
}