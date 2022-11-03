using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiegeShellExplosion : MonoBehaviour
{
    public float damage = 50;
    //List<IHit> targetList = null;

    private void Awake()
    {
        //targetList = new List<IHit>(8);
    }

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
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))    // 플레이어가 아닌 대상이 나갔을 경우
        {
            //targetList.Remove(other.gameObject.GetComponent<IHit>());   // 리스트에서 제거
            Invoke("Fin", 3.0f);
        }
    }

    public void Fin()
    {
        Destroy(this.gameObject);
    }
}
