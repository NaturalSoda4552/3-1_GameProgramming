using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MonsterCtrl : MonoBehaviour
{
    private Transform playerTr;
    private NavMeshAgent agent;

    public float traceDist = 10.0f;
    public float attackDist = 2.0f;
    
    private float monsterHp = 100.0f;
    
    private Animator anim;
    
    public enum State
    {
        IDLE, WALK, ATTACK, DIE, PLAYERDIE
    }
    
    public State state = State.IDLE;
    
    // 한 번만 실행하기 위한 플래그
    private bool playerDieTriggered = false;
    private bool isDead = false; // DIE 상태 관련

    void Start()
    {
        playerTr = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        
        agent.isStopped = true;
        anim.SetBool("IsTrace", false);
        anim.SetBool("IsAttack", false);
        
        // 지속적인 상태 업데이트 코루틴 실행
        StartCoroutine(MonsterBehaviour());
    }

    IEnumerator MonsterBehaviour()
    {
        while (true)
        {
            // 상태가 DIE 또는 PLAYERDIE일 경우 한 번만 트리거 처리
            if (state == State.DIE || state == State.PLAYERDIE)
            {
                if (!isDead)
                {
                    isDead = true;
                    yield return new WaitForSeconds(0.3f);
                    ExecuteActionForState(state);
                }
                yield break; // 상태가 죽으면 코루틴 종료
            }
            else
            {
                // 일반 상태 업데이트
                yield return MonsterState();
                yield return MonsterAction();
            }
        }
    }

    IEnumerator MonsterState()
    {
        yield return new WaitForSeconds(0.3f);
        
        float distance = Vector3.Distance(playerTr.position, transform.position);

        if (monsterHp <= 0)
        {
            state = State.DIE;
        }
        else
        {
            if (distance < traceDist)
            {
                state = State.WALK;
                anim.SetBool("IsTrace", true);
                agent.isStopped = false;
                agent.SetDestination(playerTr.position);

                if (distance < attackDist)
                {
                    state = State.ATTACK;
                    anim.SetBool("IsAttack", true);
                }
            }
            else
            {
                state = State.IDLE;
                agent.isStopped = true;
                anim.SetBool("IsTrace", false);
                anim.SetBool("IsAttack", false);
            }
        }
    }

    IEnumerator MonsterAction()
    {
        yield return new WaitForSeconds(0.3f);
        ExecuteActionForState(state);
    }

    // 상태에 따른 액션 실행을 한 곳에서 처리
    void ExecuteActionForState(State currentState)
    {
        switch (currentState)
        {
            case State.IDLE:
                agent.isStopped = true;
                anim.SetBool("IsTrace", false);
                anim.SetBool("IsAttack", false);
                break;
            case State.WALK:
                agent.isStopped = false;
                anim.SetBool("IsTrace", true);
                break;
            case State.ATTACK:
                agent.isStopped = false;
                anim.SetBool("IsAttack", true);
                break;
            case State.DIE:
                agent.isStopped = true;
                anim.SetTrigger("Die");
                GetComponent<Collider>().enabled = false;
                break;
            case State.PLAYERDIE:
                // 이미 트리거 된 적이 없다면 실행
                if (!playerDieTriggered)
                {
                    playerDieTriggered = true;
                    agent.isStopped = true;
                    anim.SetTrigger("PlayerDie");
                    GetComponent<Collider>().enabled = false;
                }
                break;
        }
    }
    
    void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.CompareTag("Bullet"))
        {
            anim.SetTrigger("Hit");
            monsterHp -= 10;            
            Destroy(coll.gameObject);
        }
    }
    
    void OnEnable()
    {
        PlayerCtrl.OnPlayerDie += this.OnPlayerDie;
    }
    void OnDisable()
    {
        PlayerCtrl.OnPlayerDie -= this.OnPlayerDie;
    }
    
    void OnPlayerDie()
    {
        state = State.PLAYERDIE;
    }
}