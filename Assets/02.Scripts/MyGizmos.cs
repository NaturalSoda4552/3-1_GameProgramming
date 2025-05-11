using UnityEngine;

public class MyGizmos : MonoBehaviour
{
    public Color _color = Color.yellow;
    public float _radius = 0.1f;

    void OnDrawGizmos()
    {
        // 색상 설정
        Gizmos.color = _color;
        // 구체 모양의 기즈모 생성
        Gizmos.DrawSphere(transform.position, _radius);
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
