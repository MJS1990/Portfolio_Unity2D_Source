using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    private static CombatManager instance = null;
    [HideInInspector]
    public bool bPlayerAttackHit = false;
    [HideInInspector]
    public bool bEnemyAttackHit = false;
    
    void Awake()
    {
        instance = this;
        //DontDestroyOnLoad(this.gameObject);
    }

    public static CombatManager Get()
    {
        if (!instance)
            return null;
    
        return instance;
    }
}
