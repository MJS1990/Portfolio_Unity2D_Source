using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    protected Rigidbody2D rigid;
    new protected BoxCollider2D collider;
    protected SpriteRenderer spriteRenderer;
    protected TrailRenderer trail;
    protected Animator anim;
    public Effect impactEffect;

    [SerializeField]
    public int damage = 1;
    public struct DT_ProjectileData
    {
        public int id;
        public string groupKey;
        public string type;
        public int level;
        public bool isMaxLevel;
        public int attackDamage;
        public float Velocity;
        public float scale;
        public int spawnCount;
        public float spawnPeriod;
        public float angleErrorRate;
        public float damageRadius;
        public float weight;
    }
    List<DT_ProjectileData> projectileData;
    public enum EProjectileType
    {
        Direct = 0,
        Gravity,
        Homing,
    }
    public EProjectileType fireType;// { get; set; }

    public float speed = 1.0f;
    protected float spreadAngle = 0.0f;
    [HideInInspector]
    public bool bImpact;
    
    protected Vector3 dir;
    protected Vector3 offset;

    protected float collectTime = 0.0f;

    public float GetCollectTime() { return collectTime; }

    public virtual void ResetProjectile() {}
    public void SetTargetPos(Vector3 vec) {}
    public void SetDir(Vector3 vec) { dir = vec; }
    public bool IsDirect() { return fireType == EProjectileType.Direct ? true : false; }
    public bool IsHoming() { return fireType == EProjectileType.Homing ? true : false; }
    public virtual void Fire() {}

    public void ReadProjectileTable()
    {
        List<Dictionary<string, object>> projectileTable = CSVReader.Read("Table_Projectile");

        projectileData = new List<DT_ProjectileData>();

        for (int i = 0; i < projectileTable.Count; i++)
        {
            DT_ProjectileData data;

            data.id = (System.Int32)projectileTable[i]["Id"];
            data.groupKey = projectileTable[i]["GroupKey"].ToString();
            data.type = projectileTable[i]["Type"].ToString();
            data.level = (System.Int32)projectileTable[i]["Level"];
            data.isMaxLevel = false;
            data.attackDamage = (System.Int32)projectileTable[i]["Atk"];
            data.Velocity = (float)projectileTable[i]["Velocity"];
            data.scale = (float)projectileTable[i]["Scale"];
            data.spawnCount = (System.Int32)projectileTable[i]["SpawnCount"];
            data.spawnPeriod = (float)projectileTable[i]["SpawnPeriod"];
            data.angleErrorRate = (float)projectileTable[i]["AngleErrorRate"];
            data.damageRadius = (float)projectileTable[i]["DamageRadius"];
            data.weight = (float)projectileTable[i]["Weight"];

            projectileData.Add(data);
        }
    }
}