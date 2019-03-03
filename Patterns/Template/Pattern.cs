using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PatternType
{
    Circular,
    AimPlayer,
    Straight,
    Wave
}

public class Pattern : ScriptableObject
{
    [Range(0,359)]
    public int rotationDegrees;

    public virtual void InitializePattern(Enemy enemy){}

    public virtual void InitializePattern(BulletObject bulletObject){}
    
    public virtual Vector2 CalculateMovement(Enemy enemy)
    { 
        Debug.LogWarning("Pattern is not implemented for Enemy");
        return Vector2.zero; 
    }
    public virtual Vector2 CalculateMovement(BulletObject bulletObject)
    {
        Debug.LogWarning("Pattern is not implemented for Bullet");
        return Vector2.zero; 
    }
}
