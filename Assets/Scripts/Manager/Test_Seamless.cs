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
                // ����(Synchronous) ��� �ε�. �ε��� �ٸ� �۾��� �� �� ����.
                //SceneManager.LoadScene($"{sceneNameBase}_{x}_{y}", LoadSceneMode.Additive);

                // �񵿱� ��� �ε�..
                asyncs[y,x] = SceneManager.LoadSceneAsync($"{sceneNameBase}_{x}_{y}", LoadSceneMode.Additive);
                
                // ��������Ʈ�� ���޵Ǵ� ������ ������ �Ű�����.
                int tempX = x;
                int tempY = y;
                asyncs[y, x].completed += (AsyncOperation _) => Debug.Log($"{sceneNameBase}_{tempX}_{tempY}");

                // allowSceneActivation = false�� ��ø���� �ϴ� ���� ����� �ϸ� �ȵ�
                //asyncs[y, x].allowSceneActivation = false;    

            }
        }

        asyncs[0, 0].allowSceneActivation = false;    
    }
}
