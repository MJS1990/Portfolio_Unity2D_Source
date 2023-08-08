using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinDamagedBehaviour : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(stateInfo.IsName("Goblin_Damaged_1"))
            animator.SetFloat("DamagedDuration", 0.0f);

        if(stateInfo.IsName("Attack"))
            animator.SetFloat("AttackDuration", 0.0f);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.IsName("Goblin_Damaged_1"))
            animator.SetFloat("DamagedDuration", stateInfo.normalizedTime);

        if (stateInfo.IsName("Attack"))
            animator.SetFloat("AttackDuration", stateInfo.normalizedTime);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
            animator.SetFloat("DamagedDuration", 0.0f);
            animator.SetFloat("AttackDuration", 0.0f);
    }
}
