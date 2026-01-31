using UnityEngine;


public sealed class MapObjectSortOrder : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_spriteRenderer;

    [SerializeField] private float m_targetYPos;
    [SerializeField] private Transform m_transform_Player;

    [SerializeField] private int ObjectUpperOrder;
    [SerializeField] private int ObjectLowerOrder;


    public void FixedUpdate()
    {
        if(m_transform_Player.position.y >= m_targetYPos)
        {
            m_spriteRenderer.sortingOrder = ObjectUpperOrder;
        }
        else
        {
            m_spriteRenderer.sortingOrder = ObjectLowerOrder;
        }
    }
}