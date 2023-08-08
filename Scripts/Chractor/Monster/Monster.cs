using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
//using System.Runtime.InteropServices;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Monster : MonoBehaviour
{
    public virtual bool Patrol() { return false; }
    public virtual bool Chase() { return false; }
    public virtual bool MoveTo() { return false; }
    public virtual void GetDamage(int damage, Vector3 dir) {}
    public virtual bool Hitted() { return false; }
    public virtual void End_Hitted() {}
    public virtual bool NoneDamaged(float time) { return false; }
    public virtual bool Groggy() { return false; }
    public virtual bool Dead(){ return false; }
    public virtual void End_Dead() {}
}