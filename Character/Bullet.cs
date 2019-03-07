using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Damager))]
public class Bullet : MonoBehaviour
{
    //Constants
    const float k_OffScreenError = 0.01f;

    public SpriteRenderer spriteRenderer;

    public bool destroyWhenOutOfView = true;
    [Tooltip("If -1 never auto destroy, otherwise bullet is return to pool when that time is reached")]
    public float timeBeforeAutodestruct = -1.0f;

    [HideInInspector]
    public BulletObject bulletPoolObject;
    [HideInInspector]
    public Camera mainCamera;

    private Pattern m_CurrentPattern;
    private CharacterController2D m_CharacterController2D;
    private PlayerCharacter m_PlayerCharacter;
    private Vector3 m_MoveVector;
    private Vector2 m_Direction;
    private Vector2 m_TargetPosition;
    private Vector3 m_OriginPosition;
    private float m_RotationVelocity;

    static readonly int VFX_HASH = VFXController.StringToHash("EnemyDeath");

    public float LifeTime {get; set;}    //Starts to count when the object is enabled

    #region UnityCalls
    private void OnEnable()
    {
        LifeTime = 0.0f;
        m_MoveVector = Vector2.zero;
    }

    private void Awake()
    {
        m_CharacterController2D = GetComponent<CharacterController2D>();
        m_PlayerCharacter = PlayerCharacter.PlayerInstance;
    }

    void FixedUpdate ()
    {
        LifeTime += Time.deltaTime;

        CalculateMovement();

        m_CharacterController2D.Move(m_MoveVector);

        if (destroyWhenOutOfView)
        {
            Vector3 screenPoint = mainCamera.WorldToViewportPoint(transform.position);
            bool onScreen = screenPoint.z > 0 &&
                            screenPoint.y > -k_OffScreenError &&
                            screenPoint.y < 1 + k_OffScreenError;
            if (!onScreen)
                bulletPoolObject.ReturnToPool();
        }

        if (timeBeforeAutodestruct > 0)
        {
            if (LifeTime > timeBeforeAutodestruct)
            {
                bulletPoolObject.ReturnToPool();
            }
        }
    }

    #endregion

    public Vector2 GetDirection()
    {
        return m_Direction;
    }

    public void SetDegreeDirection(float degree)
    {
        m_Direction = new Vector2(Mathf.Cos(degree * Mathf.Deg2Rad), Mathf.Sin(degree * Mathf.Deg2Rad));
    }

    public void SetTargetDirection(Vector3 target)
    {
        m_Direction = target - transform.position;
    }

    public void SetOriginTarget()
    {
        m_Direction = transform.position - m_OriginPosition;
    }

    //non-normalized means that the value will increase as far as the target is
    //and a normalized value will only express the direction no matter the distance
    public void SetPlayerAsTarget(bool normalize = false)
    {
        if(m_CurrentPattern.normalizedSpeed)
            m_Direction = (m_PlayerCharacter.transform.position - transform.position).normalized;
        else
            m_Direction = m_PlayerCharacter.transform.position - transform.position;
    }

    //updates the sprite rotation
    public void UpdateRendererRotation()
    {
        spriteRenderer.transform.up = m_Direction;
    }

    //Called by the Damage Events
    public void OnHitDamageable(Damager origin, Damageable damageable)
    {
        //VFXController.Instance.Trigger(VFX_HASH, transform.position, 0, m_SpriteRenderer.flipX, null);
    }

    public void OnHitNonDamageable(Damager origin)
    {
        //VFXController.Instance.Trigger(VFX_HASH, transform.position, 0, m_SpriteRenderer.flipX, null);
    }

    //Pool methods
    public void SetPattern(Pattern pattern)
    {
        m_CurrentPattern = pattern;
    }

    //subtracts the bullet spawn position with the shooter position to find out which direction he shooted
    public void SetOriginPosition(Vector3 origin)
    {
        m_OriginPosition = origin;
    }

    public void InitializePattern()
    {
        m_CurrentPattern.InitializePattern(bulletPoolObject);
    }

    public void ReturnToPool ()
    {
        bulletPoolObject.ReturnToPool ();
    }

    //Calculations based on the pattern
    private void CalculateMovement()
    {
        m_MoveVector = m_CurrentPattern.CalculateMovement(bulletPoolObject);
    }
}
