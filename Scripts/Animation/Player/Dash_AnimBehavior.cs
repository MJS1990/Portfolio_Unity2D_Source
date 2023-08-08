using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash_AnimBehavior : StateMachineBehaviour
{
    float dashTime;
    float dAttackTime;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        dashTime = 0.0f;
        dAttackTime = 0.0f;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.IsName("Dash") == true)
        {
            dashTime = stateInfo.normalizedTime;
            animator.SetFloat("DashDuration", dashTime);
        }

        if (stateInfo.IsName("DashAttack") == true)
        {
            dAttackTime = stateInfo.normalizedTime;

            animator.SetFloat("DashAttackDuration", dAttackTime);
            
            animator.SetBool("IsDash", false);
            animator.SetBool("IsJumping", false);
        }

        if (dashTime > 1.0f)
        {
            animator.SetBool("IsDash", false);

        }
    }
    
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetFloat("DashDuration", 0.0f);
    }
}
