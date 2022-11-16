using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Test_Seamless : MonoBehaviour
{
    private const int Height = 3;
    private const int Width = 3;

    AsyncOperation[,] asyncs;

    private void Start()
    {
        string sceneNameBase = "TestSeamless";
        asyncs = new AsyncOperation[Height, Width];

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                // 동기(Synchronous) 방식 로딩. 로딩중 다른 작업은 할 수 없음.
                //SceneManager.LoadScene($"{sceneNameBase}_{x}_{y}", LoadSceneMode.Additive);

                // 비동기 방식 로딩..
                asyncs[y,x] = SceneManager.LoadSceneAsync($"{sceneNameBase}_{x}_{y}", LoadSceneMode.Additive);
                
                // 델리게이트에 전달되는 변수는 힙으로 옮겨진다.
                int tempX = x;
                int tempY = y;
                asyncs[y, x].completed += (AsyncOperation _) => Debug.Log($"{sceneNameBase}_{tempX}_{tempY}");

                // allowSceneActivation = false를 중첩으로 하는 것은 절대로 하면 안됨
                //asyncs[y, x].allowSceneActivation = false;    

            }
        }

        asyncs[0, 0].allowSceneActivation = false;    
    }
}
