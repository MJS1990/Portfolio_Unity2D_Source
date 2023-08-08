using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowthManager : MonoBehaviour
{
    GameObject player;
    PlayerStatus playerStatus;

    List<GameObject> weapons;

    PlayerGrowth playerGrowth;
    WeaponGrowth weaponGrowth;

    //public struct DT_ExpData
    //{
    //    public int level;
    //    public int expToNextLv;
    //    public int rewardGroup;
    //    public int rewardCount;
    //}

    //public enum ERewardType
    //{ 
    //    Weapon = 0,
    //    Projectile,
    //    PlayerHP,
    //}

    //public struct DT_RewardData
    //{
    //    public int id;
    //    public int rewardGroup;
    //    public ERewardType rewardType;
    //    public string rewardParam;
    //    public int rewardWeights;
    //}

    //DT_ExpData[] expDatas;
    //List<DT_RewardData> rewardDatas;

    bool bLevelUp;

    private void Start()
    {
        player = GameManager.Get().GetPlayer();
        playerStatus = player.gameObject.GetComponent<PlayerStatus>();

        //expDatas = new DT_ExpData[9];
        
        bLevelUp = false;

        //TODO : 사용할지 체크할것
        playerGrowth = new PlayerGrowth();
        weaponGrowth = new WeaponGrowth();
        


    }

    private void FixedUpdate()
    {
        //if (playerStatus.GetExp() >= expDatas[playerStatus.GetLevel() - 1].expToNextLv)
        //    LevelUp();
    }

    //public void LevelUp()
    //{
    //    //Random.Range(min, max);
    //    //Range (float min, float max); //0~1사이의 난수 리턴
    //}

    //public void ReadExpTable()
    //{
    //    List<Dictionary<string, object>> levelUpTable = CSVReader.Read("Table_Player_Exp");
    //
    //    for(int i = 0; i < levelUpTable.Count; i++)
    //    {
    //        expDatas[i].level = (System.Int32)levelUpTable[i]["Level"];
    //        expDatas[i].expToNextLv = (System.Int32)levelUpTable[i]["ExpToNextLv"];
    //        expDatas[i].rewardGroup = (System.Int32)levelUpTable[i]["RewardGroup"];
    //        expDatas[i].rewardCount = (System.Int32)levelUpTable[i]["RewardCount"];
    //    }
    //}

    //public void ReadRewardTable()
    //{
    //    List<Dictionary<string, object>> rewardTable = CSVReader.Read("Table_Player_Reward");
    //
    //    for (int i = 0; i < rewardTable.Count; i++)
    //    {
    //        DT_RewardData temp;
    //
    //        temp.id = (System.Int32)rewardTable[i]["Id"];
    //        temp.rewardGroup = (System.Int32)rewardTable[i]["RewardGroup"];
    //        temp.rewardType = (ERewardType)rewardTable[i]["RewardType"];
    //        temp.rewardParam = rewardTable[i]["RewardParam"].ToString();
    //        temp.rewardWeights = (System.Int32)rewardTable[i]["RewardWeights"];
    //
    //        rewardDatas.Add(temp);
    //    }
    //}

}


class PlayerGrowth
{
    public PlayerGrowth()
    {
        status = new Status();
    }

    ~PlayerGrowth()
    {
    }

    struct Status
    {

    }

    private Status status { get; }

}

class WeaponGrowth
{
    public WeaponGrowth()
    {
        status = new Status();
    }

    ~WeaponGrowth()
    {
    }

    struct Status
    {

    }

    private Status status { get; }
}
