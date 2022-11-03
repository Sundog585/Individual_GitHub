using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Name Shell Data", menuName = "Scriptable Object/Shell Data", order = 0)]
public class ShellData : ScriptableObject
{
    [ColorUsage(true, true)]
    public Color shellColor;    // ��ź ����
    public float initialSpeed = 10.0f;  // �����Ǹ� ��� ����� �ӵ�
    public float coolTime = 1.0f;       // ��ź ��Ÿ��
    public float damage = 50.0f;        // ������

    public GameObject explosionPrefab;  // ���� ����Ʈ ������

    /// <summary>
    /// ������ ����Ʈ ó����
    /// </summary>
    /// <param name="position">����Ʈ�� ������ ��ġ</param>
    /// <param name="normal">���� ���� normal ����</param>
    public virtual void Explosion(Vector3 position, Vector3 normal)
    {
        // ���� ��ġ�� �浹 ����, ������ ���� ȸ���� �浹������ ��ֹ��͸� forward�� �����ϴ� ȸ��
        Instantiate(explosionPrefab, position, Quaternion.LookRotation(normal));
    }

    /// <summary>
    /// ������ ������
    /// </summary>
    /// <param name="target">������ ���</param>
    public virtual void TakeDamage(IHit target)
    {
        if(target != null)
        {
            target.HP -= damage;
        }
    }
}
