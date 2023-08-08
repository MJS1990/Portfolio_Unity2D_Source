using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBar : MonoBehaviour
{
    int HP, Damage;

    public void Set(int hp, int damage)
    {
        HP = hp;
        Damage = damage;
    }

    public void OnDamage()
    {
        Vector3 vec = new Vector3((float)((Damage / HP) * transform.localScale.x), transform.localScale.y, transform.localScale.z);
        transform.localScale = vec;
        transform.Translate(vec * 0.5f);

    }
}
