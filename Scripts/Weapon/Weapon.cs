using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public struct DT_WeaponData
    {
        public int id;
        public string groupKey;
        public string type;
        public int level;
        public bool isMaxLevel;
        public int attackDamage;
        public string[] projectileList;
    }
    List<DT_WeaponData> weaponData;

    protected enum EWeaponType
    {
        None = -1,
        Gun,
        Rotate,
        Bomb,
        Trap,
    }
    
    public GameObject attachParent;

    protected bool bAttach; //°ø¿¡ ¹«Âø
    
    private void Awake()
    {
        bAttach = false;
        ReadWeaponTable();
    }

    public void ReadWeaponTable()
    {
        List<Dictionary<string, object>> weaponTable = CSVReader.Read("Table_Weapon");

        weaponData = new List<DT_WeaponData>();

        for (int i = 0; i < weaponTable.Count; i++)
        {
            DT_WeaponData data;

            data.id = (System.Int32)weaponTable[i]["Id"];
            data.groupKey = weaponTable[i]["GroupKey"].ToString();
            data.type = weaponTable[i]["Type"].ToString();
            data.level = (System.Int32)weaponTable[i]["Level"];
            //weaponData.isMaxLevel = weaponTable[0]["IsMaxLevel"];
            data.isMaxLevel = false;
            data.attackDamage = (System.Int32)weaponTable[i]["Atk"];
            data.projectileList = weaponTable[i]["ProjectileList"].ToString().Split(", ");

            weaponData.Add(data);
        }
    }

    //private void OnTriggerEnter2D(Collider2D collider)
    //{
    //    if (collider.gameObject.tag == "Attachment")
    //    {
    //        bAttach = true;
    //
    //        if (attachParent)
    //            transform.parent = attachParent.transform;
    //
    //        //collider.isTrigger = true;
    //
    //        //if(rigid)
    //        //{
    //        //    print("SetGravity");
    //        //    rigid.gravityScale = 0.0f;
    //        //    rigid.mass = 0.0f;
    //        //}
    //    }
    //    //else if (collision.gameObject.tag == "Player") 
    //    //{
    //    //    bGet = true;
    //    //    transform.parent = attachParent.transform;
    //    //}
    //}

    //private void OnTriggerStay2D(Collider2D collider)
    //{
    //    if (collider.gameObject.tag == "Terrain")
    //    {
    //        float y = collider.gameObject.transform.position.y;
    //        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    //    }
    //}
}