using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    int missionCount = 0;

    private static GameManager instance;

    /// <summary>
    /// 심리스용
    /// </summary>
    MapManager mapManager;
    public MapManager MapManager => mapManager;

    public static GameManager Instance
    {
        get { return instance; }
    }

    GameObject player;
    public GameObject[] shell;

    public GameObject Player
    {
        get { return player; }
    }

    GameObject playerUI;
    public GameObject PlayerUI
    {
        get { return playerUI; }
    }

    public GameObject Shell
    {
        get { return shell[0]; }
    }

    public GameObject Shell_Siege
    {
        get { return shell[1]; }
    }

    GameObject store;
    public GameObject Store
    {
        get { return store; }
    }
    
    public int MissionCount
    {
        get => missionCount;
        set
        {
            if (value != missionCount)
            {
                missionCount = value;
                if(missionCount == 3)
                {
                    SceneManager.LoadScene("GameClearScene");
                }
            }
        }
    }

    private void Awake()
    {
        instance = this;
        //DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        //if (instance == null)
        //{
        //    instance = this;
        //    DontDestroyOnLoad(gameObject);
        //    SceneManager.sceneLoaded += OnSceneLoaded;
        //}
        //else
        //{
        //    if (instance != this)
        //    {
        //        Destroy(gameObject);
        //    }
        //}
    }

    private void OnSceneLoaded(Scene sceneData, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;  // 한번만 실행하기 위해
        Initialize();
    }

    public void GameOver()
    {
        SceneManager.LoadScene("GameOverScene");
    }

    public void Initialize()
    {
        //Cursor.visible = false;
        player = GameObject.FindGameObjectWithTag("Player");    //mapManager의 초기화보다 앞에 있어야 한다.
        store = GameObject.FindGameObjectWithTag("Store");
        playerUI = GameObject.FindGameObjectWithTag("PlayerUI");
        if (mapManager == null)
        {
            mapManager = GetComponent<MapManager>();
        }
        mapManager.Initialize();
        missionCount = 0;
    }
}
