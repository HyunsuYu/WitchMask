using UnityEngine;
using UnityEngine.Tilemaps;

using AssetKits.ParticleImage;


public sealed class PlayerControl : MonoBehaviour
{
    internal enum PlayerHeadDirection
    {
        Up,
        Down,
        Left,
        Right
    }


    [Header("Movement")]
    private Rigidbody2D m_rigidbody;
    private PlayerHeadDirection m_headDirection = PlayerHeadDirection.Down;

    [SerializeField] private float m_moveSpeed = 1.0f;

    private Animator m_animator;

    [Header("Interaction")]
    [SerializeField] private Tilemap m_tilemap_Prop;

    private ItemType m_prevInteractitemType = ItemType.None;

    [SerializeField] private float m_mouseHoldLimit = 0.75f;
    private bool m_bisInteractReady = true;
    private float m_mouseHoldTimer = 0.0f;

    [SerializeField] private RectTransform m_rectTransform_HoldBase;
    [SerializeField] private RectTransform m_rectTransform_HoldBar;

    // Shot Magic
    [SerializeField] private ParticleImage m_particleImage_ShotMagic;
    [SerializeField] private RectTransform m_rectTransform_ShowMagicTarget;

    // Return Item
    [SerializeField] private ParticleImage m_particleImage_ReturnItem;
    private RectTransform m_rectTransform_ReturnItemTarget;


    public void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();

