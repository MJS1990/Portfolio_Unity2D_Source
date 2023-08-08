using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerEvent : MonoBehaviour
 {
    PlayerStatus status;

    public UnityEvent MoveSceneEvent;
    public UnityEvent GameOverEvent;

    public UnityEvent LevelUpEvent;

    private void Start()
    {
        status = GetComponent<PlayerStatus>();
    }

    private void Update()
    {
        if (status.GetCurrentHP() <= 0)
        {
            GameOverEvent.Invoke();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Finish")
           MoveSceneEvent.Invoke();
    }
}