using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Axe : MonoBehaviour
{
    public WeaponType type = WeaponType.Axe;

    public float attackPower = 30.0f;   // °ø°Ý·Â

    protected virtual void Start()
    {
        attackPower = 30.0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IHit hit = other.GetComponent<IHit>();
            if (hit != null)
            {
                if (type == WeaponType.Axe)
                {
                    hit.TakeDamage(attackPower);
                }
            }
            return;
        }
    }

}
