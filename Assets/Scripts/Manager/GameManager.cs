using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

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

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            //DontDestroyOnLoad(gameObject);
            //SceneManager.sceneLoaded += OnSceneLoaded;
            Initialize();

        }
        else
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
    private void Initialize()
    {
        Cursor.visible = false;
        player = GameObject.FindGameObjectWithTag("Player");
        store = GameObject.FindGameObjectWithTag("Store");
        playerUI = player.transform.GetChild(1).gameObject;
    }
}
