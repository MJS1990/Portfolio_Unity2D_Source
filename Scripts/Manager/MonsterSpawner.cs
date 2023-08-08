//using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField]
    Monster spawnMonster;

    MonsterStatus monsterStatus;

    GameObject monster;
    Status status;
    float time;
    
    List<DT_SpawnData> spawnDatas;
    public Queue<int> spawnId;// { get; }

    List<int> currentRepeat;
    public struct DT_SpawnData
    {
        public int id;
        public int stage;
        public int monster;
        public UnityEngine.Vector2 spawnPosition;
        public float spawnDelayTime;
        public float spawnPeriod;
        public int spawnRepeat;
        public int spawnCount;
        public int currentRepeat;
        
        public void AddRepeat() { this.currentRepeat++; }
    }

    MonsterSpawner()
    {
        spawnDatas = new List<DT_SpawnData>();
        spawnId = new Queue<int>();

        currentRepeat = new List<int>();

        time = 0.0f;

    }

    void Start()
    {
    }

    public void ResetTime()
    {
        time = 0.0f;
    }

    private IEnumerator CoSpawn(DT_SpawnData val)
    {
        if (spawnMonster != null)
        {
            for (int i = 0; i < val.spawnRepeat; i++) //SpawnCount
            {
                //TODO : 인스턴스화 전에 몬스터 ID값 설정
                monsterStatus = spawnMonster.gameObject.GetComponent<MonsterStatus>();
                monsterStatus.SetID(val.monster);

                monster = Instantiate(spawnMonster.gameObject, val.spawnPosition, UnityEngine.Quaternion.identity);
                //status = monster.GetCompoenent<MonsterStatus>();
                monsterStatus.ReadStatus(val.id);
                yield return new WaitForSeconds(val.spawnPeriod);
            }
        }
    }

    public void UpdateSpawn()
    {
        if (spawnDatas.Count <= 0) return;

        time += Time.deltaTime;

        for(int i = 0; i < spawnDatas.Count; i++)
        {
            if (time >= spawnDatas[i].spawnDelayTime && currentRepeat[i] < spawnDatas[i].spawnCount)
            {
                StartCoroutine(CoSpawn(spawnDatas[i]));
                currentRepeat[i] += 1;
            }
        }
    }
    public void ReadSpawnDatas(int stageIndex)
    {
        List<Dictionary<string, object>> Table = CSVReader.Read("Table_Spawn");

        if (spawnDatas != null)
            spawnDatas.Clear();

        if (spawnId.Count > 0)
            spawnId.Clear();

        
        for(int i = 0; i < Table.Count; i++)
        {
            if ((System.Int32)Table[i]["Stage"] > stageIndex) return;

            DT_SpawnData value;// = new DT_SpawnValues();

            value.id = (System.Int32)Table[i]["Id"];
            value.stage = (System.Int32)Table[i]["Stage"];
            value.monster = (System.Int32)Table[i]["Monster"];
            value.spawnPosition.x = float.Parse(Table[i]["PosX"].ToString());
            value.spawnPosition.y = float.Parse(Table[i]["PosY"].ToString());
            value.spawnDelayTime = float.Parse(Table[i]["SpawnDelayTime"].ToString());
            value.spawnPeriod = float.Parse(Table[i]["SpawnPeriod"].ToString());
            value.spawnRepeat = (System.Int32)Table[i]["SpawnRepeat"];
            value.spawnCount = (System.Int32)Table[i]["SpawnCount"];
            value.currentRepeat = 0;

            spawnDatas.Add(value);            
            currentRepeat.Add(0);
        }
    }
}
