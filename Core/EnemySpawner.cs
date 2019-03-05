using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class SectionEvent : UnityEvent<Section>
    { }
    
    public SectionEvent OnStopSpawning;

    [Header("Enemy Pool Settings")]
    [Tooltip("The pool of enemies")]
    public EnemyPool enemyPool;
    [Tooltip("The pattern this enemy follows")]
    public Pattern spawnPattern;

    [Header("Bullet Pool Settings")]
    [Tooltip("The pool of Bullets")]
    public BulletPool bulletPool;
    [Tooltip("The pattern this bullet follows")]
    public Pattern bulletPattern;

    [Header("Spawn Settings")]
    [Tooltip("How much enemies should spawn on this wave?")]
    public int totalEnemiesToBeSpawned;
    [Tooltip("The enemies will randomly spawn on this area")]
    public float spawnArea = 1.0f;
    [Tooltip("The delay between each enemy spawn")]
    public float spawnDelay;
    public Vector3 spawnOffset;


    protected int m_TotalSpawnedEnemyCount;
    protected int m_CurrentSpawnedEnemyCount;
    protected Section m_CurrentSection;
    protected Coroutine m_SpawnTimerCoroutine;
    protected WaitForSeconds m_SpawnWait;
    protected WaitForSeconds m_EnemyLifeTime;

    void Start()
    {
        m_CurrentSpawnedEnemyCount = 0;
        m_TotalSpawnedEnemyCount = 0;
        m_SpawnWait = new WaitForSeconds(spawnDelay);
        m_EnemyLifeTime = new WaitForSeconds(enemyPool.timeBeforeAutodestruct);

        if(SceneController.Instance.DEBUGGING)
            StartSpawn();
    }

    public void StartSpawn(Vector3 position, Section section)
    {
        transform.position = position;
        m_CurrentSection = section;
        m_TotalSpawnedEnemyCount = 0;
        if (m_SpawnTimerCoroutine == null)
            m_SpawnTimerCoroutine = StartCoroutine(SpawnTimer());
    }

    public void StartSpawn()
    {
        m_TotalSpawnedEnemyCount = 0;
        if (m_SpawnTimerCoroutine == null)
            m_SpawnTimerCoroutine = StartCoroutine(SpawnTimer());
    }

    protected IEnumerator SpawnTimer()
    {
        while (m_TotalSpawnedEnemyCount < totalEnemiesToBeSpawned)
        {
            yield return m_SpawnWait;
            if(spawnArea > 0)
                enemyPool.Pop(transform.position + transform.right * Random.Range(-spawnArea * 0.5f, spawnArea * 0.5f), spawnPattern, bulletPattern, bulletPool);
            else
                enemyPool.Pop(transform.position + spawnOffset, spawnPattern, bulletPattern, bulletPool);
            m_TotalSpawnedEnemyCount++;
        }

        if(!SceneController.Instance.DEBUGGING)
            StartCoroutine(WaitForEnemiesDespawn());
        m_SpawnTimerCoroutine = null;
    }

    protected IEnumerator WaitForEnemiesDespawn()
    {
        yield return m_EnemyLifeTime;

        if(OnStopSpawning != null)
            OnStopSpawning.Invoke(m_CurrentSection);

        m_CurrentSection = null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnArea, 0.4f, 0));
    }
}
