using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;

//TODO : player한정이 아니라 모든 캐릭터의 상태값을 관리하도록 변경
public class Status : MonoBehaviour
{
    enum  EStateType //TODO : getter, setter 만들고 애니메이션 상태값과 매치되도록
    {
        Idle = 0,
    	Hitted,
    	Dead,
    	Action,
    	None
    };

    public enum EMonsterType
    {
        Common = 0,
        Elite,
        Boss
    }

    public enum EWeaponType
    {
        Stone = 0,
        Saw,
        Fire,
        None
    }

    public struct DT_StatusValues
    {
        public int Id;
        public EMonsterType MonsterType;
        public float Gravity;
        public float Velocity;
        public float Acceleration;
        public float AccelerationMax;
        public int HpCount;
        public EWeaponType WeaponType;
    }

    public DT_StatusValues DT_Status;
    public int MaxHP;
    public int HP;
    //public bool isIdle { get; set; }
    //public bool isHitted { get; set; }
    public bool isDead { get; set; }
    //public bool isAction { get; set; }
    
    public int AttackDamage;

    public float MaxSpeed;
    public float JumpPower;
    public float DashSpeed;
    
    //Table Test
    private void Start()
    {
        //List<Dictionary<string, object>> Table = CSVReader.Read("Table_Enemy");
        //for (int i = 0; i < Table.Count; i++)
        //{
        //print(Table[0]["Id"].ToString());
        //print(Table[0]["MonsterType"].ToString());
        //print(Table[0]["Gravity"].ToString());
        //print(Table[0]["Velocity"].ToString());
        //print(Table[0]["Acceleration"].ToString());
        //print(Table[0]["AccelerationMax"].ToString());
        //print(Table[0]["HpCount"].ToString());
        //print(Table[0]["Weapon"].ToString());    
        //}
    }


    private void Awake()
    {
        HP = MaxHP;
        DashSpeed = 0.0f;
        isDead = false;

    }
    
    public void ReadStatus(int id)
    {
        List<Dictionary<string, object>> Table = CSVReader.Read("Table_Enemy");

        for(int i = 0; i < Table.Count; i++)
        {
            if(id == (System.Int32)Table[i]["Id"])
            {
                DT_Status.Id = (System.Int32)Table[i]["Id"];
                DT_Status.MonsterType = (EMonsterType)System.Enum.Parse(typeof(EMonsterType), Table[i]["MonsterType"].ToString());
                DT_Status.Gravity = float.Parse(Table[i]["Gravity"].ToString());
                DT_Status.Velocity = float.Parse(Table[i]["Velocity"].ToString());
                DT_Status.Acceleration = float.Parse(Table[i]["Acceleration"].ToString());
                DT_Status.AccelerationMax = float.Parse(Table[i]["AccelerationMax"].ToString());
                DT_Status.HpCount = (System.Int32)Table[i]["HpCount"];
                DT_Status.WeaponType = (EWeaponType)System.Enum.Parse(typeof(EWeaponType), Table[i]["Weapon"].ToString());
            }
        }
    }

    //public void ReadStatus(int index)
    //{
    //    List<Dictionary<string, object>> Table = CSVReader.Read("Table_Enemy");
    //
    //    if (index > Table.Count) return;
    //    
    //    DT_Status.Id = (System.Int32)Table[index]["Id"];
    //    DT_Status.MonsterType = (EMonsterType)System.Enum.Parse(typeof(EMonsterType), Table[index]["MonsterType"].ToString());
    //    DT_Status.Gravity = float.Parse(Table[index]["Gravity"].ToString());
    //    DT_Status.Velocity = float.Parse(Table[index]["Velocity"].ToString());
    //    DT_Status.Acceleration = float.Parse(Table[index]["Acceleration"].ToString());
    //    DT_Status.AccelerationMax = float.Parse(Table[index]["AccelerationMax"].ToString());
    //    DT_Status.HpCount = (System.Int32)Table[index]["HpCount"];
    //    DT_Status.WeaponType = (EWeaponType)System.Enum.Parse(typeof(EWeaponType), Table[index]["Weapon"].ToString());
    //}
}
