using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;


public sealed class InteractableObject : MonoBehaviour
{
    private SpriteRenderer m_spriteRenderer;

    [SerializeField] private SpriteRenderer m_spriteRenderer_Star;

    [SerializeField] private Tilemap m_targetTilemap;
    [SerializeField] private float m_repealTime = 10.0f;
    private bool m_bisUsed = false;

    private Vector3Int m_tilePos;
    [SerializeField] private TileBase m_curPosTileBase;


    public void Awake()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();

        Vector3 mouseWorldPos = transform.position;
        m_tilePos = new Vector3Int()
        {
            x = (int)(mouseWorldPos.x),
            y = (int)(mouseWorldPos.y),
            z = 0
        };
        m_curPosTileBase = m_targetTilemap.GetTile(new Vector3Int()
        {
            x = m_tilePos.x,
            y = m_tilePos.y,
            z = 0
        });
    }
    public void Update()
    {
        if(!m_bisUsed)
        {

        }
    }

    #region Unity Callbacks
    public void CallWhenPlayerItemReturn()
    {
        Vector3 curMouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int curPos = new Vector3Int()
        {
            x = (int)(curMouseWorldPos.x),
            y = (int)(curMouseWorldPos.y),
            z = 0
        };

        if(curPos == m_tilePos)
        {
            m_bisUsed = true;
            m_targetTilemap.SetTile(m_tilePos, null);
            m_spriteRenderer.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
            m_spriteRenderer_Star.enabled = false;
            Invoke(nameof(RepealItem), m_repealTime);
        }
    }
    #endregion

    private void RepealItem()
    {
        m_spriteRenderer.color = Color.white;
        m_spriteRenderer_Star.enabled = true;
        m_targetTilemap.SetTile(m_tilePos, m_curPosTileBase);
        m_bisUsed = false;
    }
}