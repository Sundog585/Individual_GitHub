using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Shell Data(Siege Mode)", menuName = "Scriptable Object/Shell Data(Siege Mode)", order = 2)]
public class ShellData_Siege : ShellData
{
    public GameObject fire;

    public override void Explosion(Vector3 position, Vector3 normal)
    {
        base.Explosion(position, normal);
        GameObject obj = Instantiate(fire, position, Quaternion.LookRotation(normal));

        ParticleSystem[] pss = obj.GetComponentsInChildren<ParticleSystem>();
        pss[0].Play();
    }
}
