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
                return; // �ڽ��� ����
            child.ChangeState(MonsterState.Chase); // ���͵� ���� ����
        }
        transform.DetachChildren(); // �ڽĵ� �и�
        Destroy(this.gameObject,0.5f);
    }
}
