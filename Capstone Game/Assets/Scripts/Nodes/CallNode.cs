using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CallNode : Node
{
    private NavMeshAgent agent;
    private Transform playerRef;

    public CallNode(NavMeshAgent agent, Transform playerRef)
    {
        this.agent = agent;
        this.playerRef = playerRef;
    }

    public override NodeState Evaluate()
    {
        agent.isStopped = false;
        agent.SetDestination(Vector3.MoveTowards(playerRef.position, agent.transform.position, 1.5f));

        return NodeState.RUNNING;
    }
}
