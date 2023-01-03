using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
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
    
    //경사로 이동------------------------------------------------------
    public Vector3 direction { get; private set; }
    float maxSlopeAngle = 70.0f;        // 캐릭터가 등반할 수 있는 최대 경사 각도
    public Transform raycastOrigin;    // 경사 지형을 체크할 Raycast 발사 시작 지점
    const float RAY_DISTANCE = 2f;
    RaycastHit slopeHit;
    public Transform groundCheck;      // 캐릭터가 땅에 붙어 있는지 확인하기 위한 CheckBox 시작 지점.
    int groundLayer;

    Slider HpBar;
    StoreManager store;

    Quaternion turretTargetRotation = Quaternion.identity;

    Vector2 inputDir = Vector2.zero;

    // 플레이어 위치
    Vector3Int currentMap = Vector3Int.one;     // 플레이어가 존재하는 맵의 번호
    Vector3 mapSize = new Vector3(100, 0,100);      // 맵 하나의 크기
    Vector3Int mapCount = new Vector3Int(3, 0,3); //맵의 갯수(가로,세로)
    Vector3 offset = Vector3.zero;
    public Vector3Int CurrentMap
    {
        get => currentMap;
        set
        {
            if(currentMap != value)
            {
                currentMap = value;
                Debug.Log($"현재 맵의 위치 : {currentMap}");
                onMapChange?.Invoke(currentMap);
            }
        }
    }
    public Action<Vector3Int> onMapChange;

    // 스킬 베리어 ----------------------------------------------------------------------------------------
    private Skill_Barrier barrier;

    protected override void Awake()
    {
        base.Awake();

        barrier = GetComponent<Skill_Barrier>();

        inputActions = new PlayerInputSystem();
        siegeTankMode = () => { SiegeTankMode(); };
        normalTankMode = () => { NormalMode(); };

        // 월드 원점에서 맵이 얼마나 이동해 있는가? => offset으로 저장
        offset = new Vector3(mapSize.x * mapCount.x * -0.5f,0, mapSize.z * mapCount.z * -0.5f);

        groundLayer = 1 << LayerMask.NameToLayer("Ground");
    }

    protected override void Start()
    {
        base.Start();
        HpBar = GameManager.Instance.PlayerUI.GetComponentInChildren<Slider>();
        onHealthChange += (ratio) =>
        {
            HpBar.value = ratio;
        };

        store = GameManager.Instance.Store.GetComponent<StoreManager>();

        CoolTimePanel coolTimePanel = FindObjectOfType<CoolTimePanel>();
        barrier.onCoolTimeChange += coolTimePanel[0].RefreshUI;
        barrier.onDurationTimeChange += coolTimePanel[0].RefreshUI;
        barrier.onDurationMode += coolTimePanel[0].SetDurationMode;
        fireDatas[1].onCoolTimeChange += coolTimePanel[1].RefreshUI;
    }

    private void FixedUpdate()
    {
        Move();

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
        inputActions.Player.Skill_Barrier.performed += OnBarrierActivate;
    }


    private void OnDisable()
    {
        inputActions.Player.Skill_Barrier.performed -= OnBarrierActivate;
        inputActions.Player.StoreOpen.performed -= OnStoreOpen;
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
    private void OnBarrierActivate(InputAction.CallbackContext _)
    {
        barrier.UseSkill();
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


    public void OnMove(InputAction.CallbackContext context)
    {
        inputDir = context.ReadValue<Vector2>();
        direction = new Vector3(inputDir.x, 0f, inputDir.y);
    }



    private void TurretTurn()
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

    public override void TakeDamage(float damage)
    {
        if (!barrier.IsSkillActivate)
        {
            base.TakeDamage(damage);
        }
    }


    // 경사로 함수들------------

    private void Move()
    {
        //transform.Translate(inputDir * moveSpeed * Time.fixedDeltaTime, Space.Self);
        bool isOnSlope = IsOnSlope();
        bool isGrounded = IsGrounded();

        Vector3 velocity = CalculateNextFrameGroundAngle(moveSpeed) < maxSlopeAngle ? direction : Vector3.zero;
        //Vector3 gravity = Vector3.down * Mathf.Abs(rigid.velocity.y);

        if(isGrounded && isOnSlope)
        {
            //velocity = AdjustDirectionToSlope(direction);   // 여기 더 고민 해보기
            //rigid.constraints = RigidbodyConstraints.FreezePositionY;
            //transform.rotation = Quaternion.LookRotation(AdjustDirectionToSlope(direction));
            //rigid.useGravity = false;
            //rigid.useGravity = false;
        }
        else
        {
            //rigid.constraints = RigidbodyConstraints.FreezePositionY;
            rigid.useGravity = true;
            //transform.rotation = Quaternion.LookRotation(new Vector3(0, 0, 0));
        }

        rigid.AddForce(inputDir.y * moveSpeed * transform.forward); // 전진 후진
        rigid.AddTorque(inputDir.x * turnSpeed * transform.up);     // 좌회전 우회전
        //LookAt();
        //rigid.velocity = velocity * currentMoveSpeed + gravity;
    }
    private float CalculateNextFrameGroundAngle(float moveSpeed)
    {
        var nextFramePlayerPosition = raycastOrigin.position + direction * moveSpeed * Time.fixedDeltaTime; // 다음 프레임 탱크 위치

        if (Physics.Raycast(nextFramePlayerPosition, Vector3.down, out RaycastHit hitInfo, RAY_DISTANCE, groundLayer))
        {
            return Vector3.Angle(Vector3.up, hitInfo.normal);
        }
        return 0f;
    }

    public bool IsGrounded()
    {
        Vector3 boxSize = new Vector3(transform.lossyScale.x, 0.4f, transform.lossyScale.z);
        return Physics.CheckBox(groundCheck.position, boxSize, Quaternion.identity, groundLayer);
    }

    public bool IsOnSlope()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        if(Physics.Raycast(ray, out slopeHit, RAY_DISTANCE, groundLayer))
        {
            var angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle != 0f && angle < maxSlopeAngle;
        }
        return false;
    }
    private void LookAt()
    {
        if (direction != Vector3.zero)
        {
            Quaternion targetAngle = Quaternion.LookRotation(direction);
            rigid.rotation = targetAngle;
        }
    }

    private Vector3 AdjustDirectionToSlope(Vector3 direction)
    {
        Vector3 adjustVelocityDirection = Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
        return adjustVelocityDirection;
    }



}
