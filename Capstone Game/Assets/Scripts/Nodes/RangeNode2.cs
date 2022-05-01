using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeNode2 : Node
{
    private float range;
    private Transform target;
    private Transform origin;
    private LayerMask playerMask;
    private LayerMask obstructionMask;

    public RangeNode2(float range, Transform target, Transform origin)
    {
        this.range = range;
        this.target = target;
        this.origin = origin;
        playerMask = LayerMask.GetMask("Player");
        obstructionMask = LayerMask.GetMask("Ground");
    }

    public override NodeState Evaluate()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(origin.position, range, playerMask);

        if (rangeChecks.Length != 0)
        {
            Vector3 directionToTarget = (target.position - origin.position).normalized;
            float distanceToTarget = Vector3.Distance(origin.position, target.position);

            if (!Physics.Raycast(origin.position, directionToTarget, distanceToTarget, obstructionMask))
            {
                return NodeState.SUCCESS;
            }
            else
            {
                return NodeState.FAILURE;
            }
        }
        else
        {
            return NodeState.FAILURE;
        }

        /*float distance = Vector3.Distance(target.position, origin.position);
        return distance <= range ? NodeState.SUCCESS : NodeState.FAILURE;*/
    }
}
