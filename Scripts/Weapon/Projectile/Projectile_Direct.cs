using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Projectile_Direct : Projectile
{
    private void Awake()
    {
        fireType = EProjectileType.Direct;

        rigid = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (anim)
            anim = GetComponent<Animator>();

        trail = GetComponent<TrailRenderer>();
        impactEffect = GetComponentInChildren<Effect>();

        dir = transform.right;
        bImpact = false;
    }
    
    private void FixedUpdate()
    {
        collectTime += Time.deltaTime;
    }

    [System.Obsolete]
    public new void ResetProjectile()
    {
        collectTime = 0.0f;
        transform.rotation = Quaternion.identity;

        bImpact = false;

        if(!spriteRenderer.enabled)
            spriteRenderer.enabled = true;
        if(!gameObject.active)
            gameObject.SetActive(true);
    }

    public override void Fire()
    {
        if (bImpact) return;

        rigid.AddForce(dir * speed, ForceMode2D.Force);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (impactEffect != null)
            impactEffect.Play(transform.position);
        
        bImpact = true;
        spriteRenderer.enabled = false;
        //gameObject.SetActive(false);

        if (col.gameObject.layer == 7)
        {
            //적hp감소, 죽음판정, 플레이어 경험치 증가
            Monster_Common monster = col.gameObject.GetComponent<Monster_Common>();

            if (monster.GetState().IsGroggy())
                return;
            
            monster.GetDamage(damage, new Vector3(0.0f, 0.0f, 0.0f));
        }
    }

    private void OnTriggerStay2D(Collider2D collider)
    {

    }
}
