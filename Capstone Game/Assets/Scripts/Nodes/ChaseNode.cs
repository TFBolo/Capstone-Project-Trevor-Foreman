using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChaseNode : Node
{
    private Transform target;
    private NavMeshAgent agent;
    private EnemyAI ai;
    private float attackRange;

    public ChaseNode(Transform target, NavMeshAgent agent, EnemyAI ai, float attackRange)
    {
        this.target = target;
        this.agent = agent;
        this.ai = ai;
        this.attackRange = attackRange;
    }

    public override NodeState Evaluate()
    {
        ai.SetAggro();
        float distance = Vector3.Distance(target.position, agent.transform.position);
        if (distance > attackRange / 2)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
            return NodeState.RUNNING;
        }
        else
        {
            agent.isStopped = true;
            return NodeState.FAILURE;
        }
    }
}
