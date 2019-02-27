﻿using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Damager))]
public class Bullet : MonoBehaviour
{
    public bool destroyWhenOutOfView = true;

    [Tooltip("If -1 never auto destroy, otherwise bullet is return to pool when that time is reached")]
    public float timeBeforeAutodestruct = -1.0f;

    [HideInInspector]
    public BulletObject bulletPoolObject;
    [HideInInspector]
    public Camera mainCamera;

    private Pattern m_CurrentPattern;
    private SpriteRenderer m_SpriteRenderer;
    private CharacterController2D m_CharacterController2D;
    private PlayerCharacter m_PlayerCharacter;
    public Vector3 m_MoveVector;
    public Vector2 m_Direction;
    public Vector2 m_PlayerDirection;
    private float m_LifeTimer;                              //Starts to count when the object is enabled

    static readonly int VFX_HASH = VFXController.StringToHash("EnemyDeath");

    //Constants
    const float k_OffScreenError = 0.01f;

    #region UnityCalls
    private void OnEnable()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_LifeTimer = 0.0f;
        m_PlayerDirection = m_PlayerCharacter.transform.position ;
    }

    private void Awake()
    {
        m_CharacterController2D = GetComponent<CharacterController2D>();
        m_PlayerCharacter = PlayerCharacter.PlayerInstance;
    }

    void FixedUpdate ()
    {
        m_LifeTimer += Time.deltaTime;
        UpdateMovement();
        m_CharacterController2D.Move(m_MoveVector);

        if (destroyWhenOutOfView)
        {
            Vector3 screenPoint = mainCamera.WorldToViewportPoint(transform.position);
            bool onScreen = screenPoint.z > 0 && screenPoint.x > -k_OffScreenError &&
                            screenPoint.x < 1 + k_OffScreenError && screenPoint.y > -k_OffScreenError &&
                            screenPoint.y < 1 + k_OffScreenError;
            if (!onScreen)
                bulletPoolObject.ReturnToPool();
        }

        if (timeBeforeAutodestruct > 0)
        {
            if (m_LifeTimer > timeBeforeAutodestruct)
            {
                bulletPoolObject.ReturnToPool();
            }
        }
    }

    #endregion

    public float GetLifeTimer()
    {
        return m_LifeTimer;
    }

    public Vector2 GetSettedDirection()
    {
        return m_Direction.normalized;
    }

    public Vector2 GetPlayerDirection()
    {
        return m_PlayerDirection;
    }

    public void SetPattern(Pattern pattern)
    {
        m_CurrentPattern = pattern;
    }

    public void SetDirection(float degree)
    {
        m_Direction = new Vector2(Mathf.Cos(degree * Mathf.Deg2Rad), Mathf.Sin(degree * Mathf.Deg2Rad));
    }

    public void ReturnToPool ()
    {
        bulletPoolObject.ReturnToPool ();
    }

    public void OnHitDamageable(Damager origin, Damageable damageable)
    {
        //VFXController.Instance.Trigger(VFX_HASH, transform.position, 0, m_SpriteRenderer.flipX, null);
    }

    public void OnHitNonDamageable(Damager origin)
    {
        //VFXController.Instance.Trigger(VFX_HASH, transform.position, 0, m_SpriteRenderer.flipX, null);
    }

    private void UpdateMovement()
    {
        m_MoveVector = m_CurrentPattern.CalculateMovement(bulletPoolObject);
    }
}
