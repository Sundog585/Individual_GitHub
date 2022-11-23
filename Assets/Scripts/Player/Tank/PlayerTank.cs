using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerTank : BaseTank, IMoney
{

    enum ShellType
    {
        Normal = 0,
        Siege
    }

    public float shellDamage = 20.0f;


    public float ShellDamge
    {
        get => shellDamage;
        set
        {
            if(value != shellDamage)
            {
                shellDamage = value;
                onDamageChange?.Invoke(shellDamage);
            }

        }
    }

    public float DefencePower
    {
        get => defencePower;
        set
        {
            if(value != defencePower)
            {
                defencePower = value;
                onDefenceChange?.Invoke(defencePower);
            }
        }
        
    }

    public bool StoreOn { get; set; }

    public int money = 3;

    public int Money
    {
        get => money;
        set
        {
            if (value != money)
            {
                money = value;
                if (money <= 0)
                {
                    money = 0;
                    // ���� �Ұ� �� ��� �Ұ����� �����غ���
                }
                onMoneyChange?.Invoke(money);
            }
        }
    }

    // IMoney
    public Action<float> onMoneyChange { get; set; }
    public Action<float> onDamageChange { get; set; }
    public Action<float> onDefenceChange { get; set; }
    //--------------------------------------------------------------
    public static Action siegeTankMode;
    public static Action normalTankMode;
 

    PlayerInputSystem inputActions;

    //private bool zoomMode = false;
    public float moveSpeed = 3.0f;
    public float turnSpeed = 3.0f;
    public float turretTurnSpeed = 10.0f;
    //float mouseY = 0;
    public bool zoomMode;

    Slider HpBar;
    StoreManager store;

    Quaternion turretTargetRotation = Quaternion.identity;

    Vector2 inputDir = Vector2.zero;

    // �÷��̾� ��ġ

    Vector3 dir;
    Vector3 oldDir = Vector3.down;

    Vector3Int currentMap = Vector3Int.one;     // �÷��̾ �����ϴ� ���� ��ȣ
    Vector3 mapSize = new Vector3(100, 0,100);      // �� �ϳ��� ũ��
    Vector3Int mapCount = new Vector3Int(3, 0,3); //���� ����(����,����)
    Vector3 offset = Vector3.zero;
    public Vector3Int CurrentMap
    {
        set
        {
            if(currentMap != value)
            {
                currentMap = value;
                Debug.Log($"���� ���� ��ġ : {currentMap}");
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();

        HpBar = GetComponentInChildren<Slider>();
        onHealthChange += (ratio) =>
        {
            HpBar.value = ratio;
        };
        inputActions = new PlayerInputSystem();
        siegeTankMode = () => { SiegeTankMode(); };
        normalTankMode = () => { NormalMode(); };

        // ���� �������� ���� �󸶳� �̵��� �ִ°�? => offset���� ����
        offset = new Vector3(mapSize.x * mapCount.x * -0.5f,0, mapSize.z * mapCount.z * -0.5f);
    }

    protected override void Start()
    {
        base.Start();
        store = GameManager.Instance.Store.GetComponent<StoreManager>();
    }

    private void FixedUpdate()
    {
        rigid.AddForce(inputDir.y * moveSpeed * transform.forward); // ���� ����
        rigid.AddTorque(inputDir.x * turnSpeed * transform.up);     // ��ȸ�� ��ȸ��
        //transform.Translate(inputDir * moveSpeed * Time.fixedDeltaTime, Space.Self);

        Vector3 pos = (Vector3)transform.position - offset; // ���� ���� �Ʒ��� �����̶�� �������� �� ���� ��ġ
        CurrentMap = new Vector3Int((int)(pos.x / mapSize.x),0 ,(int)(pos.z / mapSize.z));    // ��ġ�� �� �ϳ��� ũ��� ����� (,)������ ���
    }

    protected override void Update()
    {
        base.Update();
        TurretTurn();    
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;
        inputActions.Player.Look.performed += OnMouseMove;
        inputActions.Player.Look.canceled += OnMouseMove;
        inputActions.Player.NormalFire.performed += OnNormalFire;
        inputActions.Player.StoreOpen.performed += OnStoreOpen;
    }


    private void OnDisable()
    {
        inputActions.Player.NormalFire.performed -= OnNormalFire;
        inputActions.Player.Look.canceled -= OnMouseMove;
        inputActions.Player.Look.performed -= OnMouseMove;
        inputActions.Player.Move.canceled -= OnMove;
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Disable();
    }


    private void OnNormalFire(InputAction.CallbackContext context)
    {
        if (!CameraManager.zoomMode)//
        {

            Fire(ShellType.Normal);
        }
        else
        {
            Fire(ShellType.Siege);
        }
    }

    private void OnMouseMove(InputAction.CallbackContext context)
    {
        Vector2 screenPos = context.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit Hit, 1000.0f, LayerMask.GetMask("Ground")))
        {
            Vector3 lookDir = Hit.point - turret.transform.position;
            //if (!CameraManager.zoomMode)
            //{
            //    lookDir.y = 0.0f;               // y ����
            //}
            lookDir.y = 0.0f;               // y ����
            lookDir = lookDir.normalized;   // ��ֶ�����.
            turretTargetRotation = Quaternion.LookRotation(lookDir);    // ���������� ��ž�� �ؾ��� ȸ�� ���
        }
        //Vector2 pos = context.ReadValue<Vector2>();

    }
    private void OnStoreOpen(InputAction.CallbackContext context)
    {
        if (StoreOn)
        {
            StoreOn = false;
            store.OpenStore();
        }
        else
        {
            StoreOn = true;
            store.OnClickClose();
        }
    }

    public void SiegeTankMode()
    {
        rigid.isKinematic = true;
    }
    public void NormalMode()
    {
        rigid.isKinematic = false;
    }


    private void OnMove(InputAction.CallbackContext context)
    {
        inputDir = context.ReadValue<Vector2>();
    }

    void TurretTurn()
    {
        //if (!isDead)
        //{
        //    // Update���� ��� ����
        //    // turretTargetRotation�� �ɶ����� ȸ��        
        //}
        turret.rotation = Quaternion.Slerp(turret.rotation, turretTargetRotation, turretTurnSpeed * Time.deltaTime);
    }
    private void Fire(ShellType type)
    {
        if (fireDatas[(int)type].IsFireReady)       // ��Ÿ�� Ȯ���ϰ� �߻� �����ϸ�
        {
            Instantiate(shellPrefabs[(int)type], firePosition.position, firePosition.rotation); // ������ ��ź �߻�
            fireDatas[(int)type].ResetCoolTime();   // ��Ÿ�� �ٽ� ������
        }
    }
}
