using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 7 || collision.gameObject.layer == 8)
        {
            //Debug.Log("Attack");
        }
        //
        //if (collision.gameObject.layer == 9)
        //{
        //    int damage = collision.GetComponent<Monster>().GetAttackDamage();
        //    if (isAttack == false)
        //        OnDamaged(collision.gameObject.transform.position, damage);
        //}
    }
}
