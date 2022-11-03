using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Shell : MonoBehaviour
{
    public ShellData data;
    PlayerTank player;
    public ShellData Data
    {
        get { return data; }
    }

    public float startDamage = 20;

    protected Rigidbody rigid;

    Material material;

    protected virtual void Awake()
    {
        player = GameManager.Instance.Player.GetComponent<PlayerTank>();
        rigid = GetComponent<Rigidbody>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        material = meshRenderer.material;
        material.SetColor("_EffectColor", Data.shellColor);
    }

    protected virtual void Start()
    {
        rigid.velocity = transform.forward * Data.initialSpeed;
        Data.damage = player.shellDamage;
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            // 충돌 시 폭파
            Data.Explosion(collision.contacts[0].point, collision.contacts[0].normal);

            // 맞은 대상 데미지 주기
            IHit hitTarget = collision.gameObject.GetComponent<IHit>();
            if (hitTarget != null)
            {
                hitTarget.TakeDamage(Data.damage);
            }

            Destroy(this.gameObject);   // 포탄 삭제
        }
    }
}
