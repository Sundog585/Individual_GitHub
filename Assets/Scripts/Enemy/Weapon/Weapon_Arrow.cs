using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Arrow : MonoBehaviour
{
    public float arrowSpeed = 10;
    public float arrowDamage = 5;
    Rigidbody rigid;


    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        rigid.velocity = transform.forward * arrowSpeed;
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // 맞은 대상 데미지 주기
            IHit hitTarget = collision.gameObject.GetComponent<IHit>();
            if (hitTarget != null)
            {
                hitTarget.TakeDamage(arrowDamage);
            }
            Destroy(this.gameObject);

        }
        Destroy(this.gameObject, 1.5f);   // 포탄 삭제
    }
}
