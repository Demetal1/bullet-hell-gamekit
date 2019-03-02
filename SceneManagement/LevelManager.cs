using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Playables;

//Level timer controls the level timer, is used to trigger sections and paused when do so
public class LevelManager : MonoBehaviour
{
    #region Singleton
    static protected LevelManager s_LevelManager;
    static public LevelManager Instance { get { return s_LevelManager; } }
    #endregion

    [ReadOnly]
    public float levelTimer = 0.0f;

    public Section[] sections;

    private Coroutine m_CurrentSectionCoroutine;
    private float m_NextSectionTime = 0;
    private bool m_SectionPlaying = false;
    private int m_CurrentSectionIndex = 0;
    private int m_WaveCount;

    //Constants
    const double k_Approximation = 0.1f;

    private void Start() 
    {
        s_LevelManager = this;
        m_NextSectionTime = sections[0].triggerTime;
    }

    private void Update()
    {
        if(!m_SectionPlaying)
            levelTimer += Time.deltaTime;
        else
            return;

        if(levelTimer > (m_NextSectionTime - k_Approximation) && levelTimer < (m_NextSectionTime + k_Approximation))
        {   
            if(m_CurrentSectionIndex == sections.Length)
                return;

            m_NextSectionTime = levelTimer;
            PlaySection(sections[m_CurrentSectionIndex]);
        }

    }

    private void PlaySection(Section section)
    {
        Debug.Log("Playing Section");
        m_SectionPlaying = true;
        if(section.type == SectionType.DIALOGUE)
            section.dialogue.stopped += EndSection;
        section.StartSection();
    }

    public void EndSpawning(Section section)
    {
        m_WaveCount++;

        if(m_WaveCount == section.spawners.Count)
        {
            m_WaveCount = 0;
            EndSection();
        }
    }

    private void EndSection(PlayableDirector director = null)
    {
        m_SectionPlaying = false;

        m_CurrentSectionIndex++;
        if(m_CurrentSectionIndex < sections.Length)
            m_NextSectionTime += sections[m_CurrentSectionIndex].triggerTime;
    }
}
