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
                    // 구매 불가 시 어떻게 할것인지 생각해보기
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

    // 플레이어 위치

    Vector3 dir;
    Vector3 oldDir = Vector3.down;

    Vector3Int currentMap = Vector3Int.one;     // 플레이어가 존재하는 맵의 번호
    Vector3 mapSize = new Vector3(100, 0,100);      // 맵 하나의 크기
    Vector3Int mapCount = new Vector3Int(3, 0,3); //맵의 갯수(가로,세로)
    Vector3 offset = Vector3.zero;
    public Vector3Int CurrentMap
    {
        set
        {
            if(currentMap != value)
            {
                currentMap = value;
                Debug.Log($"현재 맵의 위치 : {currentMap}");
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

        // 월드 원점에서 맵이 얼마나 이동해 있는가? => offset으로 저장
        offset = new Vector3(mapSize.x * mapCount.x * -0.5f,0, mapSize.z * mapCount.z * -0.5f);
    }

    protected override void Start()
    {
        base.Start();
        store = GameManager.Instance.Store.GetComponent<StoreManager>();
    }

    private void FixedUpdate()
    {
        rigid.AddForce(inputDir.y * moveSpeed * transform.forward); // 전진 후진
        rigid.AddTorque(inputDir.x * turnSpeed * transform.up);     // 좌회전 우회전
        //transform.Translate(inputDir * moveSpeed * Time.fixedDeltaTime, Space.Self);

        Vector3 pos = (Vector3)transform.position - offset; // 맵의 왼쪽 아래가 원점이라고 가정했을 때 나의 위치
        CurrentMap = new Vector3Int((int)(pos.x / mapSize.x),0 ,(int)(pos.z / mapSize.z));    // 위치를 맵 하나의 크기로 나누어서 (,)맵인지 계산
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
            //    lookDir.y = 0.0f;               // y 제거
            //}
            lookDir.y = 0.0f;               // y 제거
            lookDir = lookDir.normalized;   // 노멀라이즈.
            turretTargetRotation = Quaternion.LookRotation(lookDir);    // 최종적으로 포탑이 해야할 회전 계산
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
        //    // Update에서 계속 실행
        //    // turretTargetRotation이 될때까지 회전        
        //}
        turret.rotation = Quaternion.Slerp(turret.rotation, turretTargetRotation, turretTurnSpeed * Time.deltaTime);
    }
    private void Fire(ShellType type)
    {
        if (fireDatas[(int)type].IsFireReady)       // 쿨타임 확인하고 발사 가능하면
        {
            Instantiate(shellPrefabs[(int)type], firePosition.position, firePosition.rotation); // 지정된 포탄 발사
            fireDatas[(int)type].ResetCoolTime();   // 쿨타임 다시 돌리기
        }
    }
}
