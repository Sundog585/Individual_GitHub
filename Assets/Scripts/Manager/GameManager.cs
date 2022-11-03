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
    public GameObject Shell
    {
        get { return shell[0]; }
    }

    public GameObject Shell_Siege
    {
        get { return shell[1]; }
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
    }
}
