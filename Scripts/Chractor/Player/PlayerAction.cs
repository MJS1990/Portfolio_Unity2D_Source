using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

class PlayerAction : MonoBehaviour
{
    PlayerStatus status;

    float h;
    Rigidbody2D Rigidbody;
    SpriteRenderer SpriteRenderer;
    Animator Anim;

    [SerializeField]
    private float moveSpeed = 1.0f;
    [SerializeField]
    private float jumpPower = 1.0f;
    [SerializeField]
    private float stopTime = 1.5f;

    //hitStop
    [SerializeField]
    private Attachment attachment;
    Vector3 stopPos;


    //Test/////////////////////////////////////////////////
    Vector3 pos;
    Vector2 prevPos;
    bool isAttacking;

    public Effect effect;
    ////////////////////////////////////////////////////////

    private void Awake()
    {
        status = GetComponent<PlayerStatus>();
        
        Rigidbody = GetComponent<Rigidbody2D>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        Anim = GetComponent<Animator>();

        isAttacking = false;

        stopPos = new Vector3();
        //if(clips[0] != null)
        //{
        //    animation.clip = clips[0];
        //    //animation.clip.legacy = true;
        //}
    }

    public bool FlipX() { return SpriteRenderer.flipX; }
    public bool FlipY() { return SpriteRenderer.flipY; }

    void Update()
    {
        InputMove();
        InputJump();
        if (Input.GetButtonDown("LeftJump"))
            InputLeftTop();
        if (Input.GetButtonDown("RightJump"))
            InputRightTop();
    }

    void FixedUpdate()
    {
        if(attachment.bHit)
        {
            transform.position = stopPos;
        }
        else
        {
            stopPos = transform.position;
        }

        //if (player.HP <= 0) StartCoroutine(Dead());

        if (CombatManager.Get().bPlayerAttackHit)
        {
            //OnAttackHit();
            StartCoroutine(OnAttackHit());
            //Invoke("OffAttackHit", 0.7f);
        }

        //if (Input.GetButton("TestButton1"))
        //    animation.Play("Smoke");
    }

    public Vector3 GetPosition()
    {
        return gameObject.transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Terrain")
        {
            //Anim.SetBool("IsJumping", false);
            Anim.SetBool("IsFlying", false);
            gameObject.layer = 8;
        }

        if (collision.gameObject.layer == 7) //Monster
        {
            //int dir = transform.position.x - collision.gameObject.transform.position.x > 0 ? 1 : -1;
            Vector2 dir = transform.position - collision.gameObject.transform.position;

            //Rigidbody.AddForce(new Vector2(dir, 0.6f) * 0.7f, ForceMode2D.Impulse);
            Rigidbody.AddForce(dir * 0.7f, ForceMode2D.Impulse);
        }

        if (collision.gameObject.layer == 9) //MonsterAttack
            OnDamaged(collision.gameObject.transform.position, 1);
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.gameObject.layer == 7)
    //    {
    //        Anim.speed = 0.0f;
    //
    //        Invoke("ReturnAnimSpeed", 0.15f);
    //    }
    //
    //    if (collision.gameObject.layer == 9)
    //    {
    //        int damage = collision.GetComponent<Monster>().GetAttackDamage();
    //        if (isAttack == false)
    //            OnDamaged(collision.gameObject.transform.position, damage);
    //    }
    //}

    //void ReturnAnimSpeed()
    //{
    //    Anim.speed = 1.0f;
    //}
    
    public void InputMove()
    {
        if (gameObject.layer == 6 || isAttacking) return;

        h = Input.GetAxisRaw("Horizontal") * 0.003f * moveSpeed; //TODO : 임시변수
        Rigidbody.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        if (Input.GetButton("Horizontal"))
        {
            Anim.SetBool("IsRunning", true);
            SpriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == 1;
        }
        else if (Input.GetButtonUp("Horizontal"))
        {
            Anim.SetBool("IsRunning", false);

            if (Anim.GetBool("IsFlying") == false)
                Rigidbody.velocity = new Vector2((Rigidbody.velocity.normalized.x * 0.02f), Rigidbody.velocity.y);
        }
    }

