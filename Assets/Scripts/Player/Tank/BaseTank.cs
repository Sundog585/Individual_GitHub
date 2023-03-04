using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTank : MonoBehaviour, IHit
{
    public GameObject[] shellPrefabs;

    protected bool isDead = false;      // 사망 여부

    protected Transform turret;         // 포탑
    protected Transform firePosition;   // 발사 위치
    protected FireData[] fireDatas;     // 발사 쿨타임 관리

    public float hp;
    public float maxHP;
    public float defencePower;
    public Vector3 hitPoint = Vector3.zero; // 충돌 위치

    public float HP
    {
        get => hp;
        set
        {
            if (value != hp)
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

    public Action<float> onHealthChange { get; set; }
    public Action onDead { get; set; }

    // 컴포넌트
    protected Collider tankCollider;
    protected Rigidbody rigid;

    protected virtual void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        tankCollider = GetComponent<Collider>();

        turret = transform.Find("TankRenderers").Find("TankTurret");
        firePosition = turret.GetChild(0);

        fireDatas = new FireData[shellPrefabs.Length];
        for (int i = 0; i < shellPrefabs.Length; i++) 
        {
            Shell shell = shellPrefabs[i].GetComponent<Shell>();
            fireDatas[i] = new FireData(shell.Data);
        }
    }

    protected virtual void Start()
    {
        hp = maxHP;
    }

    protected virtual void Update()
    {
        if (!isDead)    // 살아있으면
        {
            foreach (var data in fireDatas)
            {
                data.CurrentCoolTime -= Time.deltaTime; // 포탄 쿨타임 처리
            }
        }
    }
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (isDead && collision.gameObject.CompareTag("Ground"))
    //    {
    //        DestroyProcess();
    //    }
    //}

    public virtual void TakeDamage(float damage)
    {
        float finalDamage = damage - defencePower;
        if (finalDamage < 1.0f)
        {
            finalDamage = 1.0f;
        }
        HP -= finalDamage; 
    }

    public void Dead()
    {
        isDead = true;
        HP = 0;
        GameManager.Instance.GameOver();
        DestroyProcess(); 
    }

    void DestroyProcess()
    {
        tankCollider.enabled = false;
        rigid.drag = 10.0f;
        rigid.angularDrag = 3.0f;
        Destroy(this.gameObject);
        // 나중에 UI다루면서 게임 오버 시 정하기
    }
}
