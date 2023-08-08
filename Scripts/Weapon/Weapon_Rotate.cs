using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Rotate : Weapon
{
    private Rigidbody2D rigid;
    private BoxCollider2D collider;

    //EWeaponType weaponType;

    public float distance = 1.0f;
    public float rotateSpeed = 1.0f;
    
    float angle;


    [SerializeField]
    int damage = 1;
    [HideInInspector]
    public bool bHit;
    [SerializeField]
    float hitStopTime;
    Vector3 stopPos;
    Vector3 rophStopPos;
    float time;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();

        //weaponType = EWeaponType.Rotate; 
        angle = 30.0f;
    }

    void FixedUpdate()
    {
        if (bAttach)
            CalcRadius();
    }

    private void CalcRadius()
    {
        angle += Time.deltaTime * rotateSpeed;
        if (angle <= 0 || angle >= 180)
            rotateSpeed *= -1;

        transform.position = new Vector2(attachParent.transform.position.x + (Mathf.Sin(angle) * distance), attachParent.transform.position.y + (Mathf.Cos(angle) * distance));
    }


    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            bAttach = true;
            transform.parent = attachParent.transform;

            if (rigid)
                rigid.gravityScale = 0.0f;
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == 7 || col.gameObject.layer == 18) //Monster
        {
            bHit = true;
            stopPos = transform.position;

            //TODO : 몬스터 타입에 대한 반응 추가
            if (col.gameObject.tag == "Monster_Common")
            {
                print("Hit!");
                Vector3 dir = col.transform.position - transform.position;
                dir = dir.normalized;

                //적hp감소, 죽음판정, 플레이어 경험치 증가
                Monster_Common monster = col.gameObject.GetComponent<Monster_Common>();
                if (monster.GetState().IsGroggy()) monster.GetState().SetDead();
                else monster.GetDamage(damage, dir);

            }
            else if (col.gameObject.tag == "Monster_Elite")
            {
                Vector3 dir = col.transform.position - transform.position;
                dir = dir.normalized;

                //적hp감소, 죽음판정, 플레이어 경험치 증가
                Monster_Elite monster = col.gameObject.GetComponent<Monster_Elite>();
                monster.GetDamage(damage, dir);

                if (monster.GetState().IsGroggy()) monster.GetState().SetDead();
            }
            else if (col.gameObject.tag == "Monster_Boss")
            {
                Vector3 dir = col.transform.position - transform.position;
                dir = dir.normalized;

                //적hp감소, 죽음판정, 플레이어 경험치 증가
                Monster_Boss monster = col.gameObject.GetComponent<Monster_Boss>();
                monster.GetDamage(damage, dir);

            }
        }
    }

}