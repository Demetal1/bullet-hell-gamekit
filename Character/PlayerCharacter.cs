using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    #region Singleton
    static protected PlayerCharacter s_PlayerInstance;
    static public PlayerCharacter PlayerInstance { get { return s_PlayerInstance; } }
    #endregion

    //References
    public SpriteRenderer spriteRenderer;
    public SpriteRenderer hitboxRenderer;
    public Damageable damageable;
    
    //Hurt
    public float flickeringDuration = 0.1f;
    public Transform respawnLocation;

    //Speed
    public float runSpeed = 5;
    public float walkSpeed = 3;
    public float dashSpeed = 5f;
    public float dashPressTime = .5f;

    //Shoot Settings
    public float shotsPerSecond = 1f;
    
    public Transform bulletSpawnPointLeft;
    public Transform bulletSpawnPointMiddle;
    public Transform bulletSpawnPointRight;
    public BulletPool bulletPool;
    public BulletPool bulletPool2;
    public BulletPool bulletPool3;
    public Pattern rightPattern;
    public Pattern middlePattern;
    public Pattern leftPattern;

    //Audio Settings
    public RandomAudioPlayer hurtAudioPlayer;
    public RandomAudioPlayer shootAudioPlayer;
    public RandomAudioPlayer dashAudioPlayer;

    //Particle Setings
    public ParticleSystem deathParticle;
    public ParticleSystem spawnParticle;

    //Misc Settings
    public bool spriteOriginallyFacesLeft;

    //Animator Hashs
    private readonly int m_HashHorizontalSpeedPara = Animator.StringToHash("HorizontalSpeed");
    private readonly int m_HashVerticalSpeedPara = Animator.StringToHash("VerticalSpeed");

    private bool m_ReadInput;
    private bool m_CanMove;
    private bool m_IsWalking;
    private Vector2 m_MoveVector;

    private Vector2 m_InputVector;
    private CharacterController2D m_CharacterController2D;
    private Animator m_Animator;
    private WaitForSeconds m_ShotSpawnWait;
    private Coroutine m_ShootingCoroutine;
    private WaitForSeconds m_FlickeringWait;
    private Coroutine m_FlickerCoroutine;

    private float m_ShotSpawnGap;
    private float m_NextShotTime;
    private float m_ShotTimer;
    private bool m_IsShooting;
    private bool m_StoppedShooting;
    private bool m_IsFiring;

    private bool m_DashFirstPress = true;
    private bool m_ResetDash;
    private float m_DashFirstPressTimer;

    private void Awake()
    {
        s_PlayerInstance = this;

        m_ReadInput = true;
        m_CanMove = true;
        m_CharacterController2D = GetComponent<CharacterController2D>();
        m_Animator = GetComponent<Animator>();
    }

    private void Start() 
    {
        m_ShotSpawnGap = 1f / shotsPerSecond;
        m_NextShotTime = Time.time;
        m_ShotSpawnWait = new WaitForSeconds(m_ShotSpawnGap);
        m_FlickeringWait = new WaitForSeconds(flickeringDuration);
    }

    private void Update() 
    {
        m_InputVector.x = Input.GetAxisRaw("Horizontal");
        m_InputVector.y = Input.GetAxisRaw("Vertical");
        m_IsWalking = Input.GetButton("Walk");
        m_IsShooting = Input.GetButton("Shoot");
        m_StoppedShooting = Input.GetKeyUp(KeyCode.Z);

        if(Input.GetKeyDown(KeyCode.RightArrow)) 
            CheckForDash();
        else if(Input.GetKeyDown(KeyCode.LeftArrow))
            CheckForDash();

        if(m_ResetDash) 
        {
            m_DashFirstPress = false;
            m_ResetDash = false;
        }

        CheckAndFireGun();
    }

    public void CheckForDash()
    {
        if(m_DashFirstPress)
        {
            if(Time.time - m_DashFirstPressTimer < dashPressTime)
                Dash();

            m_ResetDash = true;
        }
        else
        {
            m_DashFirstPress = true;
            m_DashFirstPressTimer = Time.time;
        }
    }


    void FixedUpdate()
    {
        CalculateMovement();
        m_Animator.SetFloat(m_HashHorizontalSpeedPara, m_MoveVector.x);
        m_Animator.SetFloat(m_HashVerticalSpeedPara, m_MoveVector.y);

        UpdateFacing();
    }

    public void SetHorizontalMovement(float newHorizontalMovement)
    {
        m_MoveVector.x = newHorizontalMovement;
    }

    public void SetVerticalMovement(float newVerticalMovement)
    {
        m_MoveVector.y = newVerticalMovement;
    }

    public void IncrementMovement(Vector2 additionalMovement)
    {
        m_MoveVector += additionalMovement;
    }

    public void IncrementHorizontalMovement(float additionalHorizontalMovement)
    {
        m_MoveVector.x += additionalHorizontalMovement;
    }

    public void IncrementVerticalMovement(float additionalVerticalMovement)
    {
        m_MoveVector.y += additionalVerticalMovement;
    }

    private void CalculateMovement()
    {
        if(!m_CanMove || !m_ReadInput)
            return;

        if(m_IsWalking)
        {
            m_MoveVector = m_InputVector.normalized * walkSpeed;
            hitboxRenderer.enabled = true;
        }
        else
        {
            m_MoveVector = m_InputVector.normalized * runSpeed;
            hitboxRenderer.enabled = false;
        }

        m_CharacterController2D.Move(m_MoveVector * Time.deltaTime);
    }

    public void SetReadInput(bool value)
    {
        m_ReadInput = value;
    }

    public void OnHurt(Damager damager, Damageable damageable)
    {
        if (!m_CanMove)
            return;

        StartCoroutine(DieCoroutine()); 
    }

    IEnumerator DieCoroutine()
    {
        m_ReadInput = false;
        m_StoppedShooting = true;
        VerifyShootRoutine();
        deathParticle.Play();

        spriteRenderer.enabled = false;
        hitboxRenderer.enabled = false;
        gameObject.layer = 13;

        damageable.EnableInvulnerability(true);
        yield return new WaitForSeconds(2.0f); //wait one second before respawing
        spawnParticle.Play();
        GameObjectTeleporter.Teleport(gameObject, respawnLocation.position);

        spriteRenderer.enabled = true;

        damageable.EnableInvulnerability();
        StartFlickering();
        m_ReadInput = true;
        yield return new WaitForEndOfFrame();
    }

    /*public void OnDie()
    {
        StartCoroutine(DieRespawnCoroutine());
    }

    IEnumerator DieRespawnCoroutine(bool resetHealth, bool useCheckPoint)
    {
        yield return new WaitForSeconds(1.0f); //wait one second before respawing
        Respawn(resetHealth, useCheckPoint);
        yield return new WaitForEndOfFrame();
    }*/

    public void Respawn()
    {
        //we reset the hurt trigger, as we don't want the player to go back to hurt animation once respawned
        //m_Animator.ResetTrigger(m_HashHurtPara);
        if (m_FlickerCoroutine != null)
        {//we stop flcikering for the same reason
            StopFlickering();
        }

        //_Animator.SetTrigger(m_HashRespawnPara);

        m_CharacterController2D.Teleport(respawnLocation.position);
    }

    public void StartFlickering()
    {
        m_FlickerCoroutine = StartCoroutine(Flicker());
    }

    public void StopFlickering()
    {
        StopCoroutine(m_FlickerCoroutine);
        spriteRenderer.enabled = true;
    }

    protected IEnumerator Flicker()
    {
        float timer = 0f;

        while (timer < damageable.invulnerabilityDuration)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return m_FlickeringWait;
            timer += flickeringDuration;
        }

        spriteRenderer.enabled = true;
        gameObject.layer = 11;
    }

    public void UpdateFacing()
    {
        bool faceLeft = m_MoveVector.x < 0f;
        bool faceRight = m_MoveVector.x > 0f;

        if (faceLeft)
        {
            spriteRenderer.flipX = !spriteOriginallyFacesLeft;
        }
        else if (faceRight)
        {
            spriteRenderer.flipX = spriteOriginallyFacesLeft;
        }
    }

    public void UpdateFacing(bool faceLeft)
    {
        if (faceLeft)
        {
            spriteRenderer.flipX = !spriteOriginallyFacesLeft;
        }
        else
        {
            spriteRenderer.flipX = spriteOriginallyFacesLeft;
        }
    }

    public void Dash()
    {
        Debug.Log("Dashing");
        SetHorizontalMovement(dashSpeed);
    }

    public void CheckAndFireGun()
    {
        if(!m_ReadInput)
            return;

        if (m_IsShooting)
        {
            if (m_ShootingCoroutine == null)
                m_ShootingCoroutine = StartCoroutine(Shoot());
        }

        VerifyShootRoutine();
    }

    public void VerifyShootRoutine()
    {
        if (m_StoppedShooting && m_ShootingCoroutine != null)
        {
            StopCoroutine(m_ShootingCoroutine);
            m_ShootingCoroutine = null;
        }
    }

    private IEnumerator Shoot()
    {
        while (m_IsShooting)
        {
            if (Time.time >= m_NextShotTime)
            {
                SpawnBullet();
                m_NextShotTime = Time.time + m_ShotSpawnGap;
            }
            yield return null;
        }
    }

    protected void SpawnBullet()
    {
        /* if(bulletPool != null)
            bulletPool.Pop(bulletSpawnPointLeft.position, leftPattern);
        if(bulletPool2 != null)
            bulletPool2.Pop(bulletSpawnPointMiddle.position, middlePattern);
        if(bulletPool3 != null)
            bulletPool3.Pop(bulletSpawnPointRight.position, rightPattern); */
        //rangedAttackAudioPlayer.PlayRandomSound();
    }
}
