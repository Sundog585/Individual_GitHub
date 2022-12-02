using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    const string sceneNameBase = "Seamless";

    private const int Height = 5;
    private const int Width = 5;

    string[,] sceneNames;

    PlayerTank player;

    enum SceneLoadState : byte
    {
        Unload = 0,
        PendingUnload,
        PendingLoad,
        Loaded
    }
    SceneLoadState[,] sceneLoadState;    // �� ���� �ε� ����

    

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

        player = GameManager.Instance.Player.GetComponent<PlayerTank>();
        player.onMapChange += RefreshScenes;
        RequestAsyncSceneLoad(player.CurrentMap.x, player.CurrentMap.z);
        RefreshScenes(player.CurrentMap);

    }

    void RefreshScenes(Vector3Int current)
    {
        // �̿��� ���ؼ� �� �ε� ��û
        for (int y = -1; y < 2; y++)
        {
            for (int x = -1; x < 2; x++)
            {
                if (x != 0 || y != 0)   // �÷��̾ �ִ� ���� �ߺ��ε� ����
                {
                    RequestAsyncSceneLoad(player.CurrentMap.x + x, player.CurrentMap.z + y);
                }
            }
        }

        // ���� �̿��� �ƴ� ���� �� �ε� ���� ��û
        // �⺻���� ������ �ε��� ������ ���� ��ġ�� ���� �÷��̾� ��ġ���� ��ĭ�� ������ ���� �� �ۿ� ����.
        // ���� �÷��̾� ��ġ�� -2 ~ +2 ������ ã�Ƽ� ���� ��û�� �Ѵ�.
        for (int y = -2; y < 3; y++)
        {
            for (int x = -2; x < 3; x++)
            {
                if (x == 2 || x == -2 || y == 2 || y == -2)
                {
                    RequestAsyncSceneUnload(player.CurrentMap.x + x, player.CurrentMap.z + y);
                }
            }
        }
    }
    /// <summary>
    /// �� �ε� ��û
    /// </summary>
    /// <param name="x">�ε��� ���� x ��ġ</param>
    /// <param name="y">�ε��� ���� y ��ġ</param>
    void RequestAsyncSceneLoad(int x, int y)
    {
        if (IsValidePosition(x, y)) // x,y�� ������ ������ Ȯ��
        {
            if (sceneLoadState[y, x] == SceneLoadState.Unload)  // ��ε� ������ ���� �ε� ��û
            {
                AsyncOperation async = SceneManager.LoadSceneAsync(sceneNames[y, x], LoadSceneMode.Additive);
                async.completed += (_) => sceneLoadState[y, x] = SceneLoadState.Loaded; // �ε尡 ������ Loaded�� ���� ����
                sceneLoadState[y, x] = SceneLoadState.PendingLoad;  // �ε� ���������ϱ� pending ���·� ����
            }
        }
    }

    /// <summary>
    /// �� �ε� ���� ��û
    /// </summary>
    /// <param name="x">�ε� ������ ���� x ��ġ</param>
    /// <param name="y">�ε� ������ ���� y ��ġ</param>
    void RequestAsyncSceneUnload(int x, int y)
    {
        if (IsValidePosition(x, y)) // x,y�� ������ ������ Ȯ��
        {
            if (sceneLoadState[y, x] == SceneLoadState.Loaded)  // Loaded ������ ���� �ε� ��û
            {
                AsyncOperation async = SceneManager.UnloadSceneAsync(sceneNames[y, x]);
                async.completed += (_) => sceneLoadState[y, x] = SceneLoadState.Unload; // �ε������� ������ Unload�� ���� ����
                sceneLoadState[y, x] = SceneLoadState.PendingUnload;    // �ε尡 �����Ǳ� ���������ϱ� pending ���·� ����
            }
        }
    }
    bool IsValidePosition(int x, int y)
    {
        return (x > -1 && x < Width && y > -1 && y < Height);
    }
}