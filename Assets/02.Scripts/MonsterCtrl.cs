using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 내비게이션 기능을 사용하기 위해 추가해야 하는 네임스페이스
using UnityEngine.AI;

public class MonsterCtrl : MonoBehaviour
{
    // 몬스터의 상태 정보
    public enum State { IDLE, TRACE, ATTACK, DIE }

    // 몬스터 컴포넌트 변수들
    private Transform playerTr;
    private NavMeshAgent agent;
    private Animator anim;
    
    // 몬스터의 현재 상태
    public State state = State.IDLE;
    public float traceDist = 10.0f; // 추적 사정거리
    public float attackDist = 2.0f; // 공격 사정거리
    private int hp = 100; // 몬스터 체력
    
    public bool isDie = false; // 몬스터 사망 플래그 -> OnPlayerDie()에서 사용

    // 혈흔 효과 프리팹
    public GameObject bloodEffect;

    void Start()
    {
    }

    // 일정한 간격으로 몬스터의 행동 상태를 체크
    IEnumerator CheckMonsterState()
    {
        // while() 사용 <- Start()에서 한번 실행한 후 관리 간편 때문
        while (true)
        {
            // 0.3초 동안 중지(대기)하는 동안 제어권을 메시지 루프에 양보
            yield return new WaitForSeconds(0.3f);

            // 몬스터의 상태가 DIE일 때 코루틴을 종료
            if (state == State.DIE) yield break;

            // 몬스터와 플레이어 사이의 거리 측정
            float distance = Vector3.Distance(playerTr.position, transform.position);
            
            if (distance <= attackDist) state = State.ATTACK; // 공격 사거리 내
            else if (distance <= traceDist) state = State.TRACE; // 추적 사거리 내
            else state = State.IDLE; // 그 외 -> 대기
        }
    }

    // 몬스터의 상태에 따라 몬스터의 동작을 수행
    IEnumerator MonsterAction()
    {
        while (true)
        {
            // 0.3초 동안 중지(대기)하는 동안 제어권을 메시지 루프에 양보
            yield return new WaitForSeconds(0.3f);
            
            switch (state)
            {
                // 1. 대기 상태
                case State.IDLE:
                    agent.isStopped = true; // 이동 X
                    
                    // Animator의 IsTrace 변수를 false로 설정
                    anim.SetBool("IsTrace", false);
                    break;

                // 2. 추적 상태
                case State.TRACE:
                    agent.isStopped = false; // 이동 O
                    
                    // 추적 대상의 좌표로 이동 시작
                    agent.SetDestination(playerTr.position);
                    // Animator 변수
                    anim.SetBool("IsTrace", true); // 추적 활성화
                    anim.SetBool("IsAttack", false); // 공격 비활성화
                    break;

                // 3. 공격 상태
                case State.ATTACK:
                    // Animator 변수
                    anim.SetBool("IsAttack", true); // 공격 활성화
                    break;

                // 4. 사망
                case State.DIE:
                    agent.isStopped = true; // 추적 X
                    isDie = true;

                    // 사망 애니메이션 실행
                    anim.SetTrigger("Die");
                    
                    // 자식까지 포함해서 Collider 비활성화
                    Collider[] cols1 = GetComponentsInChildren<Collider>();
                    foreach (Collider col in cols1)
                        col.enabled = false;   // 충돌 비활성화

                    // 일정 시간 대기 후 오브젝트 풀링으로 환원
                    yield return new WaitForSeconds(3.0f);

                    // 사망 후 다시 사용할 때를 위해 hp 값 초기화 및 플래그, 상태 초기화
                    hp = 100;
                    isDie = false;
                    state = State.IDLE;

                    // 자식까지 포함해서 Collider 활성화
                    Collider[] cols2 = GetComponentsInChildren<Collider>();
                    foreach (Collider col in cols2)
                        col.enabled = true;
                    
                    // 몬스터 비활성화 -> 생성되면 그때 다시 활성화
                    this.gameObject.SetActive(false);

                    break;
            }
        }
    }

    // 충돌 처리
    void OnCollisionEnter(Collision coll)
    {
        // Bullet 태그면 몬스터 충돌 효과 생성 및 hp 차감
        if (coll.collider.CompareTag("Bullet"))
        {
            Destroy(coll.gameObject); // 충돌한 총알을 삭제
            anim.SetTrigger("Hit"); // 피격 애니메이션 실행
            
            // 총알의 충돌 지점
            ContactPoint cp = coll.GetContact(0);
            // 총알의 충돌 지점의 법선 벡터
            Quaternion rot = Quaternion.LookRotation(-cp.normal);
            // 혈흔 효과 생성
            GameObject blood = Instantiate(bloodEffect, cp.point, rot, transform);
            Destroy(blood, 1.0f); // 혈흔 효과 제거

            // 몬스터의 hp 차감
            hp -= 10;
            if (hp <= 0 && isDie == false)
            {
                state = State.DIE; // 체력 0 이하면 사망 상태로 변경
                isDie = true;
                // 몬스터 사망 시 점수 추가
                GameManager.instance.DisplayScore(50);
            }
        }
    }
    
    // 스크립트가 활성화될 때마다 호출되는 함수
    void OnEnable()
    {
        // 이벤트 발생 시 수행할 함수 연결: 구독
        PlayerCtrl.OnPlayerDie += this.OnPlayerDie;
        
        // 스크립트 활성화 시 코루틴 실행
        StartCoroutine(CheckMonsterState()); // 몬스터 상태 체크
        StartCoroutine(MonsterAction()); // 몬스터 행동 수행
    }

    // 스크립트가 비활성화될 때마다 호출되는 함수
    void OnDisable()
    {
        // 기존에 연결된 함수 해제: 구독 해제
        PlayerCtrl.OnPlayerDie -= this.OnPlayerDie;
    }
    
    // 플레이어 사망 시(플레이어 스크립트에서 처리) 구독한 함수들 실행
    void OnPlayerDie()
    {
        // 몬스터의 상태를 체크하는 코루틴 함수를 모두 정지시킴
        StopAllCoroutines();
        
        // 몬스터가 사망한 상태라면 승리 애니메이션 X
        if (isDie) return;

        // 추적을 정지하고 애니메이션을 수행
        agent.isStopped = true; // 이동 X
        anim.SetFloat("Speed", Random.Range(0.8f, 1.2f)); 
        anim.SetTrigger("PlayerDie");
    }

    void Awake()
    {
        // 컴포넌트 캐싱
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }
}