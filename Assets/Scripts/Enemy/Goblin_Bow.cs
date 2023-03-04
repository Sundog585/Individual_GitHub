using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin_Bow : Monster_Goblin
{
    public float goblinBowAttackSpeed = 3.0f;
    
    public GameObject arrow;
    public Transform shotPosition;

    protected override void Start()
    {
        base.Start();
        maxHP = 100.0f;
        HP = maxHP;
        defencePower = 1.0f;
        dropMoney = 10;
    }

    public override void AttackUpdate()
    {
        if (attackCoolTime < 0.0f)
        {
            isAttack = true;
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(target.transform.position - transform.position), 0.7f); // 플레이어 위치 조준하기
            anim.SetTrigger("Attack");
            Instantiate(arrow, shotPosition.position, shotPosition.rotation);   // 화살 생성
            attackCoolTime = goblinBowAttackSpeed;  // 고블린 보우는 따로 공격 쿨타임 초기화
            // 에니메이션이랑 화살 나가는 속도가 싱크가 안됨. 에니메이션 바로 실행되는 방법 알아보기
        }
    }
}
