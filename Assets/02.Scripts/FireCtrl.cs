using System.Collections;
using UnityEngine;

// 플레이어 오브젝트는 AudioSource를 반드시 가지고 있어야 함
[RequireComponent(typeof(AudioSource))]

public class FireCtrl : MonoBehaviour
{
    public GameObject bullet; // 총알 프리팹
    public Transform firePos; // 총구 위치 tr
    
    private MeshRenderer muzzleFlash; // 섬광 효과 MeshRenderer
    
    public AudioClip fireSfx; // 총알 발사 사운드 클립
    private new AudioSource audio; // 소리를 재생할 AudioSource 변수
    
    void Start()
    {
        // 총구 아래 자식 오브젝트 중 MeshRenderer를 참조
        muzzleFlash = firePos.GetComponentInChildren<MeshRenderer>();
        // 시작 시 화염 비활성화
        muzzleFlash.enabled = false;
        // AudioSource 컴포넌트 가져오기
        audio = GetComponent<AudioSource>();
    }
    
    // Update is called once per frame
    void Update()
    {
        // 좌클릭 시 Fire() 호출
        if (Input.GetMouseButtonDown(0)) Fire();
    }
    
    void Fire()
    {
        // Bullet 프리팹을 동적으로 생성(프리팹, 위치, 회전)
        Instantiate(bullet, firePos.position, firePos.rotation);
        // 코루틴으로 ShowMuzzleFlash() 함수 호출
        StartCoroutine(ShowMuzzleFlash());
        // 발사 사운드 출력 (볼륨 1.0)
        audio.PlayOneShot(fireSfx, 1.0f);
    }
    
    // 총구 화염 효과를 출력하는 코루틴 함수
    IEnumerator ShowMuzzleFlash()
    {
        // 오프셋 좌푯값 랜덤 생성
        Vector2 offset = new Vector2(Random.Range(0, 2), Random.Range(0, 2)) * 0.5f;
        // 텍스처의 오프셋 값 설정 (4개 중 1개로)
        muzzleFlash.material.mainTextureOffset = offset;

        // MuzzleFlash의 회전 변경
        float angle = Random.Range(0, 360);
        muzzleFlash.transform.localRotation = Quaternion.Euler(0, 0, angle);

        // MuzzleFlash의 크기 조절
        float scale = Random.Range(0.4f, 0.8f);
        muzzleFlash.transform.localScale = Vector3.one * scale;

        // MuzzleFlash 활성화
        muzzleFlash.enabled = true;

        // 0.2초 동안 대기(정지)하는 동안 메시지 루프로 제어권을 양보
        yield return new WaitForSeconds(0.2f);

        // 화염 효과 비활성화
        muzzleFlash.enabled = false;
    }
}
