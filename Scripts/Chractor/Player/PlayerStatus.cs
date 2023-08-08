using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;

public class PlayerStatus : MonoBehaviour
{
    public struct DT_Status
    {
        public int Id;
        public float Gravity;
        public float Velocity;
        public float JumpPower;
        public float Acceleration;
        public float AccelerationMax;
        public int HpCount;
    }

    public struct DT_ExpData
    {
        public int level;
        public int expToNextLv;
        public int rewardGroup;
        public int rewardCount;
    }

    public enum ERewardType
    {
        Weapon = 0,
        Projectile,
        PlayerHp,
    }

    public struct DT_RewardData
    {
        public int id;
        public int rewardGroup;
        public ERewardType rewardType;
        public string rewardParam;
        public int rewardPercentage;
    }


    DT_Status status;
    public int currentHP = 5;

    //List<DT_ExpData> expDatas;
    DT_ExpData[] expDatas;

    Queue<DT_RewardData> rewardDatas;
    //List<DT_RewardData> rewardDatats;

    List<DT_RewardData> rewardList;

    //UI
    public LevelUpScene lvUI;


    int level = 1;
    int maxLevel = 10;
    int currentExp = 0;
    int levelUpCount = 0;

    //임시 능력치
    [HideInInspector]
    public float jumpPower;
    [HideInInspector]
    public int hpCount;

    public DT_Status GetStatus() { return status; }
    public int GetCurrentHP() { return currentHP; }
    public int GetLevel() { return level; }
    public int GetExp() { return currentExp;}

    private void Awake()
    {
        expDatas = new DT_ExpData[maxLevel];
        //expDatas = new List<DT_ExpData>();
        ReadExpTable();
        

        rewardDatas = new Queue<DT_RewardData>();
        ReadRewardTable();
        rewardList = new List<DT_RewardData>();

        //ReadStatus(status);
        //currentHP = status.HpCount;

        //임시 능력치
        status.JumpPower = 8;//jumpPower;
        status.HpCount = hpCount;
    }
    public void SetExp(int exp)
    {
        if (level >= maxLevel) return;

        currentExp += exp;
        print("CurrentEXP : " + currentExp);
        if (currentExp >= expDatas[level - 1].expToNextLv)
        {
            levelUpCount++;
            ////TODO : 레벨업시 능력치 증가, 보상코드 추가
            //LevelUp();
        }
    }

    private void FixedUpdate()
    {
        //if (level <= maxLevel && currentExp >= expDatas[level - 1].expToNextLv)
        //    LevelUp();

        if (level <= maxLevel && levelUpCount > 0 && !lvUI.bAciveUI)
            LevelUp();
    }

    public void LevelUp()
    {
        if (expDatas[level].rewardCount == 0 || level > maxLevel) return;

        //오른 레벨에 맞는 보상리스트 가져오기
        for (int i = 0; i < rewardDatas.Count(); i++)
        {
            if (rewardDatas.Peek().rewardGroup != expDatas[level].rewardGroup)
                break;

            rewardList.Add(rewardDatas.Dequeue());
        }

        //확률 높은순으로 보상리스트 정렬
        rewardList = rewardList.OrderByDescending(x => x.rewardPercentage).ToList();
        List<int> rewardId = new List<int>(); //UI에 넘겨줄 i값

        for(int i = 0; i < expDatas[level].rewardCount; i++)
        {
            //print("id : " + rewardList[i].id);
            //print("RewardGroup : " + rewardList[i].rewardGroup);
            //print("RewardType : " + rewardList[i].rewardType);
            //print("RewardPercentage : " + rewardList[i].rewardPercentage);
            //print("RewardParam : " + rewardList[i].rewardParam);
            //print("==========================================");
        
            rewardId.Add(rewardList[i].id);
            //switch (rewardList[i].rewardType)
            //{ 
            //    case (ERewardType.Weapon):
            //        {
            //            
            //            break;
            //        }
            //    case (ERewardType.Projectile):
            //        {
            //
            //
            //            break;
            //        }
            //    case (ERewardType.PlayerHp):
            //        {
            //            status.HpCount++;
            //
            //            break;
            //        }            
            //} //switch()
        } //for()

        lvUI.OnGrowthScene(rewardId);
 
        rewardList.Clear();
        levelUpCount--;
        level++;
    }

    ////TODO : 플레이어 능력치 테이블 만든 후 수정
    //public void ReadStatus()
    //{
    //    List<Dictionary<string, object>> statusTable = CSVReader.Read("Table_Player");
    //
    //    //Read Status
    //    status.Id = (System.Int32)statusTable["Id"];
    //    status.Gravity = float.Parse(statusTable["Gravity"]);
    //    status.Velocity = float.Parse(statusTable["Velocity"]);
    //    status.JumpPower = float.Parse(statusTable["JumpPower"]);
    //    status.Acceleration = float.Parse(statusTable["Acceleration"]);
    //    status.AccelerationMax = float.Parse(statusTable["AccelerationMax"]);
    //    status.HpCount = (System.Int32)statusTable["HpCount"];
    //}

    public void ReadExpTable()
    {
        List<Dictionary<string, object>> levelUpTable = CSVReader.Read("Table_Player_Exp");

        for (int i = 0; i < levelUpTable.Count; i++)
        {
            DT_ExpData temp = new DT_ExpData();

            temp.level = (System.Int32)levelUpTable[i]["Level"];
            
            if((levelUpTable[i]["ExpToNextLv"].ToString() != ""))
                temp.expToNextLv = (System.Int32)levelUpTable[i]["ExpToNextLv"];
            
            temp.rewardGroup = (System.Int32)levelUpTable[i]["RewardGroup"];
            temp.rewardCount = (System.Int32)levelUpTable[i]["RewardCount"];

            //expDatas.Add(temp);
            expDatas[i] = temp;
        }
    }

    public void ReadRewardTable()
    {
        List<Dictionary<string, object>> rewardTable = CSVReader.Read("Table_Player_Reward");

        for (int i = 0; i < rewardTable.Count; i++)
        {
            DT_RewardData temp;
            
            temp.id = (System.Int32)rewardTable[i]["Id"];
            temp.rewardGroup = (System.Int32)rewardTable[i]["RewardGroup"];
            temp.rewardType = (ERewardType)Enum.Parse(typeof(ERewardType), rewardTable[i]["RewardType"].ToString());
            temp.rewardParam = rewardTable[i]["RewardParam"].ToString();            
            //확률 미리 계산
            int rewardWeights = (System.Int32)rewardTable[i]["RewardWeights"];
            temp.rewardPercentage = UnityEngine.Random.Range(1, 10) * rewardWeights;

            rewardDatas.Enqueue(temp);
            //rewardDatats.Add(temp);
        }
    }
}
