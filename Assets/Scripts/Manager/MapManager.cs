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
    SceneLoadState[,] sceneLoadState;    // 각 씬의 로딩 상태

    

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
        // 이웃을 구해서 맵 로딩 요청
        for (int y = -1; y < 2; y++)
        {
            for (int x = -1; x < 2; x++)
            {
                if (x != 0 || y != 0)   // 플레이어가 있는 맵의 중복로딩 방지
                {
                    RequestAsyncSceneLoad(player.CurrentMap.x + x, player.CurrentMap.z + y);
                }
            }
        }

        // 이제 이웃이 아닌 곳은 맵 로딩 해제 요청
        // 기본적인 컨셉은 로딩을 해제할 맵의 위치는 현재 플레이어 위치에서 두칸씩 떨어진 맵일 수 밖에 없다.
        // 따라서 플레이어 위치의 -2 ~ +2 범위만 찾아서 해제 요청을 한다.
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
    /// 씬 로딩 요청
    /// </summary>
    /// <param name="x">로딩할 씬의 x 위치</param>
    /// <param name="y">로딩할 씬의 y 위치</param>
    void RequestAsyncSceneLoad(int x, int y)
    {
        if (IsValidePosition(x, y)) // x,y가 적절한 값인지 확인
        {
            if (sceneLoadState[y, x] == SceneLoadState.Unload)  // 언로드 상태일 때만 로딩 요청
            {
                AsyncOperation async = SceneManager.LoadSceneAsync(sceneNames[y, x], LoadSceneMode.Additive);
                async.completed += (_) => sceneLoadState[y, x] = SceneLoadState.Loaded; // 로드가 끝나면 Loaded로 상태 변경
                sceneLoadState[y, x] = SceneLoadState.PendingLoad;  // 로드 시작했으니까 pending 상태로 변경
            }
        }
    }

    /// <summary>
    /// 씬 로딩 해제 요청
    /// </summary>
    /// <param name="x">로딩 해제할 씬의 x 위치</param>
    /// <param name="y">로딩 해제할 씬의 y 위치</param>
    void RequestAsyncSceneUnload(int x, int y)
    {
        if (IsValidePosition(x, y)) // x,y가 적절한 값인지 확인
        {
            if (sceneLoadState[y, x] == SceneLoadState.Loaded)  // Loaded 상태일 때만 로딩 요청
            {
                AsyncOperation async = SceneManager.UnloadSceneAsync(sceneNames[y, x]);
                async.completed += (_) => sceneLoadState[y, x] = SceneLoadState.Unload; // 로딩해제가 끝나면 Unload로 상태 변경
                sceneLoadState[y, x] = SceneLoadState.PendingUnload;    // 로드가 해제되기 시작했으니까 pending 상태로 변경
            }
        }
    }
    bool IsValidePosition(int x, int y)
    {
        return (x > -1 && x < Width && y > -1 && y < Height);
    }
}