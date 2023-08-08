using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TaskFunc;

namespace TaskFunc
{
    public delegate bool Task();
}


public class TaskNode : Node
{
    public Task func;
    private Vector3 vec = Vector3.zero;

    public TaskNode(Task func = null, float tick = 0.0f)
    {
        attribute = NodeAttribute.Task;
        //isSuccesed = false;
        this.tick = tick;
        this.func = func;
        children = null;
    }

    public void SetVector3(Vector3 v) { vec = v; }

    public override void Execute()
    {
        if (func == null) return;
        isSuccesed = func();
    }
}
