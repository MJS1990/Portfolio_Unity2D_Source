using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Effect : MonoBehaviour
{
    Animator effectAnim;
    RuntimeAnimatorController ac;
    AnimatorStateInfo info;

    [HideInInspector]
    public float length;
    public float currentTime;

    public bool loop;
    [HideInInspector]
    public bool bEnd;
    bool bStart;

    private void Start()
    {
        effectAnim.enabled = false;
        length = 0.0f;
        loop = false;
        bStart = false;
        bEnd = false;
    }

    private void Awake()
    {
        effectAnim = GetComponent<Animator>();
        ac = effectAnim.runtimeAnimatorController;
        info = effectAnim.GetCurrentAnimatorStateInfo(0);
    }

    private void FixedUpdate()
    {
        if(bStart)
        {
            if(!effectAnim.enabled) effectAnim.enabled = true;

            currentTime += Time.deltaTime;
            length = (currentTime / info.length);

            if (length > 3.5f)
            {
                bStart = false;
                bEnd = true;
                length = 0.0f;
                currentTime = 0.0f;
                effectAnim.enabled = false;
            }
        }
    }

    public float GetLength() { return info.length; }
    public void ResetAnim()
    {
        effectAnim.enabled = true;
        bStart = false;
        bEnd = false;
        length = 0.0f;
        currentTime = 0.0f;
    }

    public void SetAnim(Animator newAnim)
    {
        effectAnim = newAnim;
    }

    public void Play()
    {
        if(effectAnim)
        {
            bStart = true;
            effectAnim.enabled = true;
        }
    }
    public void Play(Vector3 vec)
    {
        if (effectAnim)
        {
            transform.position = vec;
            bStart = true;
            effectAnim.enabled = true;
        }
    }
}
