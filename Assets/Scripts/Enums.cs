using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterState
{
    Idle = 0,
    Chase,
    Attack,
    Dead
}

public enum MonsterType
{
    Warrior = 0,
    Axe,
    Elite,
    Bow,
    Mage,
    Boss
}

public enum WeaponType
{
    Knife = 0,
    Sword,
    Axe,
    Arrow,
    Magic,
    BossAxe
}

enum TankMode
{
    Normal = 0,
    SiegeMode
}
