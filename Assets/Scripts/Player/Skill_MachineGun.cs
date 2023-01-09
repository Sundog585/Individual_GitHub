using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_MachineGun : MonoBehaviour
{
    public GameObject sparkEffect;
    public GameObject lockOnEffect;
    Transform lockOnTarget;
    float lockOnRange = 5.0f;
    public Transform LockOnTarget { get => lockOnTarget; }  // 락온 대상의 트랜스폼. 읽기 전용 프로퍼티 추가

    private void Start()
    {
        if (lockOnEffect == null)
        {
            lockOnEffect = GameObject.Find("LockOnEffect");
            lockOnEffect.SetActive(false);
        }
        sparkEffect = lockOnEffect.gameObject.transform.GetChild(5).gameObject;
    }

    public void UseSkill()
    {
        if (lockOnTarget == null)
        {
            // 락온 시도
            LockOn();
        }
        else
        {
            // 락온된 타겟이 있음
            if (!LockOn())  // 다시 락온을 시도
            {
                // 새롭게 락온이 안되면 락온 풀기
                LockOff();
            }
        }
    }

    bool LockOn()
    {
        bool result = false;

        // transform.position지점에서 반경 lockOnRange 범위 안에 있는 Enemy레이어를 가진 컬라이더를 전부 찾기
        Collider[] cols = Physics.OverlapSphere(transform.position, lockOnRange, LayerMask.GetMask("Enemy"));

        // 하나라도 오버랩된것이 있을 때만 실행
        if (cols.Length > 0)
        {
            // 가장 가까운 컬라이더를 찾기
            Collider nearest = null;
            float nearestDistance = float.MaxValue;
            foreach (Collider col in cols)
            {
                float distanceSqr = (col.transform.position - transform.position).sqrMagnitude; // 거리의 제곱으로 체크
                if (distanceSqr < nearestDistance)
                {
                    nearestDistance = distanceSqr;
                    nearest = col;
                }
            }

            if (lockOnTarget?.gameObject != nearest.gameObject) // 다른 대상을 락온 할 때만 실생
            {
                if (LockOnTarget != null)
                {
                    LockOff();  // LockOff 델리게이트 연결 해제용
                }

                lockOnTarget = nearest.transform;   // 가장 가까이 있는 대상을 락온 대상으로 설정
                                                    //Debug.Log($"Lock on : {lockOnTarget.name}");

                //lockOnTarget.gameObject.GetComponent<Goblin_Warrior>().onDead += LockOff;

                lockOnEffect.transform.position = lockOnTarget.position;    // 락온 이팩트를 락온 대상의 위치로 이동
                lockOnEffect.transform.parent = lockOnTarget;               // 락온 이팩트의 부모를 락온 대상으로 설정
                lockOnEffect.SetActive(true);                               // 락온 이팩트 보여주기

                result = true;
            }
        }

        return result;
    }
    void LockOff()
    {
        //lockOnTarget.gameObject.GetComponent<Goblin_Warrior>().onDead -= LockOff;
        lockOnTarget = null;                    // 락온 대상 null
        lockOnEffect.transform.parent = null;   // 락온 이팩트의 부모 제거
        lockOnEffect.SetActive(false);          // 락온 이팩트 보이지 않게 하기
    }

}
