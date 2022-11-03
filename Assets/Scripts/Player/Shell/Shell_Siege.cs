using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell_Siege : Shell
{
    protected override void OnCollisionEnter(Collision collision)
    {
        Ray ray = new Ray(collision.contacts[0].point, Vector3.down);
        Vector3 position;
        if (Physics.Raycast(ray, out RaycastHit hit,1000.0f, LayerMask.GetMask("Ground")))
        {
            position = hit.point;
        }
        else
        {
            position = collision.contacts[0].point;
        }
        position += new Vector3(0, 0.1f, 0);

        Data.Explosion(position, Vector3.up);

        IHit hitTarget = collision.gameObject.GetComponent<IHit>();
        Data.TakeDamage(hitTarget);

        // 스스로 없어지기
        Destroy(this.gameObject);
    }
}
