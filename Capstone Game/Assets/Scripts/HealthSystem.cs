using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    public int healthPoints;
    public int maxHealth;

    [SerializeField] private Image[] healthCapsule;

    private bool iTime;
    private float currentTime;

    private void Start()
    {
        iTime = false;
        healthPoints = 5;
        maxHealth = 5;
        UpdateHealth();
    }

    private void Update()
    {
        if (iTime)
        {
            if (currentTime < 1f)
            {
                currentTime += Time.deltaTime;
            }
            else
            {
                iTime = false;
                currentTime = 0f;
            }
        }
    }

    public void UpdateHealth()
    {
        for (int i = 0; i < healthCapsule.Length; i++)
        {
            if (i < healthPoints && i < maxHealth)
            {
                healthCapsule[i].color = Color.red;
            }
            else if (i >= maxHealth)
            {
                healthCapsule[i].color = new Color(0f, 0f, 0f, 0f);
            }
            else
            {
                healthCapsule[i].color = Color.black;
            }
        }

    }

    public HealthSystem(int healthPoints)
    {
        this.maxHealth = healthPoints;
        this.healthPoints = healthPoints;
    }

    public int GetHealth()
    {
        return healthPoints;
    }

    public void damage(int damageAmount)
    {
        if (!iTime) 
        {
            healthPoints -= damageAmount;
            iTime = true;
        } 
        if (healthPoints == 0)
        {
            transform.gameObject.GetComponent<InteractableController>().YouDied();
        }
    }

    public void Heal(int healAmount)
    {
        healthPoints += healAmount;
        if (healthPoints > maxHealth) healthPoints = maxHealth;
    }
}
