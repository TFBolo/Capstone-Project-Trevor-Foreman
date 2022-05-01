using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Weapon : MonoBehaviour
{
    public float damage;
    private PetAI ai;

    private float effectTime;
    private List<GameObject> enemyList;
    private List<float> timeList;

    private bool timedEffect;

    private void Start()
    {
        ai = GameObject.FindWithTag("Pet").GetComponent<PetAI>();
        enemyList = new();
        timeList = new();
    }

    private void Update()
    {
        if (Time.timeScale == 0) return;
        EffectTimer();
    }

    private void EffectTimer()
    {
        for (int i = 0; i < timeList.Count; i++)
        {
            timeList[i] += Time.deltaTime;
        }
        for (int i = 0; i < timeList.Count; i++)
        {
            if (timeList[i] >= 5f)
            {
                timeList.RemoveAt(i);
                enemyList[i].GetComponent<NavMeshAgent>().speed *= 2;
                enemyList.RemoveAt(i);
                i--;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            if (Random.Range(1, 101) <= ((float)ai.bond.GetLevelNumber() / 2f) + 1f)
            {
                damage = (ai.damage * 2);
            }
            else
            {
                damage = ai.damage;
            }

            if (ai.attackEffect == 0)
            {
                other.gameObject.GetComponent<EnemyAI>().currentHealth -= damage;
            }
            else if (ai.attackEffect == 1)
            {
                other.gameObject.GetComponent<EnemyAI>().currentHealth -= (damage / 2);
                if (!enemyList.Contains(other.gameObject))
                {
                    enemyList.Add(other.gameObject);
                    timeList.Add(0f);
                    other.gameObject.GetComponent<NavMeshAgent>().speed /= 2;
                } 
                else if (enemyList.Contains(other.gameObject))
                {
                    timeList[enemyList.IndexOf(other.gameObject)] = 0f;
                }
            }
        }
    }
}
