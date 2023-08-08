using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private static CameraShake instance = null;

    public float YAxis_shakeLength = 1.0f;
    public float XAxis_shakeLength = 1.0f;

    public float shakeSpeed = 1.0f;
    float amplitude = 0.0f;    
    public float shakeTime = 1.0f;

    bool bShake = false;

    //Round Shake
    public float halfRadius = 1.0f;
    public float rotateSpeed = 30.0f;
    
    float angle;

    //Zoom
    float originZ;
    float zoomSpeed = 200.0f;

    void Start()
    {
        instance = this;
        angle = 10.0f;

        originZ = transform.position.z;
    }

    public static CameraShake Get()
    {
        if (!instance)
            return null;

        return instance;
    }

    void FixedUpdate()
    {
        ShakeRound();

        if (bShake)
            ZoomIn();
        else if (!bShake)
            ZoomOut();
    }

    public bool GetbShake() { return bShake; }
    public void SetbShake(bool val) { bShake = val; }

    //좌우상하 쉐이크
    public void Shake()
    {
        amplitude += Time.deltaTime * shakeSpeed;
        
        gameObject.transform.position = new Vector3(gameObject.transform.position.x + (XAxis_shakeLength * Mathf.Sin((amplitude))), gameObject.transform.position.y + (YAxis_shakeLength * Mathf.Sin((amplitude))), gameObject.transform.position.z);

        if (shakeTime >= amplitude) //종료조건
        {
            bShake = false;
            amplitude = 0.0f;
            gameObject.transform.position = new Vector3(transform.parent.gameObject.transform.position.x, transform.parent.gameObject.transform.position.y, gameObject.transform.position.z);
        }
    }

    public void ZoomIn()
    {
         transform.position += new Vector3(0.0f, 0.0f, zoomSpeed * Time.deltaTime);
    }
    public void ZoomOut()
    {
        if(transform.position.z > originZ)
            transform.position -= new Vector3(0.0f, 0.0f, zoomSpeed * Time.deltaTime);
        //transform.position = new Vector3(transform.position.x, transform.position.y, originZ);
    }

    //회전 쉐이크
    public void ShakeRound()
    {
        if (!bShake) return;

        if (angle <= 0 || angle >= 180)
        {
            rotateSpeed *= -1;
        }

        angle += Time.deltaTime * rotateSpeed;
        transform.position = new Vector3(transform.position.x + (Mathf.Sin(angle) * halfRadius), transform.position.y + (Mathf.Cos(angle) * halfRadius), gameObject.transform.position.z);
        
        if (angle >= 18) //종료조건
        {
            bShake = false;
            angle = 10.0f;
            gameObject.transform.position = new Vector3(transform.parent.gameObject.transform.position.x, transform.parent.gameObject.transform.position.y, gameObject.transform.position.z);
        }
    }
}
