using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AB_Smoke : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetFloat("Duration", 0.0f);
        //animator.SetBool("IsPlayed", true);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetFloat("Duration", stateInfo.normalizedTime);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetFloat("Duration", 0.0f);
        animator.SetBool("IsPlayed", false);
    }
}
