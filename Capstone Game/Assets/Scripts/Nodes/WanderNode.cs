using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WanderNode : Node
{
    private NavMeshAgent agent;
    private Vector3 spawn;
    private float dist;
    private EnemyAI ai;


    public WanderNode(NavMeshAgent agent, Vector3 spawn, float dist, EnemyAI ai)
    {
        this.agent = agent;
        this.spawn = spawn;
        this.dist = dist;
        this.ai = ai;
    }

    public override NodeState Evaluate()
    {
        Vector3 wander = spawn;
        if (Vector3.Distance(ai.transform.position, ai.GetWanderSpot()) <= 1.2f || ai.GetWanderSpot() == new Vector3(0f, 0f, 0f))
        {
            wander = spawn + Random.insideUnitSphere * dist;
            wander.y = spawn.y;
            ai.SetWanderSpot(wander);
            if (ai.WanderMove())
            {
                ai.SetWanderMoveFalse();
            }
        }
        else
        {
            wander = ai.GetWanderSpot();
        }
        if (ai.WanderMove())
        {
            agent.SetDestination(wander);
        }        return NodeState.RUNNING;
    }
}
