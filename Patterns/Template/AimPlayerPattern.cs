using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Patterns/Aim Player Pattern")]
public class AimPlayerPattern : Pattern
{
    [ReadOnly]
    public PatternType type = PatternType.AimPlayer;

    public float speed = 2f;

    public override Vector2 CalculateMovement(BulletObject bulletObject)
    {
        return AimPlayer(bulletObject);
    }

    public Vector2 AimPlayer(BulletObject bulletObject)
    {
        return bulletObject.bullet.GetPlayerDirection() * speed * Time.deltaTime;
    }
}
