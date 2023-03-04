using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTank : MonoBehaviour, IHit
{
    public GameObject[] shellPrefabs;

    protected bool isDead = false;      // ��� ����

    protected Transform turret;         // ��ž
    protected Transform firePosition;   // �߻� ��ġ
    protected FireData[] fireDatas;     // �߻� ��Ÿ�� ����

    public float hp;
    public float maxHP;
    public float defencePower;
    public Vector3 hitPoint = Vector3.zero; // �浹 ��ġ

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

    // ������Ʈ
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
        if (!isDead)    // ���������
        {
            foreach (var data in fireDatas)
            {
                data.CurrentCoolTime -= Time.deltaTime; // ��ź ��Ÿ�� ó��
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
        // ���߿� UI�ٷ�鼭 ���� ���� �� ���ϱ�
    }
}
