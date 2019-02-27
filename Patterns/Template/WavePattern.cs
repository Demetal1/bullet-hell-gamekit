using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Patterns/Wave Pattern")]
public class WavePattern : Pattern
{
    [ReadOnly]
    public PatternType type = PatternType.Wave;

    public float frequency = 10f;
    public float magnitude = 0.5f;
    public float speed = 2f;

    public override Vector2 CalculateMovement(Enemy enemy)
    {
        return Wave(enemy);
    }

    public override Vector2 CalculateMovement(BulletObject bullet)
    {
        return Wave(bullet);
    }

    public Vector2 Wave(Enemy enemy)
    {
        Vector2 direction = -enemy.enemyBehaviour.transform.up * Time.deltaTime * speed;
        //m_MoveVector = enemy.enemyBehaviour.transform.right * Mathf.Sin(enemy.enemyBehaviour.GetLifeTimer() * frequency) * magnitude;
        //m_MoveVector += direction;
        return Vector2.zero;
    }

    public Vector2 Wave(BulletObject bulletObject)
    {
        Vector2 direction = -bulletObject.transform.up * Time.deltaTime * speed;
        //m_MoveVector = bulletObject.transform.right * Mathf.Sin(bulletObject.bullet.GetLifeTimer() * frequency) * magnitude;
        //m_MoveVector += direction;
        return Vector2.zero;
    }
}
