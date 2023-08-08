using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

public class Attachment : MonoBehaviour
{
    [SerializeField]
    PlayerAction player;
    new Rigidbody2D rigidbody;
    new CircleCollider2D collider;
    SpriteRenderer spriteRenderer;

    public Vector2 weight = new Vector2(0.0f, -2.5f);

    Animator anim;

    public Effect effect;

    [SerializeField]
    int damage = 1;
    [HideInInspector]
    public bool bHit;
    [SerializeField]
    float hitStopTime;
    Vector3 stopPos;
    Vector3 rophStopPos;
    float time;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<CircleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();    
        anim = GetComponent<Animator>();

        //effect.gameObject.transform.localScale = new Vector3(3.0f, 3.0f, 1.0f);


        stopPos = new Vector3();
        rophStopPos = new Vector3();
        bHit = false;
        time = 0.0f;

    }

    private void FixedUpdate()
    {
        if(bHit)
        {
            time += Time.deltaTime;
            CameraShake.Get().ShakeRound();
        
            //transform.position = stopPos;
            //transform.parent.position = rophStopPos;
        
            if (time >= hitStopTime)
            {
                time = 0.0f;
                bHit = false;
            }
        }

        if(transform.parent != null && transform.parent.transform.parent != null)
        {
            if(transform.parent.transform.parent.gameObject.GetComponent<SpriteRenderer>().flipX)
                spriteRenderer.flipX = true;
            else if(!transform.parent.transform.parent.gameObject.GetComponent<SpriteRenderer>().flipX)
                spriteRenderer.flipX = false;   
        }            
    }

    public Vector2 GetWeight()
    {
        return weight;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == 7 || col.gameObject.layer == 18) //Monster
        {
            bHit = true;
            stopPos = transform.position;

            //TODO : 몬스터 타입에 대한 반응 추가
            if(col.gameObject.tag == "Monster_Common")
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
                if (monster.GetState().IsGroggy()) monster.GetState().SetDead();
                else monster.GetDamage(damage, dir);
            }
            else if (col.gameObject.tag == "Monster_Boss")
            {
                Vector3 dir = col.transform.position - transform.position;
                dir = dir.normalized;

                //적hp감소, 죽음판정, 플레이어 경험치 증가
                Monster_Boss monster = col.gameObject.GetComponent<Monster_Boss>();
                monster.GetDamage(damage, dir);

            }

            ////collision.gameObject.GetCompoenent<Enemy_Common>.isDamaged = true;
            //float imapactValue = collision.gameObject.GetComponent<Enemy_Common>.attachmentImpact;
            //
            //Vector3 dir = collision.gameObject.transform.position - transform.position;
            //dir.Normalize();
            //collision.gameObject.GetComponent<Rigidbody2D>.AddForce(new Vector2(dir.x, dir.y) * rigidbody.velocity * attachmentImpact, ForceMode2D.Impulse);

            //Animation
            //effect.gameObject.transform.position = collision.GetContact(0).point;
            //effect.Play();
        }

        //if (collision.gameObject.layer == 14) //Weapon
        //{
        //    ContactPoint2D contact = collision.contacts[0];
        //    Vector2 attachPos = contact.point;
        //    collision.gameObject.transform.position = attachPos;
        //}
    }
}
