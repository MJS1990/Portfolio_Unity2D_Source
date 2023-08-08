using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class MonsterState : MonoBehaviour
{
    public enum EStateType //TODO : getter, setter 만들고 애니메이션 상태값과 매치되도록
    {
        Idle = 0,
        Move,
        Pathfind,
        Attack,
        Hitted,
        Groggy,
        Dead,
    };
    EStateType currentState;

    void Awake()
    {
        currentState = EStateType.Idle;
    }

    public EStateType GetCurrentState() { return currentState; }
    public void SetState(EStateType state) { currentState = state; }

    public bool IsIdle() { return currentState == EStateType.Idle ? true : false; }
    public bool IsMove() { return currentState == EStateType.Move ? true : false; }
    public bool IsPathfind() { return currentState == EStateType.Pathfind ? true : false; }
    public bool IsAttack() { return currentState == EStateType.Attack ? true : false; }
    public bool IsHitted() { return currentState == EStateType.Hitted ? true : false; }
    public bool IsGroggy() { return currentState == EStateType.Groggy ? true : false; }
    public bool IsDead() { return currentState == EStateType.Dead ? true : false; }

    public void SetIdle() { currentState = EStateType.Idle; }
    public void SetMove() { currentState = EStateType.Move; }
    public void SetPathfind() { currentState = EStateType.Pathfind; }
    public void SetAttack() { currentState = EStateType.Attack; }
    public void SetHitted() { currentState = EStateType.Hitted; }
    public void SetGroggy() { currentState = EStateType.Groggy; }
    public void SetDead() { currentState = EStateType.Dead; }
}
