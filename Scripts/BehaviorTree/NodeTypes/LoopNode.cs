using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopNode : Node
{
    int count;

    public LoopNode(float tick = 0.0f, int count = 0)
    {
        attribute = NodeAttribute.Loop;
        isSuccesed = false;
        this.tick = tick;
        children = new List<Node>();
        this.count = count;
    }

    override public void Execute()
    {
        for (int i = 0; i < count; i++) 
            Loop(children[0]);

        isSuccesed = children[0].isSuccesed;
    }

    void Loop(Node node)
    {
        if (node.children != null)
        {
            for (int i = 0; i < node.children.Count; i++)
            {
                Loop(node.children[i]);
            }
        }

        node.Execute();
    }
}
