using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpScene : MonoBehaviour
{
    GameObject growthCanvas;

    [SerializeField]
    RectTransform listPos;
    float onListPos = -280.0f;
    float offListPos = 240.0f;

    [SerializeField]
    RectTransform buttonPos;
    float onButtonPos = -630.0f;
    float offButtonPos = -860.0f;

    float tempTime = 0.0f;
    public bool bAciveUI = false;

    [SerializeField]
    Image[] rewardPanelImages;
    [SerializeField]
    List<Image> rewardImages;

    List<TextMesh> rewardText;
    
    List<int> rewardId;

    private void Awake()
    {
        growthCanvas = transform.GetChild(0).gameObject;

        rewardImages = new List<Image>();
        rewardPanelImages = new Image[3];


        rewardText = new List<TextMesh>();

        rewardId = new List<int>();
    }

    private void Update()
    {
        if(bAciveUI)
        {
            //패널 이동
            tempTime += 0.005f;
            if (listPos.anchoredPosition.y != onListPos)
            {
                listPos.anchoredPosition = new Vector2(listPos.anchoredPosition.x, Mathf.Lerp(listPos.anchoredPosition.y, onListPos, tempTime));
            }

            if (buttonPos.anchoredPosition.y != onButtonPos)
            {
               buttonPos.anchoredPosition = new Vector2(buttonPos.anchoredPosition.x, Mathf.Lerp(buttonPos.anchoredPosition.y, onButtonPos, tempTime));
            }
        }
    }


    public void OnGrowthScene(List<int> id)
    {
        rewardId = id;

        Time.timeScale = 0.0f;
        growthCanvas.gameObject.SetActive(true);
        bAciveUI = true;
    }

    public void OffGrowthScene()
    {
        growthCanvas.gameObject.SetActive(false);
        bAciveUI = false;

        listPos.anchoredPosition = new Vector2(listPos.anchoredPosition.x, offListPos);
        buttonPos.anchoredPosition = new Vector2(buttonPos.anchoredPosition.x, offButtonPos);

        rewardImages.Clear();
        //rewardPanelImages.Clear();
        rewardText.Clear();
        rewardId.Clear();

        Time.timeScale = 1.0f;
        tempTime = 0.0f;
    }
}
