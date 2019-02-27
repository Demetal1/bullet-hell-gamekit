﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Patterns/Circular Pattern")]
public class CircularPattern : Pattern
{
    [ReadOnly]
    public PatternType type = PatternType.Circular;

    public float width = 1f;
    public float height = 1f;
    [Tooltip("1 to move clockwise, -1 to move counter-clockwise")]
    public int direction = 1;

    public override Vector2 CalculateMovement(Enemy enemy)
    {
        return Circular(enemy);
    }

    public Vector2 Circular(Enemy enemy)
    {
        return new Vector2(CalculateX(enemy) , CalculateY(enemy)) * direction;
    }

    private float CalculateX(Enemy enemy)
    {
        return Mathf.Cos(enemy.enemyBehaviour.GetLifeTimer()) * width;
    }

    private float CalculateY(Enemy enemy)
    {
        return Mathf.Sin(enemy.enemyBehaviour.GetLifeTimer()) * height;
    }
}
