using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Anything Changed here needs to change in SaveData as well
[System.Serializable]
public class PlayerData
{
    public int powerLevel;
    public int powerExp;
    public int speedLevel;
    public int speedExp;
    public int bondLevel;
    public int bondExp;
    public int enduranceLevel;
    public int enduranceExp;

    public List<int> petAttacks = new List<int>();

    public float startPower;
    public float startSpeed;
    public float startBond;
    public float startEndurance;

    public float staminaCurrent;

    public string inventory;

    public PlayerData(PetAI petAI, string inventory)
    {
        powerLevel = petAI.power.GetLevelNumber();
        powerExp = petAI.power.GetExperience();
        speedLevel = petAI.speed.GetLevelNumber();
        speedExp = petAI.speed.GetExperience();
        bondLevel = petAI.bond.GetLevelNumber();
        bondExp = petAI.bond.GetExperience();
        enduranceLevel = petAI.endurance.GetLevelNumber();
        enduranceExp = petAI.power.GetExperience();

        petAttacks = petAI.attackList;

        startPower = petAI.basePower;
        startSpeed = petAI.baseSpeed;
        startBond = petAI.baseBond;
        startEndurance = petAI.baseEndurance;

        staminaCurrent = petAI.currentStamina;

        this.inventory = inventory;
    }
}
