using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Test_Seamless : MonoBehaviour
{
    private const int Height = 3;
    private const int Width = 3;

    AsyncOperation[,] asyncs;
    string[,] sceneNames;
    bool[,] sceneLoaded;    // true�� �ε� �Ǿ���. false�� ���� ����


    private void Start()
    {
        //for (int i = 0; i < 3; i++)
        //{
        //    for (int j = 0; j < 3; j++)
        //    {
        //        GameManager.Instance.MapManager.RequestAsyncSceneLoad(i, j);
        //    }
        //}
        //GameManager.Instance.Initialize();
    }

    private void Test_Code()
    {
        string sceneNameBase = "Seamless";
        asyncs = new AsyncOperation[Height, Width];
        sceneNames = new string[Height, Width];

        asyncs[1, 1] = SceneManager.LoadSceneAsync($"{sceneNameBase}_1_1", LoadSceneMode.Additive);
        asyncs[1, 1].completed += (AsyncOperation _) => Debug.Log($"{sceneNameBase}_1_1");
        asyncs[1, 1].priority = 5;
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                // ����(Synchronous) ��� �ε�. �ε��� �ٸ� �۾��� �� �� ����.
                //SceneManager.LoadScene($"{sceneNameBase}_{x}_{y}", LoadSceneMode.Additive);
                string temp = $"{sceneNameBase}_{x}_{y}";
                sceneNames[y, x] = temp;

                if (y == 1 && x == 1)
                {
                    continue;
                }

                // �񵿱� ��� �ε�..
                asyncs[y, x] = SceneManager.LoadSceneAsync(temp, LoadSceneMode.Additive);

                // ��������Ʈ�� ���޵Ǵ� ������ ������ �Ű�����.
                int tempX = x;
                int tempY = y;
                asyncs[y, x].completed += (AsyncOperation _) => Debug.Log($"{sceneNameBase}_{tempX}_{tempY}");
                asyncs[y, x].completed += (_) => sceneLoaded[tempY, tempX] = true;

                // allowSceneActivation = false�� ��ø���� �ϴ� ���� ����� �ϸ� �ȵ�
                //asyncs[y, x].allowSceneActivation = false;    

            }
        }

        //asyncs[0, 0].allowSceneActivation = false;    
    }

    private void Update()
    {
        //if (Keyboard.current.digit1Key.wasPressedThisFrame)
        //{
        //    GameManager.Instance.MapManager.RequestAsyncSceneLoad(1, 0);
        //}
        //if (Keyboard.current.digit2Key.wasPressedThisFrame)
        //{
        //    GameManager.Instance.MapManager.RequestAsyncSceneUnload(1, 0);
        //}
    }
}
