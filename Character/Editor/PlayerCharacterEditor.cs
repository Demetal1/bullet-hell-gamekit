using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Miasma
{
    [CustomEditor(typeof(PlayerCharacter))]
    public class PlayerCharacterEditor : Editor
    {
        //References Prop
        SerializedProperty m_SpriteRendererProp;
        SerializedProperty m_HitboxRendererProp;
        SerializedProperty m_DamageableProp;

        //Movement Prop
        SerializedProperty m_RunSpeedProp;
        SerializedProperty m_WalkSpeedProp;
        SerializedProperty m_DashSpeedProp;
        SerializedProperty m_DashPressTimeProp;
    
        //Hurt Prop
        SerializedProperty m_FlickeringDurationProp;
        SerializedProperty m_RespawnLocationProp;

    
        //Shoot Prop
        SerializedProperty m_ShotsPerSecondProp;
        SerializedProperty m_BulletSpawnPointLeftProp;
        SerializedProperty m_BulletSpawnPointMiddleProp;
        SerializedProperty m_BulletSpawnPointRightProp;
        SerializedProperty m_BulletPoolLeftProp;
        SerializedProperty m_BulletPoolMiddleProp;
        SerializedProperty m_BulletPoolRightProp;
        SerializedProperty m_LeftPatternProp;
        SerializedProperty m_MiddlePatternProp;
        SerializedProperty m_RightPatternProp;


        //Audio Prop
        SerializedProperty m_HurtAudioPlayerProp;
        SerializedProperty m_ShootAudioPlayerProp;
        SerializedProperty m_DashAudioPlayerProp;

        //Particle Prop
        SerializedProperty m_DeathParticleProp;
        SerializedProperty m_RespawnParticleProp;

        //Misc Prop
        SerializedProperty m_SpriteOriginallyFacesLeftProp;

        //Foldouts
        bool m_ReferencesFoldout;
        bool m_MovementSettingsFoldout;
        bool m_HurtSettingsFoldout;
        bool m_ShootSettingsFoldout;
        bool m_AudioSettingsFoldout;
        bool m_ParticleSettingsFoldout;
        bool m_MiscSettingsFoldout;

        //References Content
        readonly GUIContent m_SpriteRendererContent = new GUIContent("Sprite Renderer");
        readonly GUIContent m_HitboxRendererContent = new GUIContent("Hitbox Renderer");
        readonly GUIContent m_DamageableContent = new GUIContent("Damageable");

        //Movement Content
        readonly GUIContent m_RunSpeedContent = new GUIContent("Run Speed");
        readonly GUIContent m_WalkSpeedContent = new GUIContent("Walk Speed");
        readonly GUIContent m_DashSpeedContent = new GUIContent("Dash Speed");
        readonly GUIContent m_DashPressTimeContent = new GUIContent("Dash Press Time");

        //Hurt Content
        readonly GUIContent m_RespawnLocationContent = new GUIContent("Respawn Location");
        readonly GUIContent m_FlickeringDurationContent = new GUIContent("Flicking Duration", "When the player is hurt she becomes invulnerable for a short time and the SpriteRenderer flickers on and off to indicate this.  This field is the duration in seconds the SpriteRenderer stays either on or off whilst flickering.  To adjust the duration of invulnerability see the Damageable component.");

        //Shoot Content
        readonly GUIContent m_ShotsPerSecondContent = new GUIContent("Shots Per Second");
        readonly GUIContent m_BulletSpawnPointLeftContent = new GUIContent("Bullet Spawn Point Left");
        readonly GUIContent m_BulletSpawnPointMiddleContent = new GUIContent("Bullet Spawn Point Middle");
        readonly GUIContent m_BulletSpawnPointRightContent = new GUIContent("Bullet Spawn Point Right");
        readonly GUIContent m_BulletPoolLeftContent = new GUIContent("Bullet Pool Left");
        readonly GUIContent m_BulletPoolMiddleContent = new GUIContent("Bullet Pool Middle");
        readonly GUIContent m_BulletPoolRightContent = new GUIContent("Bullet Pool Right");
        readonly GUIContent m_LeftPatternContent = new GUIContent("Pattern Left");
        readonly GUIContent m_MiddlePatternContent = new GUIContent("Pattern Middle");
        readonly GUIContent m_RightPatternContent = new GUIContent("Pattern Right");

        //Audio Content
        readonly GUIContent m_HurtAudioPlayerContent = new GUIContent("Hurt Audio Player");
        readonly GUIContent m_ShootAudioPlayerContent = new GUIContent("Shoot Audio Player");
        readonly GUIContent m_DashAudioPlayerContent = new GUIContent("Dash Audio Player");

        //VFX Content
        readonly GUIContent m_DeathParticleContent = new GUIContent("Death Particle");
        readonly GUIContent m_RespawnParticleContent = new GUIContent("Respawn Particle");

        //Misc Content
        readonly GUIContent m_SpriteOriginallyFacesLeftContent = new GUIContent("Sprite Originally Faces Left");

        //Foldouts Content
        readonly GUIContent m_ReferencesContent = new GUIContent("References");
        readonly GUIContent m_MovementSettingsContent = new GUIContent("Movement Settings");
        readonly GUIContent m_HurtSettingsContent = new GUIContent("Hurt Settings");
        readonly GUIContent m_ShootSettingsContent = new GUIContent("Shoot Settings");
        readonly GUIContent m_AudioSettingsContent = new GUIContent("Audio Settings");
        readonly GUIContent m_ParticleSettingsContent = new GUIContent("Particle Settings");
        readonly GUIContent m_MiscSettingsContent = new GUIContent("Misc Settings");

        private GUIStyle style; 

        void OnEnable ()
        {
            style = new GUIStyle();
            style.richText = true;

            //References Properties
            m_SpriteRendererProp = serializedObject.FindProperty("spriteRenderer");
            m_HitboxRendererProp = serializedObject.FindProperty("hitboxRenderer");
            m_DamageableProp = serializedObject.FindProperty("damageable");

            //Movement Properties
            m_RunSpeedProp = serializedObject.FindProperty("runSpeed");
            m_WalkSpeedProp = serializedObject.FindProperty("walkSpeed");
            m_DashSpeedProp = serializedObject.FindProperty("dashSpeed");
            m_DashPressTimeProp = serializedObject.FindProperty("dashPressTime");

            //Hurt Properties
            m_RespawnLocationProp = serializedObject.FindProperty("respawnLocation");
            m_FlickeringDurationProp = serializedObject.FindProperty ("flickeringDuration");

            //Ranged Properties
            m_ShotsPerSecondProp = serializedObject.FindProperty("shotsPerSecond");
            m_BulletSpawnPointLeftProp = serializedObject.FindProperty("bulletSpawnPointLeft");
            m_BulletSpawnPointMiddleProp = serializedObject.FindProperty("bulletSpawnPointMiddle");
            m_BulletSpawnPointRightProp = serializedObject.FindProperty("bulletSpawnPointRight");
            m_BulletPoolLeftProp = serializedObject.FindProperty("bulletPool");
            m_BulletPoolMiddleProp = serializedObject.FindProperty("bulletPool2");
            m_BulletPoolRightProp = serializedObject.FindProperty("bulletPool3");
            m_LeftPatternProp = serializedObject.FindProperty("leftPattern");
            m_MiddlePatternProp = serializedObject.FindProperty("middlePattern");
            m_RightPatternProp = serializedObject.FindProperty("rightPattern");

            //Audio Properties
            m_HurtAudioPlayerProp = serializedObject.FindProperty("hurtAudioPlayer");
            m_ShootAudioPlayerProp = serializedObject.FindProperty("shootAudioPlayer");
            m_DashAudioPlayerProp = serializedObject.FindProperty("dashAudioPlayer");

            //Particle Properties
            m_DeathParticleProp = serializedObject.FindProperty("deathParticle");
            m_RespawnParticleProp = serializedObject.FindProperty("spawnParticle");

            //Misc Properties
            m_SpriteOriginallyFacesLeftProp = serializedObject.FindProperty ("spriteOriginallyFacesLeft");
        }

        public override void OnInspectorGUI ()
        {
            serializedObject.Update ();

            EditorGUILayout.BeginVertical (GUI.skin.box);
            EditorGUI.indentLevel++;

            m_ReferencesFoldout = EditorGUILayout.Foldout (m_ReferencesFoldout, m_ReferencesContent);

            if (m_ReferencesFoldout)
            {
                EditorGUILayout.PropertyField (m_SpriteRendererProp, m_SpriteRendererContent);
                EditorGUILayout.PropertyField (m_HitboxRendererProp, m_HitboxRendererContent);
                EditorGUILayout.PropertyField (m_DamageableProp, m_DamageableContent);
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical ();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUI.indentLevel++;

            m_MovementSettingsFoldout = EditorGUILayout.Foldout(m_MovementSettingsFoldout, m_MovementSettingsContent);

            if (m_MovementSettingsFoldout)
            {
                EditorGUILayout.PropertyField(m_RunSpeedProp, m_RunSpeedContent);
                EditorGUILayout.PropertyField(m_WalkSpeedProp, m_WalkSpeedContent);
                EditorGUILayout.PropertyField(m_DashSpeedProp, m_DashSpeedContent);
                EditorGUILayout.PropertyField(m_DashPressTimeProp, m_DashPressTimeContent);
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUI.indentLevel++;

            m_HurtSettingsFoldout = EditorGUILayout.Foldout(m_HurtSettingsFoldout, m_HurtSettingsContent);

            if (m_HurtSettingsFoldout)
            {
                EditorGUILayout.PropertyField (m_RespawnLocationProp, m_RespawnLocationContent);
                EditorGUILayout.PropertyField (m_FlickeringDurationProp, m_FlickeringDurationContent);
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUI.indentLevel++;

            m_ShootSettingsFoldout = EditorGUILayout.Foldout(m_ShootSettingsFoldout, m_ShootSettingsContent);

            if (m_ShootSettingsFoldout)
            {
                EditorGUILayout.PropertyField(m_ShotsPerSecondProp, m_ShotsPerSecondContent);
                EditorGUILayout.PropertyField(m_BulletSpawnPointLeftProp, m_BulletSpawnPointLeftContent);
                EditorGUILayout.PropertyField(m_BulletSpawnPointMiddleProp, m_BulletSpawnPointMiddleContent);
                EditorGUILayout.PropertyField(m_BulletSpawnPointRightProp, m_BulletSpawnPointRightContent);
                EditorGUILayout.PropertyField(m_BulletPoolLeftProp, m_BulletPoolLeftContent);
                EditorGUILayout.PropertyField(m_BulletPoolMiddleProp, m_BulletPoolMiddleContent);
                EditorGUILayout.PropertyField(m_BulletPoolRightProp, m_BulletPoolRightContent);
                EditorGUILayout.PropertyField(m_LeftPatternProp, m_LeftPatternContent);
                EditorGUILayout.PropertyField(m_MiddlePatternProp, m_MiddlePatternContent);
                EditorGUILayout.PropertyField(m_RightPatternProp, m_RightPatternContent);
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUI.indentLevel++;

            m_AudioSettingsFoldout = EditorGUILayout.Foldout(m_AudioSettingsFoldout, m_AudioSettingsContent);

            if (m_AudioSettingsFoldout)
            {
                EditorGUILayout.PropertyField(m_HurtAudioPlayerProp, m_HurtAudioPlayerContent);
                EditorGUILayout.PropertyField(m_ShootAudioPlayerProp, m_ShootAudioPlayerContent);
                EditorGUILayout.PropertyField(m_DashAudioPlayerProp, m_DashAudioPlayerContent);
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUI.indentLevel++;

            m_ParticleSettingsFoldout = EditorGUILayout.Foldout(m_ParticleSettingsFoldout, m_ParticleSettingsContent);

            if (m_ParticleSettingsFoldout)
            {
                EditorGUILayout.PropertyField(m_DeathParticleProp, m_DeathParticleContent);
                EditorGUILayout.PropertyField(m_RespawnParticleProp, m_RespawnParticleContent);
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUI.indentLevel++;

            m_MiscSettingsFoldout = EditorGUILayout.Foldout(m_MiscSettingsFoldout, m_MiscSettingsContent);

            if (m_MiscSettingsFoldout)
            {
                EditorGUILayout.PropertyField(m_SpriteOriginallyFacesLeftProp, m_SpriteOriginallyFacesLeftContent);
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties ();

            GUILayout.Label("<b>Select Bullet Prefab</b>",style);
            if (GUILayout.Button("Select Prefab"))
            {
                Selection.activeObject = PrefabUtility.GetCorrespondingObjectFromOriginalSource(((PlayerCharacter)target).bulletPool.prefab);
            }
        }
    }
}
