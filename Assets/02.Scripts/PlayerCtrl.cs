using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCtrl : MonoBehaviour
{
    private Transform tr; // 이동 변수
    private Animation anim; // 이동 애니메이션 변수

    public float moveSpeed = 5.0f; // 이동 속도 
    public float turnSpeed = 80.0f; // 회전 속도

    
    void Start()
    {
        tr = GetComponent<Transform>();
        anim = GetComponent<Animation>();
        
        anim.Play("Idle");
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
        if (Input.GetMouseButtonDown(0)) anim.Play("IdleFireSMG");
        
        // Vector3.up 축을 기준으로 turnSpeed만큼의 속도로 회전
        tr.Rotate(Vector3.up * turnSpeed * Time.deltaTime * r);
    }
}
