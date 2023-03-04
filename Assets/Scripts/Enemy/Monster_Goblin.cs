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
    // enum��
    public MonsterState state = MonsterState.Chase;

    // ������Ʈ�� Nav, anim, collider
    protected Animator anim;
    protected Collider col;
    protected Rigidbody rigid;
    protected NavMeshAgent nav;


    // HP��
    public bool takeDamage = false;
    protected bool isDead = false;
    public bool isAttack = false;
    public float maxHP = 100.0f;
    public float hp;
    public Vector3 hitPoint = Vector3.zero; // �浹 ��ġ
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

    // ������
    public float attackPower = 10.0f;
    public float defencePower = 5.0f;
    public float attackSpeed = 1.0f;
    public float attackCoolTime = 0;

    // ���� Ŭ������
    protected GameObject player;
    public Transform target;    // Nav�� ��ǥ
    protected PlayerTank playerTank;
    protected Goblin_Group group;

    public float AttackPower { get => attackPower; }
    public float DefencePower { get => defencePower; }

    // �̴ϸʿ�

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
        // �̴ϸ�
        //quadPosition = new Vector3(quad.position.x, transform.position.y, quad.position.z); //�̴ϸʰ�����
        //quad.transform.LookAt(quadPosition); //�̴ϸʰ�����
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
        state = newState;   // ���ο� ���·� ����
        anim.SetInteger("MonsterState", (int)state);
    }


    public virtual void AttackUpdate()
    {

        if (attackCoolTime < 0.0f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(target.transform.position - transform.position), 0.1f); // �÷��̾� �ٶ󺸱�
            anim.SetTrigger("Attack");
            isAttack = true;
            attackCoolTime = attackSpeed;   // ��Ÿ�� �ʱ�ȭ ���ֱ�
        }
    }

    public void ChaseUpdate()
    {
        nav.SetDestination(target.position);
        return;
    }

    public void IdleUpdate()
    {
        
        // ���ظ� ���� �ÿ� �i�ư���
        return;
    }



    public void Dead()
    {
        if (!isDead)
        {
            gameObject.layer = LayerMask.NameToLayer("Default");    // �׾��� �� ������ �ٽ� ���� �ʰ� �ϱ� ���� ����
            anim.SetBool("Dead", true);
            anim.SetTrigger("Die");
            isDead = true;
            nav.isStopped = true;
            HP = 0;
            onDead?.Invoke();           // ���� üũ
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
