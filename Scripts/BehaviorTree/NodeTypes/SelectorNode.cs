using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorNode : Node
{
    public SelectorNode(float tick = 0.0f)
    {
        children = new List<Node>();

        attribute = NodeAttribute.Selector;
        isSuccesed = true;
        this.tick = tick;
    }

    override public void Execute()
    {
        for (int i = 0; i < children.Count; i++)
        {
            if (children[i].isSuccesed == true)
            {
                isSuccesed = true;
                break;
            }
        }
    }
}
