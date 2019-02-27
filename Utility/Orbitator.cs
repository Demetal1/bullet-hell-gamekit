using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbitator : MonoBehaviour
{
    public GameObject orbitateAround;
    public Vector3 velocity;
    public float smooth;
    public float width;
    public float height;
    public float depth;

    private float m_TimeCounter;
    private Vector3 m_MoveVector;
    private Rigidbody2D m_RigidBody2D;


    private void Awake()
    {
        m_RigidBody2D = GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
    void Update()
    {
        m_TimeCounter += Time.deltaTime;
        m_MoveVector.x = Mathf.Cos(m_TimeCounter) * width;
        m_MoveVector.y = Mathf.Sin(m_TimeCounter) * height;
        m_MoveVector.z = Mathf.Sin(m_TimeCounter) * depth;
        transform.position = Vector3.SmoothDamp(transform.position, orbitateAround.transform.position + m_MoveVector, ref velocity, smooth);
    }
}
