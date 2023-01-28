using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin_Bow : Monster_Goblin
{
    public GameObject weapon;

    protected override void Awake()
    {
        base.Awake();
        weapon = GetComponentInChildren<Weapon_Bow>().gameObject;
    }

    protected override void Start()
    {
        base.Start();
        maxHP = 100.0f;
        HP = maxHP;
        defencePower = 1.0f;
    }
}
