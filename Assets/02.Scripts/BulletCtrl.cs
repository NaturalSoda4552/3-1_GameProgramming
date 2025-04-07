using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    public float damage = 20.0f; // 총알 데미지
    public float force = 1500.0f; // 총알의 힘 = 총알 오브젝트의 속도

    private Rigidbody rb; // 총알 Prefab의 Ridigbody 변수 rb
    
    void Start()
    {
        rb = GetComponent<Rigidbody>(); // 본인 Rigidbody 컴포넌트 가져오기

        rb.AddForce(transform.forward * force); // 앞방향으로 force만큼 힘 더하기
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
