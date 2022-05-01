using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AttackNode : Node
{
    private NavMeshAgent agent;
    private EnemyAI ai;
    private Transform target;
    private float angle;

    private Vector3 currentVelocity;
    private float smoothDamp;

    public AttackNode(NavMeshAgent agent, EnemyAI ai, Transform target, float smoothDamp, float angle)
    {
        this.agent = agent;
        this.ai = ai;
        this.target = target;
        this.smoothDamp = smoothDamp;
        this.angle = angle;
    }

    public override NodeState Evaluate()
    {
        agent.isStopped = true;
        Vector3 direction = target.position - ai.transform.position;
        Vector3 currentDirection = Vector3.SmoothDamp(ai.transform.forward, direction, ref currentVelocity, smoothDamp);
        Quaternion rotation = Quaternion.LookRotation(currentDirection, Vector3.up);
        ai.transform.rotation = rotation;

        Vector3 directionToTarget = (target.position - ai.transform.position).normalized;
        if (Vector3.Angle(ai.transform.forward, directionToTarget) < (angle / 2) / 2)
        {
            ai.Attack1();
        }
        return NodeState.RUNNING;
    }

}
