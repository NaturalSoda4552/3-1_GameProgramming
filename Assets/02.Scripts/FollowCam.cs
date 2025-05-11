using Unity.VisualScripting;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    private Transform camTr; // 카메라 tr -> private : 자기 자신 tr 컴포넌트
    public Transform targetTr; // 플레이어 tr
    
    [Range(2.0f, 20.0f)] public float distance = 10.0f; // z축 기준 거리
    [Range(0.0f, 10.0f)] public float height = 2.0f; // y축 기준 거리
    [Range(0.0f, 5.0f)] public float targetOffset = 2.0f; // 위에서 바라보는 각도 오프셋
    
    public Vector3 velocity = Vector3.zero; // SmoothDamp에서 사용할 변수
    [Range(0.0f, 2.0f)] public float damping = 1.0f;// 반응 속도
    
    void Start()
    {
        camTr = GetComponent<Transform>(); // 본인(메인 카메라) 위치 가져오기
    }
    
    void LateUpdate()
    {
        // 플레이어 tr 기준, 카메라 위치 계산
        Vector3 pos = targetTr.position
                      + (-targetTr.forward * distance)
                      + (Vector3.up * height);
        
        // 부드럽게 감속
        camTr.position = Vector3.SmoothDamp(
            camTr.position, // 시작 위치
            pos,            // 목표 위치
            ref velocity,   // 현재 속도
            damping);       // 목표 위치까지 도달할 시간 
        
        // 플레이어 머리 방향을 바라보게 설정
        camTr.LookAt(targetTr.position + (targetTr.up * targetOffset));
    }
}
