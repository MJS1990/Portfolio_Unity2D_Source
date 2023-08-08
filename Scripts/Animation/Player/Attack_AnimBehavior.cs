using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_AnimBehavior : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetFloat("Attack1Duration", 0.0f);
        animator.SetFloat("Attack2Duration", 0.0f);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(stateInfo.IsName("Attack_1") == true)
        {
            animator.SetFloat("Attack1Duration", stateInfo.normalizedTime);
            animator.SetBool("IsRunning", false);
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsDash", false);

        }
        else if(stateInfo.IsName("Attack_2") == true)
        {
            animator.SetFloat("Attack2Duration", stateInfo.normalizedTime);
            animator.SetBool("IsRunning", false);
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsDash", false);
        }

        if (animator.GetFloat("Attack1Duration") >= 1.0f)
            animator.SetBool("IsAttack", false);

        if (animator.GetFloat("Attack2Duration") >= 1.0f)
            animator.SetBool("IsAttack2", false);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetFloat("Attack1Duration", 0.0f);
        animator.SetFloat("Attack2Duration", 0.0f);
    }
}
