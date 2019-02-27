using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Patterns/Straight Pattern")]
public class StraightPattern : Pattern
{
    [ReadOnly]
    public PatternType type = PatternType.Straight;

    public float speed = 5f;

    public override Vector2 CalculateMovement(Enemy enemy)
    {
        //Debug.LogWarning("Straight Pattern is not implemented for enemies");
        return Vector2.down;
    }

    public override Vector2 CalculateMovement(BulletObject objectPool)
    {
        return Straight(objectPool);
    }

    public Vector2 Straight(BulletObject objectPool)
    {
        return objectPool.bullet.GetSettedDirection() * speed * Time.deltaTime;
    }
}
