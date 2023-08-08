using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;

    [SerializeField]
    private GameObject player;

    PlayerAction playerAction;
    public Image fadePanel;
    MonsterSpawner spawner;

    private void Start()
    {
        instance = this;

        StartCoroutine(FadeOut());

        spawner = GetComponent<MonsterSpawner>();

        //Test Spawn
        spawner.ReadSpawnDatas(1);
    }

    public static GameManager Get()
    {
        if (!instance)
            return null;

        return instance;
    }

    public GameObject GetPlayer()
    {
        if (player == null)
            return null;

        return player;
    }

    private void FixedUpdate()
    {
        //spawner.UpdateSpawn();
        //if(spawner.spawnId.Count > 0)
        //{
        //    print("==========================");
        //    while(spawner.spawnId.Count > 0)
        //    {
        //        if (spawner.spawnId.Count == 0) break;
        //
        //        print("spawnId : " + spawner.spawnId.Dequeue());
        //    }
        //    print("==========================");
        //}
    }

    public void MoveNextScene()
    {
        StartCoroutine(CMoveNextScene());
    }

    public void MoveGameOverScene()
    {
        StartCoroutine(CMoveGameOverScene());
    }


    IEnumerator CMoveNextScene()
    {
        int index = SceneManager.GetActiveScene().buildIndex + 1;

        spawner.ReadSpawnDatas(index);
        spawner.ResetTime();

        StartCoroutine(FadeIn());   
        yield return new WaitForSeconds(1.25f);
    
        SceneManager.LoadScene(index);
    }

    IEnumerator CMoveGameOverScene()
    {
        yield return new WaitForSeconds(1.25f);

        StartCoroutine(FadeIn());
        yield return new WaitForSeconds(1.25f);

        SceneManager.LoadScene("GameOver");
    }

    IEnumerator FadeIn()
    {
        while(fadePanel.color.a < 1.0f)
        {
            fadePanel.color += new Color(0.0f, 0.0f, 0.0f, 0.01f);
            yield return new WaitForSeconds(0.01f);
        }
    }

    IEnumerator FadeOut()
    {
        while (fadePanel.color.a > 0.0f)
        {
           fadePanel.color -= new Color(0.0f, 0.0f, 0.0f, 0.01f);
            yield return new WaitForSeconds(0.01f);
        }
    }
}