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
    
    void Start()
    {
        camTr = GetComponent<Transform>();
    }
    
    void LateUpdate()
    {
        // 플레이어의 위치를 기준으로 카메라의 위치 계산
        camTr.position = targetTr.position
                      + (-targetTr.forward * distance)
                      + (Vector3.up * height);
        // Camera를 피벗 좌표를 향해 회전
        camTr.LookAt(targetTr.position + (targetTr.up * targetOffset));
    }
}
