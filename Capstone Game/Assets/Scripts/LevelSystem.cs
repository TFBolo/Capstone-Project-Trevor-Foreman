using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSystem
{
    [Range(0, 100)]
    private int level;
    private int experience;
    private int expToNextLevel;

    public LevelSystem()
    {
        level = 0;
        experience = 0;
        expToNextLevel = 100;
    }

    public LevelSystem(int savedLevel, int savedExp)
    {
        level = savedLevel;
        experience = savedExp;
        expToNextLevel = 100;
    }

    public void AddExperience(int amount)
    {
        experience += amount;
        while (experience >= expToNextLevel && level < 100)
        {
            level++;
            experience -= expToNextLevel;
        }
    }
    
    public int GetLevelNumber()
    {
        return level;
    }

    public int GetExperience()
    {
        return experience;
    }

    public float GetExpNormalized()
    {
        return (float)experience / expToNextLevel;
    }
}
