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
                // 고블린이 죽고 나서도 이팩트에 들어오는거 같음
                Destroy(this.gameObject, 3.0f);
            }
        }
    }
    //private void OnTriggerExit(Collider other)
    //{
    //    if (!other.CompareTag("Player"))    // 플레이어가 아닌 대상이 나갔을 경우
    //    {
    //        //targetList.Remove(other.gameObject.GetComponent<IHit>());   // 리스트에서 제거
    //        Invoke("Fin", 3.0f);
    //    }
    //}

    //public void Fin()
    //{
    //    Destroy(this.gameObject);
    //}
}
