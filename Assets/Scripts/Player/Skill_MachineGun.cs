using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_MachineGun : MonoBehaviour
{
    public GameObject sparkEffect;
    public GameObject lockOnEffect;
    Transform lockOnTarget;
    float lockOnRange = 5.0f;
    public Transform LockOnTarget { get => lockOnTarget; }  // ���� ����� Ʈ������. �б� ���� ������Ƽ �߰�

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
            // ���� �õ�
            LockOn();
        }
        else
        {
            // ���µ� Ÿ���� ����
            if (!LockOn())  // �ٽ� ������ �õ�
            {
                // ���Ӱ� ������ �ȵǸ� ���� Ǯ��
                LockOff();
            }
        }
    }

    bool LockOn()
    {
        bool result = false;

        // transform.position�������� �ݰ� lockOnRange ���� �ȿ� �ִ� Enemy���̾ ���� �ö��̴��� ���� ã��
        Collider[] cols = Physics.OverlapSphere(transform.position, lockOnRange, LayerMask.GetMask("Enemy"));

        // �ϳ��� �������Ȱ��� ���� ���� ����
        if (cols.Length > 0)
        {
            // ���� ����� �ö��̴��� ã��
            Collider nearest = null;
            float nearestDistance = float.MaxValue;
            foreach (Collider col in cols)
            {
                float distanceSqr = (col.transform.position - transform.position).sqrMagnitude; // �Ÿ��� �������� üũ
                if (distanceSqr < nearestDistance)
                {
                    nearestDistance = distanceSqr;
                    nearest = col;
                }
            }

            if (lockOnTarget?.gameObject != nearest.gameObject) // �ٸ� ����� ���� �� ���� �ǻ�
            {
                if (LockOnTarget != null)
                {
                    LockOff();  // LockOff ��������Ʈ ���� ������
                }

                lockOnTarget = nearest.transform;   // ���� ������ �ִ� ����� ���� ������� ����
                                                    //Debug.Log($"Lock on : {lockOnTarget.name}");

                //lockOnTarget.gameObject.GetComponent<Goblin_Warrior>().onDead += LockOff;

                lockOnEffect.transform.position = lockOnTarget.position;    // ���� ����Ʈ�� ���� ����� ��ġ�� �̵�
                lockOnEffect.transform.parent = lockOnTarget;               // ���� ����Ʈ�� �θ� ���� ������� ����
                lockOnEffect.SetActive(true);                               // ���� ����Ʈ �����ֱ�

                result = true;
            }
        }

        return result;
    }
    void LockOff()
    {
        //lockOnTarget.gameObject.GetComponent<Goblin_Warrior>().onDead -= LockOff;
        lockOnTarget = null;                    // ���� ��� null
        lockOnEffect.transform.parent = null;   // ���� ����Ʈ�� �θ� ����
        lockOnEffect.SetActive(false);          // ���� ����Ʈ ������ �ʰ� �ϱ�
    }

}
