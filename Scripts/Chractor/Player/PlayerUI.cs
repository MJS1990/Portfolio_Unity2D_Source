using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    PlayerStatus status;

    public Image[] HPUI;
    int index;
    int prev;

    [SerializeField]
    Image[] ItemImages;
        

     void Awake()
     {
        status = GetComponent<PlayerStatus>();
       
         index = status.GetCurrentHP() - 1;
         prev = status.GetCurrentHP();
     }

     void FixedUpdate()
     {
         if (status.GetCurrentHP() != prev && status.GetCurrentHP() >= 0)
         {
             HPUI[index].enabled = false;
             index--;
             prev--;
         }
     }
}
