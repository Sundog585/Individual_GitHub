using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin_Group : MonoBehaviour
{
    Monster_Goblin[] goblins;

    private void Start()
    {
        goblins = GetComponentsInChildren<Monster_Goblin>();
    }

    public void GroupChase()
    {
        // 
        foreach (Monster_Goblin child in goblins)
        {
            if (child.name == transform.name)
                return; // 자신은 리턴
            child.ChangeState(MonsterState.Chase); // 몬스터들 상태 변경
        }
        transform.DetachChildren(); // 자식들 분리
        Destroy(this.gameObject,0.5f);
    }
}
