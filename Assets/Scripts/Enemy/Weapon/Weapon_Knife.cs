using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Knife : MonoBehaviour
{
    public WeaponType type = WeaponType.Knife;

    public float attackPower = 10.0f;   // °ø°Ý·Â

    protected virtual void Start()
    {
        attackPower = 10.0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IHit hit = other.GetComponent<IHit>();
            if(hit != null)
            {
                if(type == WeaponType.Knife)
                {
                    hit.TakeDamage(attackPower);
                }
            }
            return;
        }
    }

}
