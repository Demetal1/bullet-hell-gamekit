using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyBehaviour))]
public class EnemyBehaviourEditor : Editor
{
    SerializedProperty m_ContactDamager;
    SerializedProperty m_BulletSpawnPoint;

    SerializedProperty m_ShouldShoot;
    SerializedProperty m_BurstShot;
    SerializedProperty m_BurstBullets;
    SerializedProperty m_BurstGap;
    SerializedProperty m_ShootingGap;

    SerializedProperty m_DieAudio;

    SerializedProperty m_FlickeringDuration;

    private void OnEnable()
    {
        m_ContactDamager = serializedObject.FindProperty("contactDamager");
        m_BulletSpawnPoint = serializedObject.FindProperty("bulletSpawnPoint");

        m_ShouldShoot = serializedObject.FindProperty("shouldShoot");
        m_BurstShot = serializedObject.FindProperty("burstShot");
        m_BurstBullets = serializedObject.FindProperty("burstBullets");
        m_BurstGap = serializedObject.FindProperty("burstGap");
        m_ShootingGap = serializedObject.FindProperty("shootingGap");

        m_DieAudio = serializedObject.FindProperty("dieAudio");

        m_FlickeringDuration = serializedObject.FindProperty("flickeringDuration");
    }

    public override void OnInspectorGUI ()
    {
        serializedObject.Update ();

        EditorGUILayout.PropertyField(m_ContactDamager);
        EditorGUILayout.PropertyField(m_BulletSpawnPoint);

        m_ShouldShoot.boolValue = EditorGUILayout.Toggle("Should Shoot?", m_ShouldShoot.boolValue);
        if(m_ShouldShoot.boolValue)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUI.indentLevel++;
            
            EditorGUILayout.PropertyField(m_ShootingGap);

            m_BurstShot.boolValue = EditorGUILayout.Toggle("Burst Shot?", m_BurstShot.boolValue);
            if(m_BurstShot.boolValue)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(m_BurstBullets);
                EditorGUILayout.PropertyField(m_BurstGap);

                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical ();
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical ();
        }

        EditorGUILayout.PropertyField(m_DieAudio);
        EditorGUILayout.PropertyField(m_FlickeringDuration);

        serializedObject.ApplyModifiedProperties ();
    }
}
