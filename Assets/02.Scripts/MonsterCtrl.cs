using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MonsterCtrl : MonoBehaviour
{
    private Transform playerTr;
    private NavMeshAgent agent;

    public float traceDist = 10.0f;
    public float attackDist = 2.0f;
    
    private Animator anim;

    public enum State
    {
        IDLE, WALK, ATTACK
    }
    
    public State state = State.IDLE;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerTr = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();

        anim = GetComponent<Animator>();
        
        
        agent.isStopped = true;
        anim.SetBool("IsTrace", false);
        anim.SetBool("IsAttack", false);
    }
    
    // 공격당할 시 경직 변수
    private bool isStunned = false;

    // Update is called once per frame
    void Update()
    {
        
        if (isStunned) return;
        StartCoroutine(MonsterState());
        StartCoroutine(MonsterAction());
    }

    IEnumerator MonsterState()
    {
        // 0.3초 동안 대기(정지)하는 동안 메시지 루프로 제어권을 양보
        yield return new WaitForSeconds(0.3f);
        
        float distance = Vector3.Distance(playerTr.position, transform.position);

        // 추적 거리보다 작으면 추적 시작
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

    IEnumerator MonsterAction()
    {
        // 0.3초 동안 대기(정지)하는 동안 메시지 루프로 제어권을 양보
        yield return new WaitForSeconds(0.3f);

        switch (state)
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
        }
    }
    
    
    void OnCollisionEnter(Collision coll)
    {
        // 충돌한 오브젝트 태그가 'Bullet' 이라면
        if (coll.collider.CompareTag("Bullet"))
        {
            anim.SetTrigger("Hit");
            
            Destroy(coll.gameObject);
            
            StartCoroutine(HitStun(1f));
        }
    }

    IEnumerator HitStun(float duration) 
    {
        isStunned = true;
        agent.isStopped = true; 
        
        yield return new WaitForSeconds(duration);
        
        isStunned = false;
    }
    
}
