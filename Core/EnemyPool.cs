using System.Collections;
using UnityEngine;

public class EnemyPool : ObjectPool<EnemyPool, Enemy, Vector2, Pattern, Pattern, BulletPool>
{
    [Header("Pool Settings")]
    public bool destroyWhenOutOfView = true;
    [Tooltip("If -1 never auto destroy, otherwise bullet is return to pool when that time is reached")]
    public float timeBeforeAutodestruct = -1.0f;

    public float removalDelay = .5f;

    void Start()
    {
        for (int i = 0; i < initialPoolCount; i++)
        {
            Enemy newEnemy = CreateNewPoolObject();
            pool.Add(newEnemy);
        }
    }

    public override void Push(Enemy poolObject)
    {
        poolObject.inPool = true;
        poolObject.Sleep();
    }
}

public class Enemy : PoolObject<EnemyPool, Enemy, Vector2, Pattern, Pattern, BulletPool>
{
    public Damageable damageable;
    public EnemyBehaviour enemyBehaviour;

    protected WaitForSeconds m_RemoveWait;

    protected override void SetReferences()
    {
        damageable = instance.GetComponent<Damageable>();
        enemyBehaviour = instance.GetComponent<EnemyBehaviour>();
        enemyBehaviour.SetTimeToAutoDestroy(objectPool.timeBeforeAutodestruct);
        enemyBehaviour.SetDestroyWhenOutScreen(objectPool.destroyWhenOutOfView);
        enemyBehaviour.enemyObject = this;
        enemyBehaviour.mainCamera = Object.FindObjectOfType<Camera> ();

        damageable.OnDie.AddListener(ReturnToPoolEvent);

        m_RemoveWait = new WaitForSeconds(objectPool.removalDelay);
    }

    public override void WakeUp(Vector2 info, Pattern enemyPattern, Pattern bulletPattern, BulletPool bulletPool)
    {
        enemyBehaviour.SetMoveVector(Vector2.zero);
        enemyBehaviour.SetEnemyPattern(enemyPattern);
        enemyBehaviour.SetBulletPattern(bulletPattern);
        enemyBehaviour.SetBulletPool(bulletPool);
        instance.transform.position = info;
        instance.SetActive(true);
        damageable.SetHealth(damageable.startingHealth);
        damageable.DisableInvulnerability();
        enemyBehaviour.contactDamager.EnableDamage();
    }

    public override void Sleep()
    {
        instance.SetActive(false);
        damageable.EnableInvulnerability();
    }

    protected void ReturnToPoolEvent(Damager dmgr, Damageable dmgbl)
    {
        objectPool.StartCoroutine(ReturnToPoolAfterDelay());
    }

    protected IEnumerator ReturnToPoolAfterDelay()
    {
        yield return m_RemoveWait;
        ReturnToPool();
    }
}
