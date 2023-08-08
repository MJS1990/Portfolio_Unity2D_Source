using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootNode : Node 
{
    public RootNode(float tick = 0.0f)
    {
        children = new List<Node>();

        attribute = NodeAttribute.Root;
        isSuccesed = true;
        this.tick = tick;
    }

    override public void Execute()
    {
        isSuccesed = true;

        for (int i = 0; i < children.Count; i++)
        {
            if (children[i].isSuccesed == false)
            {
                isSuccesed = false;
                break;
            }
        }
    }
}
