using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Monster_Goblin : MonoBehaviour, IHit
{
    // enum들
    public MonsterState state = MonsterState.Chase;

    // 컴포넌트들 Nav, anim, collider
    protected Animator anim;
    protected Collider col;
    protected Rigidbody rigid;
    protected NavMeshAgent nav;


    // HP용
    public bool takeDamage = false;
    protected bool isDead = false;
    public bool isAttack = false;
    public float maxHP = 100.0f;
    public float hp;
    public Vector3 hitPoint = Vector3.zero; // 충돌 위치
    protected int dropMoney = 5;
    
    public float HP
    {
        get => hp;
        set
        {
            if(value != hp)
            {
                hp = value;
                if (hp <= 0)
                {
                    hp = 0;
                    Dead();
                }
                hp = Mathf.Min(hp, maxHP);
                onHealthChange?.Invoke(hp / maxHP);
            }
        }
    }

    public float MaxHP { get => maxHP; }
    public Action<float> onHealthChange { get ; set ; }
    public Action onDead { get ; set; }

    // 전투용
    public float attackPower = 10.0f;
    public float defencePower = 5.0f;
    public float attackSpeed = 1.0f;
    public float attackCoolTime = 0;

    // 각종 클래스들
    protected GameObject player;
    public Transform target;    // Nav의 목표
    protected PlayerTank playerTank;
    protected Goblin_Group group;

    public float AttackPower { get => attackPower; }
    public float DefencePower { get => defencePower; }

    // 미니맵용

    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        nav = GetComponent<NavMeshAgent>();
    }

    protected virtual void Start()
    {
        player = GameManager.Instance.Player;
        target = player.transform;
        playerTank = player.GetComponent<PlayerTank>();
        state = MonsterState.Idle;
        if(transform.parent != null)
        {
            group = GetComponentInParent<Goblin_Group>();
        }
    }

    public void Update()
    {
        attackCoolTime -= Time.deltaTime;
        if (!isDead)
        {
            switch (state)
            {
                case MonsterState.Idle:
                    IdleUpdate();
                    break;
                case MonsterState.Chase:
                    ChaseUpdate();
                    break;
                case MonsterState.Attack:
                    AttackUpdate();
                    break;
                case MonsterState.Dead:
                    Dead();
                    break;
                default:
                    break;
            }
        }
        // 미니맵
        //quadPosition = new Vector3(quad.position.x, transform.position.y, quad.position.z); //미니맵고정용
        //quad.transform.LookAt(quadPosition); //미니맵고정용
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Shell"))
        {
            hitPoint = collision.contacts[0].point;
            hitPoint.y = -1.0f;
            if (!isAttack)
            {
                ChangeState(MonsterState.Chase);
            }
            return;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ChangeState(MonsterState.Attack);
            return;
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!isDead)
            {
                ChangeState(MonsterState.Chase);
            }
            return;
        }
    }

    public void ChangeState(MonsterState newState)
    {
        if (isDead)
        {
            return;
        }

        switch (state)
        {
            case MonsterState.Idle:
                break;
            case MonsterState.Chase:
                nav.isStopped = true;
                break;
            case MonsterState.Attack:
                nav.isStopped = true;
                break;
            case MonsterState.Dead:
                nav.isStopped = true;
                isDead = false;
                break;
            default:
                break;
        }
        switch (newState)
        {
            case MonsterState.Idle:
                break;
            case MonsterState.Chase:
                nav.isStopped = false;
                break;
            case MonsterState.Attack:
                nav.isStopped = true;
                break;
            case MonsterState.Dead:
                isDead = true;
                break;
            default:
                break;
        }
        state = newState;   // 새로운 상태로 변경
        anim.SetInteger("MonsterState", (int)state);
    }


    public virtual void AttackUpdate()
    {

        if (attackCoolTime < 0.0f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(target.transform.position - transform.position), 0.1f); // 플레이어 바라보기
            anim.SetTrigger("Attack");
            isAttack = true;
            attackCoolTime = attackSpeed;   // 쿨타임 초기화 해주기
        }
    }

    public void ChaseUpdate()
    {
        nav.SetDestination(target.position);
        return;
    }

    public void IdleUpdate()
    {
        
        // 피해를 입을 시에 쫒아가기
        return;
    }



    public void Dead()
    {
        if (!isDead)
        {
            gameObject.layer = LayerMask.NameToLayer("Default");    // 죽었을 때 락온이 다시 되지 않게 하기 위해 설정
            anim.SetBool("Dead", true);
            anim.SetTrigger("Die");
            isDead = true;
            nav.isStopped = true;
            HP = 0;
            onDead?.Invoke();           // 죽음 체크
            DestroyProcess();
        }
    }

    void DestroyProcess()
    {
        col.enabled = false;
        rigid.drag = 10.0f;
        rigid.angularDrag = 3.0f;
        Destroy(this.gameObject, 5.0f);
    }

    public virtual void TakeDamage(float damage)
    {
        float finalDamage = damage - defencePower;
        if (finalDamage < 1.0f)
        {
            finalDamage = 1.0f;
        }
        HP -= finalDamage;

        if (HP > 0.0f)
        {
            anim.SetTrigger("TakeDamage");
            if (!isAttack)
            {
                ChangeState(MonsterState.Chase);
                if (transform.parent != null)
                {
                    group.GroupChase();
                    transform.parent = null;
                }
            }
        }
        else
        {
            playerTank.Money += dropMoney;
            Die();
        }
    }

    protected virtual void Die()
    {
        if (!isDead)
        {
            ChangeState(MonsterState.Dead);          
        }
    }
}
