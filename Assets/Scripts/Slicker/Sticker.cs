using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public sealed class Sticker : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform m_rectTransform_Front;
    [SerializeField] private RectTransform m_rectTransform_Back;
    [SerializeField] private Image m_image_Back;

    [Header("Settings")]
    [SerializeField] private Vector2 m_size = new Vector2(200, 100);
    [SerializeField] private float m_duration = 0.5f; // 애니메이션 속도
    [SerializeField] private float m_liftHeight = 30f; // 들리는 높이
    [SerializeField] private Color m_shadowColor = new Color(0.7f, 0.7f, 0.7f);

    private float m_rate = 0f;
    private Coroutine m_aniRoutine;


    public void Start()
    {
        Init();
    }

    public void PlayAttachAnimation()
    {
        gameObject.SetActive(true);
        if (m_aniRoutine != null) StopCoroutine(m_aniRoutine);
        m_aniRoutine = StartCoroutine(AnimateSticker());
    }

    internal void Init()
    {
        gameObject.SetActive(false);
    }

    private IEnumerator AnimateSticker()
    {
        float elapsed = 0f;

        while (elapsed < m_duration)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = elapsed / m_duration;

            // 가속도 효과(Easing): 뒤로 갈수록 살짝 느려지게 (선택 사항)
            m_rate = Mathf.SmoothStep(0f, 1f, normalizedTime);

            UpdateSticker();
            yield return null;
        }

        m_rate = 1f;
        UpdateSticker();
    }

    private void UpdateSticker()
    {
        if (m_rectTransform_Front == null || m_rectTransform_Back == null) return;

        float frontWidth = m_size.x * m_rate;
        float backWidth = m_size.x - frontWidth;

        // 1. 앞면: 너비 조절 (Pivot이 0, 0.5여야 함)
        m_rectTransform_Front.sizeDelta = new Vector2(frontWidth, m_size.y);

        // 2. 뒷면: 너비 및 위치 조절
        m_rectTransform_Back.sizeDelta = new Vector2(backWidth, m_size.y);
        m_rectTransform_Back.anchoredPosition = new Vector2(frontWidth, 0);
        // 뒤집힌 연출을 위해 Scale X를 -1로
        m_rectTransform_Back.localScale = new Vector3(-1, 1, 1);

        // 3. 입체감: Sin 곡선을 이용해 중앙이 들리게 함
        float lift = Mathf.Sin(m_rate * Mathf.PI) * m_liftHeight;
        
        // 뒷면과 앞면의 위치를 살짝 올려 입체감을 줌
        Vector2 liftOffset = new Vector2(0, lift);
        m_rectTransform_Back.anchoredPosition += liftOffset;
        m_rectTransform_Front.anchoredPosition = liftOffset * 0.5f;

        // 4. 뒷면 그림자: 접혀있을 때 어둡게
        if (m_image_Back != null)
        {
            m_image_Back.color = Color.Lerp(m_shadowColor, Color.white, m_rate);
        }
    }
}