using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    [Header("References")]
    public Damager contactDamager;
    public Transform bulletSpawnPoint;

    [Header("Enemy Settings")]
    [Tooltip("The time the enemy takes to shoot")]
    public bool burstShot = false;
    public int burstBullets = 5;
    public float burstGap = .4f;
    public float shootingGap = 2f;
    public bool shouldShoot = false;

    [Header("Audio Settings")]
    public RandomAudioPlayer dieAudio;

    [Header("Misc")]
    public float flickeringDuration;

    //Hide variables, set by the pool
    [HideInInspector]
    public Enemy enemyObject;
    [HideInInspector]
    public Camera mainCamera;

    private Pattern m_CurrentEnemyPattern;
    private Pattern m_CurrentBulletPattern;
    private BulletPool m_CurrentBulletPool;
    private WaitForSeconds m_BurstBulletGap;
    private float m_NextShotTime;
    

    private CharacterController2D m_CharacterController2D;
    private Vector3 m_MoveVector;

    private SpriteRenderer m_SpriteRenderer;
    private Coroutine m_FlickeringCoroutine = null;
    private Color m_OriginalColor;

    private float m_LifeTimer;                              //Starts to count when the object is enabled
    private float m_TimeBeforeAutodestruct;
    private bool m_DestroyWhenOutOfView;
    private bool m_Dead = false;

    static readonly int VFX_HASH = VFXController.StringToHash("EnemyDeath");

    //Constants
    const float k_OffScreenError = 0.01f;

    #region UnityCalls
    private void OnEnable()
    {
        m_NextShotTime = shootingGap;
        m_LifeTimer = 0.0f;
        m_Dead = false;
    }

    private void Awake()
    {
        m_CharacterController2D = GetComponent<CharacterController2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();

        m_OriginalColor = m_SpriteRenderer.color;
    }

    private void Start()
    {
        m_BurstBulletGap = new WaitForSeconds(burstGap);
    }

    void FixedUpdate()
    {
        if(m_Dead)
            return;

        m_LifeTimer += Time.deltaTime;

        UpdateMovement();
        m_CharacterController2D.Move(m_MoveVector);

        if (m_DestroyWhenOutOfView)
        {
            Vector3 screenPoint = mainCamera.WorldToViewportPoint(transform.position);
            bool onScreen = screenPoint.z > 0 && screenPoint.x > -k_OffScreenError &&
                            screenPoint.x < 1 + k_OffScreenError && screenPoint.y > -k_OffScreenError &&
                            screenPoint.y < 1 + k_OffScreenError;
            if (!onScreen)
                enemyObject.ReturnToPool();
        }

        if (m_TimeBeforeAutodestruct > 0)
        {
            if (m_LifeTimer > m_TimeBeforeAutodestruct)
            {
                enemyObject.ReturnToPool();
            }
        }

        if(shouldShoot)
            CheckShootingTimer();
    }

    #endregion

    public float GetLifeTimer()
    {
        return m_LifeTimer;
    }

    public void SetEnemyPattern(Pattern pattern)
    {
        m_CurrentEnemyPattern = pattern;
    }

    public void SetBulletPattern(Pattern pattern)
    {
        m_CurrentBulletPattern = pattern;
    }

    public void SetBulletPool(BulletPool bulletPool)
    {
        m_CurrentBulletPool = bulletPool;
    }

    public void SetTimeToAutoDestroy(float time)
    {
        m_TimeBeforeAutodestruct = time;
    }

    public void SetLineDirection(float degree)
    {
        
    }

    public void SetDestroyWhenOutScreen(bool value)
    {
        m_DestroyWhenOutOfView = value;
    }

    public void SetMoveVector(Vector2 newMoveVector)
    {
        m_MoveVector = newMoveVector;
    }

    public void DisableDamage ()
    {
        if(contactDamager != null)
            contactDamager.DisableDamage ();
    }

    public void CheckShootingTimer()
    {
        if(m_LifeTimer >= m_NextShotTime)
        {
            m_NextShotTime = m_LifeTimer + shootingGap;
            if(burstShot)
                StartCoroutine(Burst());
            else
                Shoot();
        }
    }

    IEnumerator Burst()
    {
        int currentBullets = 0;
        while(currentBullets < burstBullets)
        {
            currentBullets++;
            Shoot();
            yield return m_BurstBulletGap;
        }
    }

    private void Shoot()
    {
        m_CurrentBulletPool.Pop(bulletSpawnPoint.position, m_CurrentBulletPattern);
    }

    public void Die(Damager damager, Damageable damageable)
    {
        //dieAudio.PlayRandomSound();
        m_Dead = true;
        VFXController.Instance.Trigger(VFX_HASH, transform.position, 0, false, null);
        CameraShaker.Shake(0.15f, 0.3f);
    }

    public void Hit(Damager damager, Damageable damageable)
    {
        if (damageable.CurrentHealth <= 0)
            return;

        if (m_FlickeringCoroutine != null)
        {
            StopCoroutine(m_FlickeringCoroutine);
            m_SpriteRenderer.color = m_OriginalColor;
        }

        m_FlickeringCoroutine = StartCoroutine(Flicker(damageable));
        CameraShaker.Shake(0.15f, 0.3f);
    }

    protected IEnumerator Flicker(Damageable damageable)
    {
        float timer = 0f;
        float sinceLastChange = 0.0f;

        Color transparent = m_OriginalColor;
        transparent.a = 0.2f;
        int state = 1;

        m_SpriteRenderer.color = transparent;

        while (timer < damageable.invulnerabilityDuration)
        {
            yield return null;
            timer += Time.deltaTime;
            sinceLastChange += Time.deltaTime;
            if(sinceLastChange > flickeringDuration)
            {
                sinceLastChange -= flickeringDuration;
                state = 1 - state;
                m_SpriteRenderer.color = state == 1 ? transparent : m_OriginalColor;
            }
        }

        m_SpriteRenderer.color = m_OriginalColor;
    }

    //Update Enemy position based on the pattern
    private void UpdateMovement()
    {
        if(m_CurrentEnemyPattern != null)
            m_MoveVector = m_CurrentEnemyPattern.CalculateMovement(enemyObject);
    }
}
