using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin_Boss : Monster_Goblin
{
    public GameObject weapon;

    protected override void Awake()
    {
        base.Awake();
        weapon = GetComponentInChildren<Weapon_Axe>().gameObject;
    }

    protected override void Start()
    {
        base.Start();
        maxHP = 100.0f;
        HP = maxHP;
        defencePower = 10.0f;
    }
    
    public override void AttackUpdate()
    {
        if (state == MonsterState.Attack)
        {
            anim.SetTrigger("Attack");
            int randomAttack = Random.Range(0, 10);
            //Debug.Log($"{randomAttack}");
            switch (randomAttack)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                    AttackPatternA();   // 일반공격 50%
                    break;
                case 5:
                case 6:
                    AttackPatternB();   // 4연격 20%
                    break;
                case 7:
                case 8:
                case 9:
                    AttackPatternC();   // 회전공격(2연격) 30%
                    break;
            }
            isAttack = true;
        }
    }

    void AttackPatternA()
    {
        anim.SetInteger("AttackType", 0);
        //Attack(attackTarget);
        attackCoolTime = attackSpeed;
        return;
    }

    void AttackPatternB()
    {
        anim.SetInteger("AttackType", 1);
        //Attack(attackTarget);
        attackCoolTime = attackSpeed;
        return;
    }

    void AttackPatternC()
    {
        anim.SetInteger("AttackType", 2);
        //Attack(attackTarget);
        attackCoolTime = attackSpeed;
        return;
    }

    protected override void Die()
    {
        if (!isDead)
        {
            ChangeState(MonsterState.Dead);
            
        }
        GameManager.Instance.MissionCount++;
    }

}
