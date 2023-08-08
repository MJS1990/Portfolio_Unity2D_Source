using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class Monster_Common : Monster
{
    int id { get; }

    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer sprite;

    MonsterState state;
    MonsterStatus status;

    [SerializeField]
    PlayerStatus player;

    int maxhp;
    int currenthp;
    public float attachmentImpact = 2.0f;
    public float moveSpeed = 5.0f;
    public float minSpeed = 5.0f;
    public float maxSpeed = 8.0f;
    public float moveOffset = 0.05f;
    public float moveForce = 0.0f;
    public float jumpPower = 5.0f;
    float moveOffsetTime = 0.0f;

    //Patrol
    LineRenderer patrolPath;
    int patrolIndex = 0;

    //PathFind
    Monster_Pathfinding pathfinder;
    List<Vector2Int> chasePath;
    Vector3 targetPos;
    Vector3 pathPointPos;
    Vector2 pathMoveOffset;
    int pathIndex = 0;
    bool bArrived = false;
    float chaseTime = 0.0f;

    bool bDamaged = false;
    //int takeDamage = 0;
    [SerializeField]
    float knockBackPower;
    Vector3 knockBackDir;
    [SerializeField]
    float knockbackTime;
    float kTime;

    //NoneDamage
    [SerializeField]
    float noneDamageTime = 0.0f;

    //Groggy
    

    //Test
    int PatrolCount = 0;
    int ChaseCount = 0;
    int MoveCount = 0;
    int HittedCount = 0;

    public Monster Get() { return this; }
    public MonsterStatus GetStatus() { return status; }
    public MonsterState GetState() { return state; }
    public Vector3 GetTargetPos() { return player.transform.position; }

    void Start()
    {
        //Components
        sprite = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        
        //State
        state = GetComponent<MonsterState>();
        
        //Status
        status = GetComponent<MonsterStatus>();
        maxhp = status.GetHp();
        currenthp = maxhp;
        //status.GetStatus(id);
        
        //Pathfinding
        chasePath = new List<Vector2Int>();
        targetPos = new Vector3();
        pathPointPos = new Vector3();
        //Patrol
        patrolPath = GetComponentInChildren<LineRenderer>();
        //patrolPath = GetComponent<LineRenderer>();
        
        knockBackDir = new Vector3();
        kTime = 0.0f;
    }

    private void Update()
    {
        //if (bDamaged)
        //{
        //    bDamaged = true;
        //
        //    kTime += Time.deltaTime;
        //    if (kTime >= knockbackTime)
        //    {
        //        bDamaged = false;
        //        kTime = 0.0f;
        //    }
        //
        //    transform.position += new Vector3(knockBackDir.x * knockBackPower * Time.deltaTime, knockBackDir.y * knockBackPower * Time.deltaTime, transform.position.z);
        //}
        
        if (moveSpeed <= 0)
            moveSpeed = UnityEngine.Random.Range(minSpeed, maxSpeed);
    }

    void FixedUpdate()
    {
        moveOffsetTime += Time.deltaTime;

        if (bDamaged)
        {
            bDamaged = true;
            //NoneDamaged(2.0f);

            kTime += Time.deltaTime;
            if (kTime >= knockbackTime)
            {
                bDamaged = false;
                kTime = 0.0f;
            }

            transform.position += new Vector3(knockBackDir.x * knockBackPower * Time.deltaTime, knockBackDir.y * knockBackPower * Time.deltaTime, transform.position.z);
        }

        if (GetStatus().GetHp() <= 0) state.SetGroggy();
    }

    
    public override bool MoveTo()
    {
        Vector3 dir = targetPos - transform.position;
        dir.Normalize();
        transform.position += dir * moveSpeed * Time.deltaTime;
        //rigid.AddForce(dir * moveSpeed, ForceMode2D.Force); //Force
        moveSpeed -= 0.3f;

        ////Jump
        //if (target.y > transform.position.y)
        //    rigid.AddForce(dir * jumpPower, ForceMode2D.Impulse);
        //else if (target.y < transform.position.y)
        //    rigid.AddForce(-dir * jumpPower, ForceMode2D.Impulse);

        //이동 도착위치 설정
        Vector3 min = new Vector3(targetPos.x - moveOffset, targetPos.y - moveOffset, targetPos.z);
        Vector3 max = new Vector3(targetPos.x + moveOffset, targetPos.y + moveOffset, targetPos.z);
        //도착했다면 true
        if ((transform.position.x > min.x && transform.position.y > min.y) && (transform.position.x < max.x && transform.position.y < max.y))
        {
            state.SetIdle();
            return true;
        }

        MoveCount++;

        return false;
    }

    public override bool Patrol()
    {
        if (patrolPath == null || patrolPath.positionCount <= 0)
            return false;

        targetPos = patrolPath.GetPosition(patrolIndex);

        if (MoveTo())
        {
            if (patrolIndex == (patrolPath.positionCount - 1))
                patrolIndex = 0;
            else
                patrolIndex++;
        }

        PatrolCount++;

        return true;
    }

    public override bool Chase()
    {
        state.SetPathfind();
        //if (!state.IsPathfind() || pathIndex > (chasePath.Count - 1))
        //{
        //    state.SetIdle();
        //    pathIndex = 0;
        //    chasePath.Clear();
        //    return false;
        //}

        if (chasePath.Count <= 0)
        {
            chasePath = pathfinder.ReFindPath(player.transform.position);
            pathIndex = 0;
        }

        pathMoveOffset.x = UnityEngine.Random.Range(-0.5f, 0.5f);
        pathMoveOffset.y = UnityEngine.Random.Range(-0.5f, 0.5f);

        float x = (float)chasePath[pathIndex].x + pathMoveOffset.x;
        float y = (float)chasePath[pathIndex].y + pathMoveOffset.y;
        pathPointPos = new Vector3(x, y, 0.0f);

        targetPos = pathPointPos;
        bArrived = MoveTo();

        if (bArrived)
        {
            if (pathIndex == chasePath.Count - 1)
            {
                chasePath.Clear();
                pathIndex = 0;
                //state.SetIdle();
            }

            pathIndex++;

        }

        ChaseCount++;

        return true;
    }
    public override void GetDamage(int damage, Vector3 dir)
    {
        knockBackDir = dir;
        bDamaged = true;
        state.SetHitted();
        status.SetDamage(damage);
    }

    public override bool Hitted()
    {
        if (status.GetHp() <= 0 && !state.IsGroggy())
        {
            state.SetGroggy();
            return false;
        }
        else
            End_Hitted();

        return true;
    }

    public override void End_Hitted()
    {
        state.SetIdle();
    }

    public override bool NoneDamaged(float time)
    {
        if(!state.IsGroggy())
        {
            gameObject.layer = 6;
        }


        noneDamageTime += Time.deltaTime;

        if (sprite.color.a == 1.0f)
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0.3f);
        else
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1.0f);

        if (time != 0 && (noneDamageTime >= time || !state.IsGroggy()))
        {

            if (sprite.color.a != 1.0f)
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1.0f);
            gameObject.layer = 7;
            noneDamageTime = 0.0f;

            return true;
        }

        return false;
    }

    public override bool Groggy()
    {
        gameObject.layer = 18;
        NoneDamaged(0);
        rigid.gravityScale = 1.0f;
        rigid.mass = 1.0f;

        return true;
    }

    public override bool Dead()
    {
        print("Dead");

        //애니메이션 재생
        PlayerStatus playerStatus = player.gameObject.GetComponent<PlayerStatus>();
        playerStatus.SetExp(status.GetExp());

        End_Dead();

        return true;
    }

    public override void End_Dead()
    {
        Destroy(this.gameObject); //2번째 파라미터 : 함수 콜 지연시간
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Attachment" || col.gameObject.tag == "PlayerAttack") //Attachment
        {
            bDamaged = true;
            //state.SetHitted();
            CombatManager.Get().bPlayerAttackHit = true;

            //Vector3 dir = transform.position - col.gameObject.transform.position;
            //dir.Normalize();
            ////Vector2 vel = collision.gameObject.GetComponent<Rigidbody2D>().velocity;
            //rigid.AddForce(new Vector2(dir.x, dir.y) * attachmentImpact, ForceMode2D.Impulse);
        }

        if (col.gameObject.layer == 13) //Bullet
        {
            //state.SetHitted();

            //bDamaged = true;
            //CombatManager.Get().bPlayerAttackHit = true;
            //
            //Vector3 dir = transform.position - col.gameObject.transform.position;
            //dir.Normalize();
            ////Vector2 vel = collision.gameObject.GetComponent<Rigidbody2D>().velocity;
            //rigid.AddForce(new Vector2(dir.x, dir.y) * 0.5f, ForceMode2D.Force);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            targetPos = col.transform.position;
            if(state != null)
                state.SetMove();
        }
    }
    
    private void OnTriggerStay2D(Collider2D col)
    {
        if (!state.IsMove() && col.gameObject.tag == "Player" && moveOffsetTime >= 0.5f && !state.IsGroggy())
        {
            moveOffsetTime = 0.0f;
            targetPos = col.transform.position;
            state.SetMove();
        }
    }
    
    private void OnTriggerExit2D(Collider2D col)
    {
        if (state.IsMove() && col.gameObject.tag == "Player")
        {
            moveOffsetTime = 0.0f;
            targetPos = patrolPath.GetPosition(0);
            state.SetIdle();
        }
    }
}
