using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class TouchControl : MonoBehaviour
{
    [SerializeField]
    PlayerAction action;

    Button button;
    void Start()
    {
        button = GetComponent<Button>();
    }

    void FixedUpdate()
    {
    }

    public void ClickButtonLT()
    {
        action.InputLeftTop();
    }

    public void ClickButtonRT()
    {
        action.InputRightTop();
    }
}
