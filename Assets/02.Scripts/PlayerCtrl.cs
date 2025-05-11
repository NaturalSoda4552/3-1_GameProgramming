using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerCtrl : MonoBehaviour
{
    private Transform tr; // 이동 변수
    private Animation anim; // 이동 애니메이션 변수

    public float moveSpeed = 5.0f; // 이동 속도 
    public float turnSpeed = 80.0f; // 회전 속도

    private readonly float initHp = 100.0f; // 초기화 할 체력
    public float currHp; // 현재 체력
    private Image hpBar;
    
    public delegate void PlayerDieHandler();
    public static event PlayerDieHandler OnPlayerDie; // 이벤트 선언
    
    public bool isDie = false; // 플레이어 사망 플래그
    
    private GameObject HpScorePanel; // hp&score 패널 오브젝트
    
    void Start()
    {
        // 컴포넌트 캐싱 (private: 자기 자신)
        tr = GetComponent<Transform>();
        anim = GetComponent<Animation>();
        
        // 초기 설정
        anim.Play("Idle"); // idle 애니메이션 재생
        currHp = initHp; // 체력 초기화
        
        // Hpbar 연결
        hpBar = GameObject.FindGameObjectWithTag("hpBar")?.GetComponent<Image>();
        HpScorePanel = GameObject.FindGameObjectWithTag("HpAndScorePanel");
        HpScorePanel.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // 1. 키보드 입력
        float v = Input.GetAxis("Vertical"); // W/S
        float h = Input.GetAxis("Horizontal"); // A/D 
        float r = Input.GetAxis("Mouse X"); // 마우스 가로 이동
        
        // 2. 이동 처리
        // 2-1. 이동 방향
        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h); 
        // 2-2. 실제 이동, Translate(이동 방향 * 속력 * Time.deltaTime)
        tr.Translate(moveDir.normalized * moveSpeed * Time.deltaTime);
        // 2-3. 시점 이동, Vector3.up 축을 기준으로 turnSpeed만큼의 속도로 회전
        tr.Rotate(Vector3.up * turnSpeed * Time.deltaTime * r);
        
        // 3. 이동 애니메이션
        PlayerAnim(h, v);
    }
    void PlayerAnim(float h, float v)
    {
        // 키보드 입력값을 기준으로 동작할 애니메이션 수행
        if (v >= 0.1f) 
            anim.CrossFade("RunF", 0.25f);  // 전진 애니메이션 실행
        else if (v <= -0.1f) 
            anim.CrossFade("RunB", 0.25f);  // 후진 애니메이션 실행
        else if (h >= 0.1f) 
            anim.CrossFade("RunR", 0.25f);  // 오른쪽 이동 애니메이션 실행
        else if (h <= -0.1f) 
            anim.CrossFade("RunL", 0.25f);  // 왼쪽 이동 애니메이션 실행
        else if (Input.GetMouseButtonDown(0) && (h == 0 && v == 0))
            anim.Play("IdleFireSMG"); // 정지 사격 애니메이션
        else 
            anim.CrossFade("Idle", 0.25f);   // 정지 시 Idle 애니메이션 실행
    }

    // 충돌 처리
    void OnTriggerEnter(Collider coll)
    {
        // 몬스터의 punch 태그면 Player의 HP 차감
        if (currHp >= 0.0f && coll.CompareTag("Punch"))
        {
            currHp -= 10.0f; // 10씩 감소
            hpBar.fillAmount = currHp / initHp;
            
            // Player의 hp가 0 이하면 사망 처리
            if (currHp <= 0.0f && isDie == false)
            {
                isDie = true; // 사망 플래그 true
                PlayerDie(); // 플레이어 사망 함수 호출
            }
        }
    }
    
    // Player 사망 처리
    void PlayerDie()
    {
        // Debug.Log("Player Die !");

        GameManager.instance.IsGameOver = true;
        
        // 주인공 사망 이벤트 호출(발생)
        OnPlayerDie();
    }
    
}
