using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Homing : Projectile
{
    bool bGetTarget = false;
    float currentTime = 0.0f;
    float searchTime = 0.0f;
    public float searchInterval = 2.5f;
    public float homingStartTime = 1.0f;
    float rTime = 0.0f;
    [SerializeField]
    float homingSpeed = 5.0f;
    float distance = 0.0f;
    Vector3 targetPos;
    float tempSpeed = 0.0f;

    Vector2[] BezierControlPoint = new Vector2[8];
    [SerializeField]
    public float posA = 0.55f;
    [SerializeField]
    public float posB = 0.45f;
    [SerializeField][Range(0, 1)] private float t = 0;


    void Awake()
    {
        fireType = EProjectileType.Homing;

        rigid = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (anim)
            anim = GetComponent<Animator>();

        trail = GetComponent<TrailRenderer>();
        impactEffect = GetComponentInChildren<Effect>();

        dir = transform.right;
        bImpact = false;

        //offset = new Vector3(1.0f, 0.0f, 0.0f);

        //gameObject.transform.position += offset;
        //impactEffect.gameObject.transform.position += offset;
    }

    void FixedUpdate()
    {
        currentTime = Time.deltaTime;
        rTime += Time.deltaTime * homingSpeed;
        collectTime += Time.deltaTime;
        searchTime += Time.deltaTime;

        if (!bGetTarget)
            transform.position += new Vector3(dir.x * speed, dir.y * speed, 0.0f) * currentTime;
        else
        {
            Fire();
        }
    }

    [System.Obsolete]
    public new void ResetProjectile()
    {
        transform.rotation = Quaternion.identity;
        bImpact = false;
        bGetTarget = false;
        collectTime = 0.0f;
        rTime = 0.0f;

        ////Homing
        //targetPos = new Vector3(50.0f - transform.position.x, transform.position.y, 0.0f); //TODO : 50은 맵의 x길이 추후 변수로 변경할것

        //Test
        BezierControlPoint[0] = new Vector2(transform.position.x, transform.position.y);
        BezierControlPoint[1] = GetBezierPoint(new Vector2(transform.position.x, transform.position.y));
        BezierControlPoint[2] = GetBezierPoint(targetPos);
        BezierControlPoint[3] = targetPos;

        if (!spriteRenderer.enabled)
            spriteRenderer.enabled = true;
        if (!gameObject.active)
            gameObject.SetActive(true);
    }

    public new void SetTargetPos(Vector3 vec)
    {
        bGetTarget = true;
        targetPos = vec;
        distance = Vector3.Distance(transform.position, targetPos);
        dir = (targetPos - transform.position).normalized;

        //BezierControlPoint[0] = new Vector2(transform.position.x, transform.position.y);
        //BezierControlPoint[1] = GetBezierPoint(new Vector2(transform.position.x, transform.position.y));
        //BezierControlPoint[2] = GetBezierPoint(targetPos);
        //BezierControlPoint[3] = targetPos;
    }

    public override void Fire()
    {
        if (bImpact) return;

        if (bGetTarget)
        {
            //Test
            BezierControlPoint[0] = new Vector2(transform.position.x, transform.position.y);
            
            //BezierControlPoint[1] = GetBezierPoint(new Vector2(transform.position.x, transform.position.y));
            //BezierControlPoint[2] = GetBezierPoint(targetPos);
            BezierControlPoint[1] = new Vector2(transform.position.x, transform.position.y);
            BezierControlPoint[2] = targetPos;
            
            BezierControlPoint[3] = targetPos;

            transform.position = new Vector2(
            FourPointBezier(BezierControlPoint[0].x, BezierControlPoint[1].x, BezierControlPoint[2].x, BezierControlPoint[3].x),
            FourPointBezier(BezierControlPoint[0].y, BezierControlPoint[1].y, BezierControlPoint[2].y, BezierControlPoint[3].y));


            //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 10.0f);
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    private float FourPointBezier(float a, float b, float c, float d)
    {
        return Mathf.Pow((1 - rTime), 3) * a
                + Mathf.Pow((1 - rTime), 2) * 3 * rTime * b
                + Mathf.Pow(rTime, 2) * 3 * (1 - rTime) * c
                + Mathf.Pow(rTime, 3) * d;
    }

    private Vector2 GetBezierPoint(Vector2 v)
    {
        float x, y;
        x = posA * Mathf.Cos(UnityEngine.Random.Range(0, 360) * Mathf.Deg2Rad) + v.x;
        y = posB * Mathf.Sin(UnityEngine.Random.Range(0, 360) * Mathf.Deg2Rad) + v.y;
        return new Vector2(x, y);
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
            searchTime = 0.0f;
            SetTargetPos(col.transform.position);
        }

        if (col.gameObject.layer == 7)
        {
            //적hp감소, 죽음판정, 플레이어 경험치 증가
            Monster monster = col.gameObject.GetComponent<Monster>();
            monster.GetDamage(damage, new Vector3(0.0f, 0.0f, 0.0f));
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.layer == 7 && searchTime > searchInterval)
        {
            searchTime = 0.0f;
            SetTargetPos(col.transform.position);
        }
    }
}
