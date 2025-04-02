using UnityEngine;

public class FollowCam : MonoBehaviour
{
    private Transform camTr; // 카메라 tr -> private : 자기 자신 tr 컴포넌트
    public Transform targetTr; // 플레이어 tr
    
    [Range(2.0f, 20.0f)] 
    public float distance = 10.0f;
    
    [Range(0.0f, 10.0f)] 
    public float height = 2.0f;
    
    public float targetOffset = 2.0f;
    
    // SmoothDamp에서 사용할 변수
    public Vector3 velocity = Vector3.zero; 
    // velocity = new Vector3.zero;
    // 반응 속도
    public float damping = 1.0f;
    
    void Start()
    {
        camTr = GetComponent<Transform>();
    }
    
    void LateUpdate()
    {
        // 플레이어의 위치를 기준으로 카메라의 위치 계산
        Vector3 pos = targetTr.position
                      + (-targetTr.forward * distance)
                      + (Vector3.up * height);
        
        camTr.position = Vector3.SmoothDamp(camTr.position, // 시작 위치
            pos,            // 목표 위치
            ref velocity,   // 현재 속도
            damping);       // 목표 위치까지 도달할 시간
        // Camera를 피벗 좌표를 향해 회전
        camTr.LookAt(targetTr.position + (targetTr.up * targetOffset));
    }
}
