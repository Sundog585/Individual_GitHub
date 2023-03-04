using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin_Boss : Monster_Goblin
{
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
        attackCoolTime = attackSpeed;
        return;
    }

    void AttackPatternB()
    {
        anim.SetInteger("AttackType", 1);
        attackCoolTime = attackSpeed;
        return;
    }

    void AttackPatternC()
    {
        anim.SetInteger("AttackType", 2);
        attackCoolTime = attackSpeed;
        return;
    }

    protected override void Die()
    {
        if (!isDead)
        {
            ChangeState(MonsterState.Dead);
            
        }
        GameManager.Instance.MissionCount++;    // 보스 사망시 미션 카운트 올려주기
    }

}
