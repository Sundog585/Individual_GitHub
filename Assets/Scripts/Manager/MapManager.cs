using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    const string sceneNameBase = "Seamless";

    private const int Height = 3;
    private const int Width = 3;

    string[,] sceneNames;

    enum SceneLoadState : byte
    {
        Unload = 0,
        PendingLoad,
        PendingUnload,
        Loaded
    }
    SceneLoadState[,] sceneLoadState;    // true�� �ε� �Ǿ���. false�� ���� ����

    public void Initialize()
    {
        sceneNames = new string[Height, Width];
        sceneLoadState = new SceneLoadState[Height, Width];

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                string temp = $"{sceneNameBase}_{x}_{y}";
                sceneNames[y, x] = temp;
                sceneLoadState[y, x] = SceneLoadState.Unload;
            }
        }
    }

    public void RequestAsyncSceneLoad(int x, int y)
    {
        if (sceneLoadState[y, x] == SceneLoadState.Unload)
        {
            Debug.Log("�� �ε� ��û �Ϸ�");
            AsyncOperation async = SceneManager.LoadSceneAsync(sceneNames[y, x], LoadSceneMode.Additive);
            async.completed += (_) => sceneLoadState[y, x] = SceneLoadState.Loaded;
            sceneLoadState[y, x] = SceneLoadState.PendingLoad;
        }
        else
        {
            Debug.Log("�� �ε� ��û �Ұ�");
        }
    }

    public void RequestAsyncSceneUnload(int x, int y)
    {
        if (sceneLoadState[y, x] == SceneLoadState.Loaded)
        {
            Debug.Log("�� ��ε� ��û �Ϸ�");
            AsyncOperation async = SceneManager.UnloadSceneAsync(sceneNames[y, x]);
            async.completed += (_) => sceneLoadState[y, x] = SceneLoadState.Unload;
            sceneLoadState[y, x] = SceneLoadState.PendingUnload;
        }
        else
        {
            Debug.Log("�� ��ε� ��û �Ұ�");
        }
    }
}