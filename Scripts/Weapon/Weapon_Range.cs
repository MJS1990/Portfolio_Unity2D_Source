using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using UnityEngine;

public class Weapon_Range : Weapon
{
    private Rigidbody2D rigid;
    private new BoxCollider2D collider;
    SpriteRenderer spriteRenderer;

    Effect muzzleEffect;

    //EWeaponType weaponType;

    public Projectile bullet;
    public Animator muzzleAnim;

    public int poolingCount = 30;
    public int fireCount = 1;
    public float RateOfFire = 1.0f;
    public float weight = 0.0f;

    float currentTime = 0.0f;

    [SerializeField]
    float bulletCollectTime = 5.0f;

    Vector2 muzzlePos;
    //Vector2 attachPos;

    [HideInInspector]
    public Queue<Projectile> bullets;
    List<Projectile> firedBullets;

    Vector3 targetPos;
    //bool bSearchTarget;

    int damage;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        //weaponType = EWeaponType.Gun;

        bullets = new Queue<Projectile>();
        firedBullets = new List<Projectile>();

        //attachPos = new Vector2(0, 0);

        muzzlePos = new Vector2(transform.position.x, transform.position.y);
        //muzzleEffect = GetComponent<Effect>();
        //muzzleEffect.SetAnim(muzzleAnim);

        for (int i = 0; i < poolingCount; i++)
        {
            Projectile newBullet = Instantiate(bullet, muzzlePos, Quaternion.identity);
            newBullet.gameObject.SetActive(false);
            bullets.Enqueue(newBullet);
        }

        //bSearchTarget = false;
    }

    void FixedUpdate()
    {
        currentTime += Time.deltaTime;

        //철구에 부탁됐다면 발사 시작
        if (bAttach)
        {
            //gameObject.transform.position = attachPos;
            //transform.localPosition = Vector3.zero;

            if (currentTime >= RateOfFire)
            {
                for (int i = 0; i <= fireCount; i++)
                    Fire();

                currentTime = 0.0f;
            }
        }

        //충돌한 투사체 보관
        for (int i = firedBullets.Count - 1; i >= 0; i--)
        {
            if (firedBullets[i].GetCollectTime() >= bulletCollectTime && firedBullets[i].impactEffect.bEnd) // || firedBullets[i].bImpact == true 
            {
                if (!firedBullets[i].impactEffect.bEnd) continue;

                Projectile p = firedBullets[i];
                p.gameObject.SetActive(false);
                bullets.Enqueue(p);
                firedBullets.RemoveAt(i);
            }
        }


        //if(transform.parent != null && transform.parent.gameObject.name == "Attachment")
        //{
        //    if (transform.parent.gameObject.GetComponent<SpriteRenderer>().flipX)
        //    {
        //        //spriteRenderer.flipX = true;
        //        //transform.position = new Vector3(transform.parent.transform.position.x, 0.0f, 0.0f);
        //    }
        //    else if (!transform.parent.gameObject.GetComponent<SpriteRenderer>().flipX)
        //    {
        //        //spriteRenderer.flipX = false;
        //        //transform.position = new Vector3(transform.parent.transform.position.x, 0.0f, 0.0f);
        //    }
        //}
    }

    [System.Obsolete]
    void Fire()
    {
        if (bullets.Count == 0) return;
        Projectile currentBullet = bullets.Dequeue();

        if(currentBullet.IsDirect())
        {
            Projectile_Direct proj = currentBullet.GetComponent<Projectile_Direct>();
            proj.transform.position = transform.position;
            proj.SetDir(transform.right);
            currentBullet.gameObject.SetActive(true);

            proj.ResetProjectile();
            proj.Fire();

            firedBullets.Add(proj);
        }
        else if(currentBullet.IsHoming())
        {
            Projectile_Homing proj = currentBullet.GetComponent<Projectile_Homing>();
            proj.transform.position = transform.position;
            proj.SetDir(transform.right);

            //if (bSearchTarget)
            //    proj.SetTargetPos(targetPos);

            currentBullet.gameObject.SetActive(true);

            proj.ResetProjectile();
            //proj.Fire();

            firedBullets.Add(proj);
        }

        //firedBullets.Add(currentBullet);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Attachment")
        {
            bAttach = true;

            if (attachParent)
            {
                //GameObject obj = Instantiate(Resources.Load<GameObject>("Player/Roph/Attachment"));
                //transform.parent = obj.transform;
                this.gameObject.transform.parent = attachParent.gameObject.transform;
            }

            //if (attachParent != null)
            //{
            //    transform.parent = attachParent.transform;
            //    
            //    //GameObject obj = Instantiate(Resources.Load<GameObject>("Prefabs/Item"));
            //    //obj.transform.parent = content.transform;
            //}

            //ContactPoint2D contact = collision.contacts[0];
            //attachPos = contact.point;

            //this.collider.isTrigger = true;

            //if (rigid)
            //{
            //    rigid.gravityScale = 0.0f;
            //    rigid.mass = 0.0f;
            //}
        }

        //if (col.gameObject.tag == "Monster_Common")
        //{
        //    //적hp감소, 죽음판정, 플레이어 경험치 증가
        //    Monster monster = col.gameObject.GetComponent<Monster>();
        //    //monster.GetDamage(damage, new Vector3(0.0f, 0.0f, 0.0f));
        //    monster.Hitted();
        //
        //    if (transform.parent != null && transform.parent.transform.parent != null)
        //    {
        //        PlayerStatus playerStatus = transform.parent.transform.parent.transform.parent.GetComponent<PlayerStatus>();
        //        playerStatus.SetExp(monster.status.exp);
        //    }
        //}
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.layer == 7)
        {
            targetPos = collider.transform.position;
            //bSearchTarget = true;
        }
        else
        {
            //targetPos = transform.position + new Vector3(20.0f, 0.0f, 0.0f);
            targetPos = Vector3.zero;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.layer == 7)
        {
            //bSearchTarget = false;
        }
    }
}