using UnityEngine;


public sealed class TitleControl : MonoBehaviour
{
    [SerializeField] private GameObject m_layout_background;


    public void StartGame()
    {
        m_layout_background.SetActive(false);
    }
}