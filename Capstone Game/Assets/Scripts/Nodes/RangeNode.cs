using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeNode : Node
{
    private float range;
    private float angle;
    private Transform target;
    private Transform origin;

    public LayerMask targetMask;
    public LayerMask obstructionMask;

    public bool canSeePlayer;

    public RangeNode(float range, float angle, Transform target, Transform origin)
    {
        this.range = range;
        this.target = target;
        this.origin = origin;
        this.angle = angle;
        targetMask = LayerMask.GetMask("Player");
        obstructionMask = LayerMask.GetMask("Ground");
    }

    public override NodeState Evaluate()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(origin.position, range, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - origin.position).normalized;

            if (Vector3.Angle(origin.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(origin.position, target.position);

                if (!Physics.Raycast(origin.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    canSeePlayer = true;
                }
            }
        }
        if (Vector3.Distance(origin.position, target.position) >= 2*range)
        {
            canSeePlayer = false;
        }
        return canSeePlayer ? NodeState.SUCCESS : NodeState.FAILURE;
    }
}
