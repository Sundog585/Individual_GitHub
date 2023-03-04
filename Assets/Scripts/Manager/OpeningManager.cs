using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OpeningManager : MonoBehaviour
{
    GameObject gameManual;
    GameObject gameStart;
    GameObject manualButton;
    GameObject controlManual;
    GameObject missionManual;
    GameObject storyManual;

    private void Awake()
    {
        Screen.SetResolution(1920, 1080,true);
        gameManual = transform.Find("GameManual").gameObject;
        gameStart = transform.Find("GameStart").gameObject;
        manualButton = transform.Find("ManualButton").gameObject;
        controlManual = transform.Find("Control").gameObject;
        missionManual = transform.Find("Mission").gameObject;
        storyManual = transform.Find("Story").gameObject;
    }

    private void Start()
    {
        gameStart.SetActive(true);
        manualButton.SetActive(true);
    }

    public void OnClickGameStart()
    {
        SceneManager.LoadScene("GameScene");
    }
    public void OnClickGameManual()
    {
        gameManual.SetActive(true);
        gameStart.SetActive(false);
        manualButton.SetActive(false);
    }

    public void OnClickGameManualClose()
    {
        gameManual.SetActive(false);
        gameStart.SetActive(true);
        manualButton.SetActive(true);
    }

    public void OnClickControlButton()
    {
        gameManual.SetActive(false);
        controlManual.SetActive(true); 
    }

    public void OnClickMissionButton()
    {
        gameManual.SetActive(false);
        missionManual.SetActive(true);
    }

    public void OnClickStoryButton()
    {
        gameManual.SetActive(false);
        storyManual.SetActive(true);
    }

    public void OnClickClose()
    {
        controlManual.SetActive(false);
        missionManual.SetActive(false);
        storyManual.SetActive(false);
        gameManual.SetActive(true);
    }
}
