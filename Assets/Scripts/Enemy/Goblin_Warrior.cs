using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin_Warrior : Monster_Goblin
{
    public GameObject weapon;
    protected override void Awake()
    {
        base.Awake();
        weapon = GetComponentInChildren<Weapon_Knife>().gameObject;
    }

    protected override void Start()
    {
        base.Start();
        maxHP = 100.0f;
        HP = maxHP;
        defencePower = 10.0f;
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
    }
}
