using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin_Warrior : Monster_Goblin
{
    protected override void Start()
    {
        base.Start();
        maxHP = 100.0f;
        HP = maxHP;
        defencePower = 10.0f;
    }
}
