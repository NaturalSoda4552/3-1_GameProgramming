using UnityEngine;

public class RemoveBullet : MonoBehaviour
{
    public GameObject sparkEffect; // spark 효과 가져오기 (public)
    
    // 충돌 처리
    void OnCollisionEnter(Collision coll)
    {
        // 충돌한 오브젝트 태그가 'Bullet' 이면
        if (coll.collider.CompareTag("Bullet"))
        {
            // 충돌 지점을 cp 가져오기
            ContactPoint cp = coll.GetContact(0); 
            // 충돌 지점의 반대 방향으로 rot 설정
            Quaternion rot = Quaternion.LookRotation(-cp.normal);
            // 충돌지점인 cp에서 rot 방향으로 spark 효과 프리팹 생성
            GameObject spark = Instantiate(sparkEffect, cp.point, rot);
            // 0.5초동안 발생
            Destroy(spark, 0.5f);
            // Bullet 오브젝트 삭제 (즉시)
            Destroy(coll.gameObject);
        }
    }
}
