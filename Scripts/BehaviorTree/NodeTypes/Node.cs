using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Node
{
    public enum NodeAttribute
    {
        Root = 0,

        Sequence,
        Selector, 

        Task,

        //Decorator
        If,
        Loop,
        Fixed,
        Random,
    }

    public NodeAttribute attribute;
    public bool isSuccesed;
    public float tick;
    public List<Node> children;

    bool state = true;
    public bool GetState() { return state; }
    public void SetState(bool val) { state = val; }

    public abstract void Execute();

    public void SetChild(Node node)
    {
        children.Add(node);
    }

    public bool GetChildResult(int index)
    {
        return children[index].isSuccesed;
    }
}
