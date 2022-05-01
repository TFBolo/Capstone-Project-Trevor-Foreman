using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSystem : MonoBehaviour
{
    [SerializeField] private int touchDamage;

    [SerializeField] private HealthSystem healthController;

    [SerializeField] private bool persist;

    [SerializeField] private bool harmEnemies;

    [SerializeField] private int enemyDamage;

    private void Start()
    {
        healthController = GameObject.FindGameObjectWithTag("UI").GetComponent<HealthSystem>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            healthController.damage(touchDamage);
            healthController.UpdateHealth();
            if (!persist)
            {
                gameObject.SetActive(false);
            }
        }
        else if (other.CompareTag("Enemy") && harmEnemies)
        {
            other.gameObject.GetComponent<EnemyAI>().currentHealth -= enemyDamage;
        }
    }
}
