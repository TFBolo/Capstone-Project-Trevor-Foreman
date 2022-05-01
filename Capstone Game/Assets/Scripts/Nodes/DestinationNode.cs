using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DestinationNode : Node
{
    private NavMeshAgent agent;
    private PetAI ai;

    public DestinationNode(PetAI ai, NavMeshAgent agent)
    {
        this.agent = agent;
        this.ai = ai;
    }

    public override NodeState Evaluate()
    {
        agent.isStopped = false;
        agent.SetDestination(ai.GetPetDestination());
        return NodeState.RUNNING;
    }
}
