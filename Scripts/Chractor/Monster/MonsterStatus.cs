using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;


//TODO : player한정이 아니라 모든 캐릭터의 상태값을 관리하도록 변경
public class MonsterStatus : MonoBehaviour
{
    public enum ID
    {
        None = -1,
        Common_01 = 1001,
        Common_02,
        Common_03,
        Common_04
    }

    public enum EMonsterType
    {
        Common = 0,
        Elite,
        Boss,
    }
    
    public enum EWeaponType
    {
        Stone = 0,
        Saw,
        Fire,
        None
    }

    public struct EStatusValues
    {
        public int id;
        public EMonsterType monsterType;
        public EWeaponType weaponType;
        public int hpCount;
        public float gravity;
        public float velocity; //MoveSpeed
        public float acceleration; 
        public float accelerationMax;
    }

    Rigidbody2D rigid;

    public EStatusValues status;
    [HideInInspector]
    public ID id; //TODO : 추후에 private으로 변경

    //TODO : exp 테이블에 추가
    int exp = 30;
    int currentHp;
    int maxHp;
    bool bDead = false;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();

        status = new EStatusValues();
        ReadStatus((int)id);
        //SetStatus();

        currentHp = 10;// status.hpCount;
        maxHp = currentHp;

    }

    public void SetID(int newId)
    {
        id = (ID)newId;
    }

    public EStatusValues GetStatus()
    {
        return status;
    }

    //TODO : 테이블 항목 수정 후 완성
    private void SetStatus()
    {
        rigid.gravityScale = status.gravity;
    }
    public int GetExp() { return exp; }

    public int GetHp() { return currentHp; }
    public void SetDamage(int damage) { currentHp -= damage; }

    public bool ReadStatus(int id)
    {
        List<Dictionary<string, object>> Table = CSVReader.Read("Table_Enemy");

        for (int i = 0; i < Table.Count; i++)
        {
            if(id == (System.Int32)Table[i]["Id"])
            {
                status.monsterType = (EMonsterType)System.Enum.Parse(typeof(EMonsterType), Table[i]["MonsterType"].ToString());
                status.gravity = float.Parse(Table[i]["Gravity"].ToString());
                status.velocity = float.Parse(Table[i]["Velocity"].ToString());
                status.acceleration = float.Parse(Table[i]["Acceleration"].ToString());
                status.accelerationMax = float.Parse(Table[i]["AccelerationMax"].ToString());
                status.hpCount = (System.Int32)Table[i]["HpCount"];
                status.weaponType = (EWeaponType)System.Enum.Parse(typeof(EWeaponType), Table[i]["Weapon"].ToString());
    
                return true;
            }
        }

        return false;
    }

    //private void ReadStatus()
    //{
    //    List<Dictionary<string, object>> Table = CSVReader.Read("Table_Enemy");
    //
    //    for (int i = 0; i < Table.Count; i++)
    //    {
    //        EStatusValues val;
    //        val.id = (System.Int32)Table[i]["Id"];
    //        val.monsterType = (EMonsterType)System.Enum.Parse(typeof(EMonsterType), Table[i]["MonsterType"].ToString());
    //        val.gravity = float.Parse(Table[i]["Gravity"].ToString());
    //        val.velocity = float.Parse(Table[i]["Velocity"].ToString());
    //        val.acceleration = float.Parse(Table[i]["Acceleration"].ToString());
    //        val.accelerationMax = float.Parse(Table[i]["AccelerationMax"].ToString());
    //        val.hpCount = (System.Int32)Table[i]["HpCount"];
    //        val.weaponType = (EWeaponType)System.Enum.Parse(typeof(EWeaponType), Table[i]["Weapon"].ToString());
    //
    //        status.Add(val);
    //    }
    //}
}