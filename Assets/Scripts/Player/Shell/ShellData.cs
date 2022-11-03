using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Name Shell Data", menuName = "Scriptable Object/Shell Data", order = 0)]
public class ShellData : ScriptableObject
{
    [ColorUsage(true, true)]
    public Color shellColor;    // 포탄 색깔
    public float initialSpeed = 10.0f;  // 생성되면 즉시 적용될 속도
    public float coolTime = 1.0f;       // 포탄 쿨타임
    public float damage = 50.0f;        // 데미지

    public GameObject explosionPrefab;  // 폭발 이펙트 프리펩

    /// <summary>
    /// 터지는 이팩트 처리용
    /// </summary>
    /// <param name="position">이팩트가 생성될 위치</param>
    /// <param name="normal">터진 면의 normal 벡터</param>
    public virtual void Explosion(Vector3 position, Vector3 normal)
    {
        // 생성 위치는 충돌 지점, 생성될 때의 회전은 충돌지점의 노멀백터를 forward로 지정하는 회전
        Instantiate(explosionPrefab, position, Quaternion.LookRotation(normal));
    }

    /// <summary>
    /// 데미지 입히기
    /// </summary>
    /// <param name="target">데미지 대상</param>
    public virtual void TakeDamage(IHit target)
    {
        if(target != null)
        {
            target.HP -= damage;
        }
    }
}
