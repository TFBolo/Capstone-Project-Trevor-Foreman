using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SaveData : MonoBehaviour
{
    public static SaveData instance;
    public bool shouldLoad = false;

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

    public int hubSpawnPoint;

    private void Awake()
    {
        if (instance != null)
        {
            //SceneManager.MoveGameObjectToScene(instance.gameObject, SceneManager.GetActiveScene());
            Destroy(instance.gameObject);
        }
        instance = this;
        shouldLoad = false;

        DontDestroyOnLoad(this.gameObject);
    }

    public void LoadInstance()
    {
        shouldLoad = true;
    }

    public void SetData(PlayerData data)
    {
        powerLevel = data.powerLevel;
        powerExp = data.powerExp;
        speedLevel = data.speedLevel;
        speedExp = data.speedExp;
        bondLevel = data.bondLevel;
        bondExp = data.bondExp;
        enduranceLevel = data.enduranceLevel;
        enduranceExp = data.enduranceExp;

        petAttacks = data.petAttacks;

        startPower = data.startPower;
        startSpeed = data.startSpeed;
        startBond = data.startBond;
        startEndurance = data.startEndurance;

        staminaCurrent = data.staminaCurrent;

        inventory = data.inventory;
    }
}
