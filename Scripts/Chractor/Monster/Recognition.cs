using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recognition : MonoBehaviour
{
    [HideInInspector]
    public bool bChase;

    void Awake()
    {
        bChase = false;
    }
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == 8)// && path.Count == 0)
                bChase = true;
    }
}
