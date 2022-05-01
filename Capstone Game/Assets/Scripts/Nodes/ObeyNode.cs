using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObeyNode : Node
{
    private PetAI ai;
    private int order;

    public ObeyNode(PetAI ai, int order)
    {
        this.ai = ai;
        this.order = order;
    }

    public override NodeState Evaluate()
    {
        return ai.GetOrder() == order ? NodeState.SUCCESS : NodeState.FAILURE;
    }
}