        m_rectTransform_ReturnItemTarget = m_particleImage_ReturnItem.GetComponent<RectTransform>();
    }
    public void Update()
    {
        #region Movement
        // Start or change move direction
        if (BIsPressMoveKeyDown(PlayerHeadDirection.Up))
        {
            m_headDirection = PlayerHeadDirection.Up;
        }
        else if (BIsPressMoveKeyDown(PlayerHeadDirection.Down))
        {
            m_headDirection = PlayerHeadDirection.Down;
        }
        else if (BIsPressMoveKeyDown(PlayerHeadDirection.Left))
        {
            m_headDirection = PlayerHeadDirection.Left;
        }
        else if (BIsPressMoveKeyDown(PlayerHeadDirection.Right))
        {
            m_headDirection = PlayerHeadDirection.Right;
        }
        else
        {
            if (BIsPressMoveKey(PlayerHeadDirection.Up))
            {
                m_headDirection = PlayerHeadDirection.Up;
            }
            else if (BIsPressMoveKey(PlayerHeadDirection.Down))
            {
                m_headDirection = PlayerHeadDirection.Down;
            }
            else if (BIsPressMoveKey(PlayerHeadDirection.Left))
            {
                m_headDirection = PlayerHeadDirection.Left;
            }
            else if (BIsPressMoveKey(PlayerHeadDirection.Right))
            {
                m_headDirection = PlayerHeadDirection.Right;
            }
        }

        // Keep moving or stop
        if (m_headDirection == PlayerHeadDirection.Up && BIsPressMoveKey(PlayerHeadDirection.Up))
        {
            m_rigidbody.linearVelocity = new Vector2()
            {
                x = 0.0f,
                y = m_moveSpeed
            };
        }
        else if (m_headDirection == PlayerHeadDirection.Down && BIsPressMoveKey(PlayerHeadDirection.Down))
        {
            m_rigidbody.linearVelocity = new Vector2()
            {
                x = 0.0f,
                y = -m_moveSpeed
            };
        }
        else if (m_headDirection == PlayerHeadDirection.Left && BIsPressMoveKey(PlayerHeadDirection.Left))
        {
            m_rigidbody.linearVelocity = new Vector2()
            {
                x = -m_moveSpeed,
                y = 0.0f
            };
        }
        else if (m_headDirection == PlayerHeadDirection.Right && BIsPressMoveKey(PlayerHeadDirection.Right))
        {
            m_rigidbody.linearVelocity = new Vector2()
            {
                x = m_moveSpeed,
                y = 0.0f
            };
        }
        else
        {
            m_rigidbody.linearVelocity = Vector2.zero;
        }
        #endregion

        #region Interaction
        if(Input.GetMouseButtonDown(0) && BIsInteractionReachable())
        {
            m_bisInteractReady = false;

            m_particleImage_ShotMagic.Play();
            m_rectTransform_HoldBase.gameObject.SetActive(true);
        }
        else if(!m_bisInteractReady && Input.GetMouseButton(0) && m_mouseHoldTimer < m_mouseHoldLimit && BIsInteractionReachable())
        {
            m_mouseHoldTimer += Time.deltaTime;

            m_rectTransform_HoldBar.localScale = new Vector3()
            {
                x = m_mouseHoldTimer / m_mouseHoldLimit,
                y = 1.0f
            };

            Vector2Int curResolutionSize = SettingDataBuffer.Instance.Data.GetGetResolutionSize();
            Vector3 curMousePos = Input.mousePosition;
            m_rectTransform_ShowMagicTarget.anchoredPosition = new Vector2()
            {
                x = curMousePos.x - curResolutionSize.x / 2,
                y = curMousePos.y - curResolutionSize.y / 2
            };
            m_rectTransform_HoldBase.anchoredPosition = m_rectTransform_ShowMagicTarget.anchoredPosition;
        }
        else if(!Input.GetMouseButtonUp(0) || !BIsInteractionReachable())
        {
            m_mouseHoldTimer = 0.0f;
            m_bisInteractReady = true;
            m_particleImage_ShotMagic.Stop();
            m_rectTransform_HoldBase.gameObject.SetActive(false);
        }

        if (m_mouseHoldTimer >= m_mouseHoldLimit && BIsInteractionReachable())
        {
            ItemType curItemType = GetCurMousePosItem();
            if (curItemType != ItemType.None)
            {
                m_prevInteractitemType = curItemType;

                m_particleImage_ShotMagic.Stop();
            }
        }
        #endregion
    }

    #region Unity Callbacks
    public void HandleReturnItem()
    {
        m_rectTransform_ReturnItemTarget.anchoredPosition = m_rectTransform_ShowMagicTarget.anchoredPosition;
        m_particleImage_ReturnItem.Play();

        m_mouseHoldTimer = 0.0f;
        m_bisInteractReady = true;
        m_particleImage_ShotMagic.Stop();
        m_rectTransform_HoldBase.gameObject.SetActive(false);

        // Handle Inventory
        SaveDataBuffer.Instance.Data.AddInventoryItem(m_prevInteractitemType);
        SaveDataBuffer.Instance.SaveData();
        m_prevInteractitemType = ItemType.None;
    }
    #endregion

    #region Movement Utils
    private bool BIsPressMoveKeyDown(in PlayerHeadDirection direction)
    {
        switch (direction)
        {
            case PlayerHeadDirection.Up:
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                {
                    return true;
                }
                break;

            case PlayerHeadDirection.Down:
                if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                {
                    return true;
                }
                break;

            case PlayerHeadDirection.Left:
                if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    return true;
                }
                break;

            case PlayerHeadDirection.Right:
                if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                {
                    return true;
                }
                break;
        }

        return false;
    }
    private bool BIsPressMoveKey(in PlayerHeadDirection direction)
    {
        switch (direction)
        {
            case PlayerHeadDirection.Up:
                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                {
                    return true;
                }
                break;

            case PlayerHeadDirection.Down:
                if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                {
                    return true;
                }
                break;

            case PlayerHeadDirection.Left:
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                {
                    return true;
                }
                break;

            case PlayerHeadDirection.Right:
                if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                {
                    return true;
                }
                break;
        }

        return false;
    }
    #endregion

    #region Interaction Utils
    private bool BIsInteractionReachable()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int curPlayerPos = new Vector2Int()
        {
            x = (int)transform.position.x,
            y = (int)transform.position.y
        };

        if(Mathf.Abs((int)mouseWorldPos.x - curPlayerPos.x) > 2)
        {
            return false;
        }
        else if (Mathf.Abs((int)mouseWorldPos.y - curPlayerPos.y) > 2)
        {
            return false;
        }

        return true;
    }
    private ItemType GetCurMousePosItem()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        ItemType curItemType = TileBaseSetBuffer.Instance.Data.GetItemType(m_tilemap_Prop.GetTile(new Vector3Int()
        {
            x = (int)(mouseWorldPos.x),
            y = (int)(mouseWorldPos.y),
            z = 0
        }));

        return curItemType;
    }
    #endregion
}