    public void InputLeftTop()
    {
        if (gameObject.layer == 6) return;

        Anim.SetBool("IsFlying", true);
        SpriteRenderer.flipX = false;

        Rigidbody.AddForce(new Vector2(-moveSpeed * 0.5f, jumpPower * 5), ForceMode2D.Impulse);
        //Rigidbody.AddForce(Vector2.right * -moveSpeed, ForceMode2D.Impulse);
        //Rigidbody.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        //Rigidbody.velocity = new Vector2((Rigidbody.velocity.normalized.x * 0.02f), Rigidbody.velocity.y);
    }

    public void InputRightTop()
    {
        if (gameObject.layer == 6) return;

        Anim.SetBool("IsFlying", true);
        SpriteRenderer.flipX = true;

        Rigidbody.AddForce(new Vector2(moveSpeed * 0.5f , jumpPower * 5), ForceMode2D.Impulse);
        //Rigidbody.AddForce(Vector2.right * moveSpeed, ForceMode2D.Impulse);
        //Rigidbody.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        //Rigidbody.velocity = new Vector2((Rigidbody.velocity.normalized.x * 0.02f), Rigidbody.velocity.y);
    }

    void InputJump()
    {
        if (gameObject.layer == 6 || isAttacking) return;

        if (Input.GetButtonDown("Fire2"))
        {
            //Anim.SetBool("IsDash", false);
            Anim.SetBool("IsJumping", true);
            Anim.SetBool("IsFlying", false);
            Anim.SetBool("IsRunning", false);

            if (gameObject.layer == 10)
                Rigidbody.AddForce(Vector2.up * status.GetStatus().JumpPower / 2, ForceMode2D.Impulse);
            else
                Rigidbody.AddForce(Vector2.up * status.GetStatus().JumpPower, ForceMode2D.Impulse);

            gameObject.layer = 10;
        }
        else if (Input.GetButtonUp("Fire2"))
        {
            Anim.SetBool("IsJumping", false);
            Anim.SetBool("IsFlying", true);
        }
        //Anim.SetFloat("IsFalling", Rigidbody.velocity.y);
    }

    IEnumerator OnAttackHit()
    {
        isAttacking = true;
        prevPos = Rigidbody.velocity;
        Rigidbody.velocity = Vector2.zero;
        CameraShake.Get().SetbShake(true);

        CombatManager.Get().bPlayerAttackHit = false;


        yield return new WaitForSeconds(stopTime);
        isAttacking = false;
        Rigidbody.velocity = prevPos;
    }

    void OffAttackHit()
    {
        Rigidbody.velocity = prevPos;
    }

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////


    //public IEnumerator Dead()
    //{
    //    Anim.SetTrigger("IsDead");
    //    
    //    yield return new WaitForSeconds(0.82f);
    //    Anim.speed = 0.0f;
    //    player.isDead = true; 
    //    
    //    yield return true;
    //}

    void OnDamaged(Vector2 enemyPos, int damage)
    {
        for (int i = 1; i <= damage; i++)
        {
            if (i > status.GetCurrentHP()) break;

            int index = status.GetCurrentHP() - i;
        }

        status.currentHP -= damage;
        SpriteRenderer.color = new Color(1, 1, 1, 0.4f);

        int dir = transform.position.x - enemyPos.x > 0 ? 1 : -1;
        Rigidbody.AddForce(new Vector2(dir, 0.6f) * 0.5f, ForceMode2D.Impulse);

        gameObject.layer = 6;
        Anim.SetBool("IsHit", true);

        Invoke("OffDamaged", 0.8f);
    }

    void OffDamaged()
    {
        SpriteRenderer.color = new Color(1, 1, 1, 1);
        Anim.SetBool("IsHit", false);
        gameObject.layer = 0;
    }
}
