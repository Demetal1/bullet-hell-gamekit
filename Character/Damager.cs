﻿using System;
using UnityEngine;
using UnityEngine.Events;

public class Damager : MonoBehaviour
{
    [Serializable]
    public class DamagableEvent : UnityEvent<Damager, Damageable>
    { }


    [Serializable]
    public class NonDamagableEvent : UnityEvent<Damager>
    { }

    //call that from inside the onDamageableHIt or OnNonDamageableHit to get what was hit.
    public Collider2D LastHit { get { return m_LastHit; } }

    public int damage = 1;
    public float radius = 2;
    public Vector2 offset = new Vector2(1.5f, 1f);

    [Tooltip("If disabled, damager ignore trigger when casting for damage")]
    public bool canHitTriggers;
    public bool disableDamageAfterHit = false;
    [Tooltip("If set, the player will be forced to respawn to latest checkpoint in addition to loosing life")]
    public bool forceRespawn = false;
    [Tooltip("If set, an invincible damageable hit will still get the onHit message (but won't loose any life)")]
    public bool ignoreInvincibility = false;
    public LayerMask hittableLayers;
    public DamagableEvent OnDamageableHit;
    public NonDamagableEvent OnNonDamageableHit;

    protected bool m_SpriteOriginallyFlipped;
    protected bool m_CanDamage = true;
    protected ContactFilter2D m_AttackContactFilter;
    protected Collider2D[] m_AttackOverlapResults = new Collider2D[10];
    protected Transform m_DamagerTransform;
    protected Collider2D m_LastHit;

    void Awake()
    {
        m_AttackContactFilter.layerMask = hittableLayers;
        m_AttackContactFilter.useLayerMask = true;
        m_AttackContactFilter.useTriggers = canHitTriggers;

        m_DamagerTransform = transform;
    }

    public void EnableDamage()
    {
        m_CanDamage = true;
    }

    public void DisableDamage()
    {
        m_CanDamage = false;
    }

    void FixedUpdate()
    {
        if (!m_CanDamage)
            return;

        Vector2 scale = m_DamagerTransform.lossyScale;
        Vector2 colliderPosition = (Vector2)m_DamagerTransform.position + offset;
        int hitCount = Physics2D.OverlapCircle(colliderPosition, radius, m_AttackContactFilter, m_AttackOverlapResults);

        for (int i = 0; i < hitCount; i++)
        {
            m_LastHit = m_AttackOverlapResults[i];
            Damageable damageable = m_LastHit.GetComponent<Damageable>();

            if (damageable)
            {
                OnDamageableHit.Invoke(this, damageable);
                damageable.TakeDamage(this, ignoreInvincibility);
                if (disableDamageAfterHit)
                    DisableDamage();
            }
            else
            {
                OnNonDamageableHit.Invoke(this);
            }
        }
    }
}
