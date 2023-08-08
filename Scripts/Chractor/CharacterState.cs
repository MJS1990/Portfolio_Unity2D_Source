using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;


public class CharacterState : MonoBehaviour
{
    public enum EStateType //TODO : getter, setter 만들고 애니메이션 상태값과 매치되도록
    {
        Idle = 0,
        Move,
        Action,
        Hitted,
        Dead,
    }
    EStateType currentState;

    private void Awake()
    {
        currentState = new EStateType();
        currentState = EStateType.Idle;
    }

    public EStateType GetCurrentState() { return currentState; }

    public void SetIdle() { currentState = EStateType.Idle; }
    public void SetMove() { currentState = EStateType.Move; }
    public void SetAction() { currentState = EStateType.Action; }
    public void SetHitted() { currentState = EStateType.Hitted; }
    public void SetDead() { currentState = EStateType.Dead; }

    public bool IsIdle() { return (currentState == EStateType.Idle); }
    public bool IsMove() { return (currentState == EStateType.Move); }
    public bool IsAction() { return (currentState == EStateType.Action); }
    public bool IsHitted() { return (currentState == EStateType.Hitted); }
    public bool IsDead() { return (currentState == EStateType.Dead); }
}
