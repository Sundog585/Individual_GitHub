using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiegeShellExplosion : MonoBehaviour
{
    public float damage = 50;

    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("Player"))
        {
            IHit hit = other.gameObject.GetComponent<IHit>();
            if(hit != null)
            {
                hit.TakeDamage(damage);
                //targetList.Add(hit);
                //foreach (var target in targetList)
                //{
                //    target.HP -= damage;
                //}
                // ����� �װ� ������ ����Ʈ�� �����°� ����
                Destroy(this.gameObject, 3.0f);
            }
        }
    }
    //private void OnTriggerExit(Collider other)
    //{
    //    if (!other.CompareTag("Player"))    // �÷��̾ �ƴ� ����� ������ ���
    //    {
    //        //targetList.Remove(other.gameObject.GetComponent<IHit>());   // ����Ʈ���� ����
    //        Invoke("Fin", 3.0f);
    //    }
    //}

    //public void Fin()
    //{
    //    Destroy(this.gameObject);
    //}
}
