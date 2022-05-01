using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PetAttackNode : Node
{
    private int attackType;
    private Transform target;
    private Transform origin;
    private PetAI ai;
    private NavMeshAgent agent;
    private float range;

    public PetAttackNode(Transform origin, PetAI ai, NavMeshAgent agent, float range)
    {
        this.origin = origin;
        this.ai = ai;
        this.agent = agent;
        this.range = range;
    }

    public override NodeState Evaluate()
    {
        attackType = ai.GetAttackType();
        target = ai.GetEnemy();
        float distance = Vector3.Distance(target.position, origin.position);
        if (distance > range)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
            return NodeState.RUNNING;
        }
        else
        {
            
            Vector3 directionToTarget = (target.position - origin.position);
            if (Vector3.Distance(origin.position, target.position) <= 2f)
            {
                if (attackType == 1)
                {
                    ai.AttackType1();
                    return NodeState.FAILURE;
                }
                else if (attackType == 2)
                {
                    ai.AttackType2();
                    return NodeState.FAILURE;
                }
                else if (attackType == 3)
                {
                    ai.AttackType3();
                    return NodeState.FAILURE;
                }
                else if (attackType == 4)
                {
                    ai.AttackType4();
                    return NodeState.FAILURE;
                }
            }
        }
        return NodeState.RUNNING;
    }
}
