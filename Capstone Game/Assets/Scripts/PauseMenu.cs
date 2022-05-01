using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public PetAI ai;

    public GameObject pausePanel;

    private void Start()
    {
        ai = GameObject.FindWithTag("Pet").GetComponent<PetAI>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            Time.timeScale = 0;
            pausePanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void AddXP()
    {
        ai.power.AddExperience(500);
        ai.speed.AddExperience(500);
        ai.bond.AddExperience(500);
        ai.endurance.AddExperience(500);
    }

    public void AddSpeed()
    {
        ai.speed.AddExperience(500);
    }

    public void UnPause()
    {
        Time.timeScale = 1;
        pausePanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }
}
