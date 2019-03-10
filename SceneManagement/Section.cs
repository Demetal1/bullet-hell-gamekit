using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using RPGTALK;
using UnityEngine.Playables;

public enum SectionType
{
    DIALOGUE,
    BOSS,
    WAVE
}

[Serializable]
public class Section
{
    [Tooltip("this section will trigger based on when the last section finishes, by the formula (levelTimer + triggerTime)")]
    public float triggerTime;
    public SectionType type = SectionType.WAVE;
    public List<SpawnerWrapper> spawners;
    public PlayableDirector dialogue;

    public void StartSection()
    {
        if(type == SectionType.WAVE)
        {
            if(spawners.Count > 0)
            {
                for(int i = 0; i < spawners.Count; i++)
                {
                    spawners[i].Spawn(this);
                }
            }
        }
        else if(type == SectionType.DIALOGUE)
        {
            dialogue.Play();
        }
        else
        {

        }
    }
}

[Serializable]
public class SpawnerWrapper
{
    public EnemySpawner spawn;
    public Transform spawnPosition;

    public void Spawn(Section section)
    {
        spawn.StartSpawn(spawnPosition.position, section);
    }
}
