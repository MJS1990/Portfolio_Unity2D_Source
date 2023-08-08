using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorTree : MonoBehaviour
{
    private bool isTreeEnable;

    [HideInInspector]
    public double callCount;

    void Start()
    {
        isTreeEnable = true;
        callCount = 0.0d;
    }

    public void BTUpdate(Node node)
    {
        if (node != null) 
        {
            switch (node.attribute)
            {
                case Node.NodeAttribute.Root:
                {
                    for (int i = 0; i < node.children.Count; i++)
                        BTUpdate(node.children[i]);
                    
                    //node.Execute();

                    if (node.isSuccesed == false)
                        isTreeEnable = false;

                    break;
                }
                case Node.NodeAttribute.Sequence:
                {
                    if (node.GetState() == false)
                    {
                        node.isSuccesed = false;
                        break;
                    }

                    for (int i = 0; i < node.children.Count; i++)
                    {
                        BTUpdate(node.children[i]);
                        if (node.children[i].isSuccesed == false)
                        {
                            node.isSuccesed = false;
                            break;
                        }
                    }

                    break;
                }
                case Node.NodeAttribute.Selector:
                {
                    if (node.GetState() == false)
                    {
                        node.isSuccesed = false;
                        break;
                    }

                    for (int i = 0; i < node.children.Count; i++)
                    {
                        BTUpdate(node.children[i]);
                    }

                    node.Execute();

                    break;
                }
                case Node.NodeAttribute.Task:
                {
                    node.Execute();

                    break;
                }
            }
        }
    }
}