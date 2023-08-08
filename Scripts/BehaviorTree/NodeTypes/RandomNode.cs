using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomNode : Node
{
    public int rValue;

    public RandomNode(float tick = 0.0f)
    {
        attribute = NodeAttribute.Random;
        isSuccesed = false;
        this.tick = tick;
        children = new List<Node>();

        rValue = 0;
    }


    override public void Execute()
    {
        int range = children.Count;
        rValue = Random.Range(0, range);
    }
}
