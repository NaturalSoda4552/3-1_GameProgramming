using UnityEngine;

public class BarrelCtrl : MonoBehaviour
{
    // 폭팔 반경
    public float radius = 5.0f;
    // 본인 Rigidbody rb 변수
    private Rigidbody rb;

    private Transform barreltr;
    
    // 드럼통 텍스쳐 배열
    public Texture[] textures;
    // MeshRenderer 변수 renderer
    private new MeshRenderer renderer;
    // 텍스쳐 인덱스 0으로 초기화
    private int textureIndex = 0;
    
    void Start()
    {
        // 드럼통 MeshRenderer 컴포넌트 가져오기
        renderer = GetComponentInChildren<MeshRenderer>();
        // 텍스쳐 인덱스 0, 1, 2 중 랜덤 배정
        textureIndex = Random.Range(0, textures.Length);
        // 현재 드럼통의 텍스쳐 택스쳐 배열에서 인덱스로 접근하여 텍스쳐 변경
        renderer.material.mainTexture = textures[textureIndex];
        
        // 드럼통 본인 위치
        barreltr = GetComponent<Transform>();
    }
    
    // 주변 드럼통에 폭팔 힘을 가하는 함수
    void IndirectDamage(Vector3 pos)
    {
        // radius 반경 내에 있는 3번 레이어(Barrel) 탐색하여 배열 콜라이더 저장
        Collider[] colls = Physics.OverlapSphere(pos, radius, 1 << 3);

        // 배열 탐색
        foreach (var coll in colls)
        {
            // coll Rigidbody 컴포넌트 가져오기
            rb = coll.GetComponent<Rigidbody>();
            // 무게 1로 변경
            rb.mass = 1.0f;
            // 움직임 제한 해제
            rb.constraints = RigidbodyConstraints.None;
            // 폭팔 힘 1500만큼 주기
            rb.AddExplosionForce(1500.0f, pos, radius, 1200.0f);
        }
    }
    
    public GameObject BoomEffect; // 폭팔 이펙트
    int cnt = 0; // 드럼통에 총알 박힌 횟수, 최초 0으로 초기화
    // 오브젝트 충돌 함수
    void OnCollisionEnter(Collision coll)
    {
        // 충돌한 오브젝트 태그가 'Bullet'이라면
        if (coll.collider.CompareTag("Bullet"))
        {
            cnt++; // 충돌횟수 cnt + 1
            // 3번 충돌 시
            if (cnt == 3)
            {
                // 드럼통 위치에서 폭팔 효과 프리팹 생성
                GameObject boom = Instantiate(BoomEffect, transform.position, Quaternion.identity);
                // 폭팔 이펙트 4초 후 제거
                Destroy(boom, 4.0f);
                // 드럼통 3초 뒤 제거
                Destroy(gameObject, 3.0f);
                // 주변 폭팔 영향 함수 실행 (인자는 폭팔한 드럼통 위치)
                IndirectDamage(transform.position);
                // cnt 초기화
                cnt = 0;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
