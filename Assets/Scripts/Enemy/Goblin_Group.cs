using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin_Group : MonoBehaviour
{
    Monster_Goblin[] goblins;

    private void Start()
    {
        //int childCount = transform.childCount;
        //for (int i = 0; i < childCount; i++)
        //{
        //}
        goblins = GetComponentsInChildren<Monster_Goblin>();
    }

    public void GroupChase()
    {
        foreach (Monster_Goblin child in goblins)
        {
            if (child.name == transform.name)
                return;
            child.ChangeState(MonsterState.Chase);
            
        }
    }
}
