using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameClearUI : MonoBehaviour
{
    public void OnClickRestart()
    {
        SceneManager.LoadScene("OpeningScene");
    }
}
