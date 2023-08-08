using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Xml;
using UnityEngine;

class Monster_Elite_BT : MonoBehaviour
{
    Monster_Elite monster;

    [HideInInspector]
    public BehaviorTree bt;

    RootNode root;

    SelectorNode selNode;
    SelectorNode selNode2;

    SequenceNode seqPatrol;
    TaskNode tPatrol;

    SelectorNode selChase;
    SequenceNode seqMove;
    TaskNode tMove;
    SequenceNode seqPathfind;
    TaskNode tPathfind;
    
    SequenceNode seqAction;
    TaskNode tAction;
    SequenceNode seqHitted;
    TaskNode tHitted;
    SequenceNode seqGroggy;
    TaskNode tGroggy;
    SequenceNode seqDead;
    TaskNode tDead;

    void Awake()
    {
        //bt = gameObject.AddComponent<BehaviorTree>();
        bt = GetComponent<BehaviorTree>();

        //dTask Patrol = new dTask(monster.Patrol);

        monster = GetComponent<Monster_Elite>();
        
        root = new RootNode();

        selNode = new SelectorNode();
        selNode2 = new SelectorNode();

        seqPatrol = new SequenceNode();
        tPatrol = new TaskNode(monster.Patrol);

        selChase = new SelectorNode();
        seqMove = new SequenceNode();
        tMove = new TaskNode(monster.MoveTo);
        seqPathfind = new SequenceNode();
        tPathfind = new TaskNode(monster.Chase);

        //seqAction = new SequenceNode();
        //tAction = new TaskNode();
        seqHitted = new SequenceNode();
        tHitted = new TaskNode(monster.Hitted);
        seqGroggy = new SequenceNode();
        tGroggy = new TaskNode(monster.Groggy);
        seqDead = new SequenceNode();
        tDead = new TaskNode(monster.Dead);

        LinkTreeNode();
    }

    private void Update()
    {
        if(monster.GetState() != null)
        {
            seqPatrol.SetState(monster.GetState().IsIdle());
            seqMove.SetState(monster.GetState().IsMove());
            seqPathfind.SetState(monster.GetState().IsPathfind());
            //seqAction.SetState(monster.state.IsAttack());
            seqHitted.SetState(monster.GetState().IsHitted());
            seqGroggy.SetState(monster.GetState().IsGroggy());
            seqDead.SetState(monster.GetState().IsDead());
        }
        
        if(monster.GetState().IsMove())
            tMove.SetVector3(monster.GetTargetPos());

        bt.BTUpdate(root);
        //bt.ExecuteBT(root);
    }

    void LinkTreeNode()
    {
        root.SetChild(selNode2);

        selNode2.SetChild(selNode);

            selNode.SetChild(seqPatrol);
                seqPatrol.SetChild(tPatrol);

            selNode.SetChild(seqHitted);
                seqHitted.SetChild(tHitted);
    
            selNode.SetChild(selChase);
                    selChase.SetChild(seqMove);
                    seqMove.SetChild(tMove);

                    selChase.SetChild(seqPathfind);
                    seqPathfind.SetChild(tPathfind);            

            selNode.SetChild(seqDead);
                seqDead.SetChild(tDead);
                //selNode.SetChild(seqAction);
                //seqAction.SetChild(tAction);        
 
        selNode2.SetChild(seqGroggy);
            seqGroggy.SetChild(tGroggy);

    }
}