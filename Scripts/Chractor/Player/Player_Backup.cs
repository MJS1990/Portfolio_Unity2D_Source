using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.Animations;
using UnityEngine.UI;

public class Player_Backup : MonoBehaviour
{
    public int HP;
    public Image[] HPUI;
    int MaxHP;
    
    public int AttackDamage;
    
    public float MaxSpeed;
    public float JumpPower;
    
    public PolygonCollider2D colAttack1;
    public PolygonCollider2D colAttack2;
    public PolygonCollider2D colDashAttack;

    bool isAttack;
    float a1, a2, da;

    float DashSpeed;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    
    float h;

    public bool GetAttackCondition() { return isAttack; }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        
        DashSpeed = 0.0f;
        
        isAttack = false;

        colAttack1.enabled = false;
        colAttack2.enabled = false;
        colDashAttack.enabled = false;

        MaxHP = HP;
    }

    void Update()
    {
        InputMove();
        InputJump();

        if ((Input.GetButtonDown("Fire3") && anim.GetFloat("DashDuration") <= 1.0f))
        {
            InputDash();
        }
        else if (Input.GetButtonUp("Fire3") || anim.GetFloat("DashDuration") > 1.0f)
        {
            anim.SetBool("IsDash", false);
            rigid.velocity = new Vector2(0.0f, rigid.velocity.y);
            DashSpeed = 0.0f;
        }

        //대시공격
        if (Input.GetButtonDown("Fire1") && anim.GetBool("IsDash") == true)
        {
            anim.SetBool("IsDashAttack", true);
        }
        else
        {
            anim.SetBool("IsDashAttack", false);
        }
        
        InputAttack();
        CalcAttack();
    }

    private void FixedUpdate()
    {
        if (rigid.velocity.x > MaxSpeed + DashSpeed)
            rigid.velocity = new Vector2(MaxSpeed + DashSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < -MaxSpeed + DashSpeed)
            rigid.velocity = new Vector2(-MaxSpeed + DashSpeed, rigid.velocity.y);

        if (HP <= 0) OnDead();      
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Terrain")
        {
            anim.SetBool("IsJumping", false);
        }

        if(collision.gameObject.layer == 7 && isAttack == false) 
            OnDamaged(collision.gameObject.transform.position, 1);
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 7)
        {
            anim.speed = 0.0f;

            Invoke("ReturnAnimSpeed", 0.15f);
        }
    }
    
    void ReturnAnimSpeed()
    {
        anim.speed = 1.0f;
    }
     
    void OnDamaged(Vector2 enemyPos, int damage)
    {
        for(int i = 1; i <= damage; i++)
        {
            if (i > HP) break;

            int index = HP - i;
            HPUI[index].enabled = false;
        }

        HP -= damage;
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        int dir = transform.position.x - enemyPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dir, 0.6f) * 0.5f, ForceMode2D.Impulse);
        
        gameObject.layer = 6;
        anim.SetBool("IsHit", true);

        Invoke("OffDamaged", 0.8f);
    }
    
    void OffDamaged()
    {
        spriteRenderer.color = new Color(1, 1, 1, 1);
        anim.SetBool("IsHit", false);
        gameObject.layer = 0;
    }
    
    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }

    void InputMove()
    {
        if (gameObject.layer == 6) return;
        
        h = Input.GetAxisRaw("Horizontal") * 2.0f;
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);
        
        if (Input.GetButton("Horizontal"))
        {
            anim.SetBool("IsRunning", true);
        }
        else if (Input.GetButtonUp("Horizontal"))
        {
            anim.SetBool("IsRunning", false);
            rigid.velocity = new Vector2((rigid.velocity.normalized.x * 0.02f), rigid.velocity.y);
        }
        
        if (Input.GetButton("Horizontal"))
        {
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }
    }

    void InputJump()
    {
        if (gameObject.layer == 6) return;
        
        if (Input.GetButton("Fire2") && rigid.velocity.y == 0)
        {
            anim.SetBool("IsDash", false);
            anim.SetBool("IsJumping", true);
            rigid.AddForce(Vector2.up * JumpPower, ForceMode2D.Impulse);
        }
        anim.SetFloat("IsFalling", rigid.velocity.y);
    }

    void InputAttack()
    {
        if (gameObject.layer == 6) return;

        float a1 = anim.GetFloat("Attack1Duration");
        float a2 = anim.GetFloat("Attack2Duration");

        if (Input.GetButtonDown("Fire1"))
        {
            anim.SetBool("IsAttack", true);
        }
        else if (Input.GetButtonUp("Fire1"))
        {
            anim.SetBool("IsAttack", false);
        }
       
        if (a1 > 0.01f && a1 <= 1.0f)
        {
            if (Input.GetButtonDown("Fire1"))
                anim.SetBool("IsAttack2", true);
        }

        if ((a1 > 0.01f && a1 <= 1.0f) || (a2 > 0.01f && a2 <= 1.0f))
            rigid.velocity = Vector2.zero;
    }

    void InputDash()
    {
        if (gameObject.layer == 6) return;

        anim.SetBool("IsDash", true);
        
        if (spriteRenderer.flipX == false)
        {
            DashSpeed = 1.5f;
            rigid.AddForce(new Vector2(DashSpeed, 0.0f), ForceMode2D.Impulse);
        }
        else if (spriteRenderer.flipX == true)
        {
            DashSpeed = -1.5f;
            rigid.AddForce(new Vector2(DashSpeed, 0.0f), ForceMode2D.Impulse);
        }
    }

    void CalcAttack()
    {
        a1 = anim.GetFloat("Attack1Duration");
        a2 = anim.GetFloat("Attack2Duration");
        da = anim.GetFloat("DashAttackDuration");

        if (a1 > 1.1f || a2 > 1.1f) return;

        if (a1 >= (6.0f / 9.0f) && a1 <= 1.0f)
        {
            colAttack1.enabled = true;
            isAttack = true;

            if (spriteRenderer.flipX)
                colAttack1.gameObject.transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
            else
                colAttack1.gameObject.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        }
        else if (a1 < (6.0f / 9.0f) || a1 > 1.0f)
        {
            colAttack1.enabled = false;
            isAttack = false;
        }

        if (a2 >= 0.00001f && a2 <= 1.0f)
        {
            colAttack2.enabled = true;
            isAttack = true;

            if (spriteRenderer.flipX)
                colAttack2.gameObject.transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
            else
                colAttack2.gameObject.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        }
        else if (a2 > 1.0f)
        {
            colAttack2.enabled = false;
            isAttack = false;
        }
        
        if (da >= (4.0f / 10.0f) && da <= 1.0f)
        {
            colDashAttack.enabled = true;
            isAttack = true;

            if (spriteRenderer.flipX)
                colDashAttack.gameObject.transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
            else
                colDashAttack.gameObject.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        }
        else if (da < (5.0f / 10.0f) || da > 1.0f)
        {
            colDashAttack.enabled = false;
            isAttack = false;
        }
    }
    
    void OnDead()
    {
        anim.SetTrigger("IsDead");
    }
}