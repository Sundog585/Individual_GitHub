using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin_Bow : Monster_Goblin
{
    public float goblinBowAttackSpeed = 2.0f;
    public GameObject arrow;
    public Transform shotPosition;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        maxHP = 100.0f;
        HP = maxHP;
        defencePower = 1.0f;
    }

    public override void AttackUpdate()
    {
        if (attackCoolTime < 0.0f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(target.transform.position - transform.position), 0.7f); // ���ϸ��̼��̶� ȭ�� ������ �ӵ��� ��ũ�� �ȵ�. ���ϸ��̼� �ٷ� ����Ǵ� ��� �˾ƺ���
            anim.SetTrigger("Attack");
            Instantiate(arrow, shotPosition.position, shotPosition.rotation);
            attackCoolTime = attackSpeed + goblinBowAttackSpeed;
        }
    }
}
