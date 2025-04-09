using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCtrl : MonoBehaviour
{
    private Transform tr; // 이동 변수
    private Animation anim; // 이동 애니메이션 변수

    public float moveSpeed = 5.0f; // 이동 속도 
    public float turnSpeed = 80.0f; // 회전 속도

    private readonly float initHp = 100.0f;
    public float currHp;
    
    public delegate void PlayerDieHandler();
    public static event PlayerDieHandler OnPlayerDie;
    
    void Start()
    {
        tr = GetComponent<Transform>();
        anim = GetComponent<Animation>();
        
        anim.Play("Idle");
        
        currHp = initHp;
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float r = Input.GetAxis("Mouse X");
        
        // 전후좌우 이동 방향 벡터 계산
        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);
        // Translate(이동 방향 * 속력 * Time.deltaTime)
        tr.Translate(moveDir.normalized * moveSpeed * Time.deltaTime);
        
        // 이동 애니메이션
        if (h == 0 && v == 0) anim.CrossFade("Idle", 0.25f);
        if (v > 0) anim.CrossFade("RunF", 0.25f);
        if (v < 0) anim.CrossFade("RunB", 0.25f);
        if (h > 0) anim.CrossFade("RunR", 0.25f);
        if (h < 0) anim.CrossFade("RunL", 0.25f);
        if (Input.GetMouseButtonDown(0) && (h != 0 || v != 0))
        {
            anim.CrossFade("RunFireSMG", 0.25f); // ?
        }
        else if (Input.GetMouseButtonDown(0) && (h == 0 && v == 0))
        {
            anim.Play("IdleFireSMG");
        }
        
        // Vector3.up 축을 기준으로 turnSpeed만큼의 속도로 회전
        tr.Rotate(Vector3.up * turnSpeed * Time.deltaTime * r);

        // 플레이어 사망 처리
        if (currHp <= 0)
        {
            PlayerDie(); // 몬스터에게 알림
            OnPlayerDie(); // 이벤트 방식 병행 처리
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Punch"))
        {
            currHp -= 10;
            Debug.Log(currHp);
        }
    }
    void PlayerDie(){
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        foreach (GameObject monster in monsters) {
            monster.SendMessage("OnPlayerDie", SendMessageOptions.DontRequireReceiver);
        }
    }
}
