using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class FireCtrl : MonoBehaviour
{
    public GameObject bullet;
    public Transform firePos;
    
    private MeshRenderer muzzleFlash;
    
    public AudioClip fireSfx;
    private new AudioSource audio;
    
    void Start()
    {
        muzzleFlash = firePos.GetComponentInChildren<MeshRenderer>();
        muzzleFlash.enabled = false;
        
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
        // Bullet 프리팹을 동적으로 생성(생성할 객체, 위치, 회전)
        Instantiate(bullet, firePos.position, firePos.rotation);
        // 총구 화염 show(코루틴)
        StartCoroutine(ShowMuzzleFlash());
        // 발사 사운드 출력
        audio.PlayOneShot(fireSfx, 1.0f);
    }
    
    IEnumerator ShowMuzzleFlash()
    {
        // 오프셋 좌푯값을 랜덤 함수로 생성
        Vector2 offset = new Vector2(Random.Range(0, 2), Random.Range(0, 2)) * 0.5f;
        // 텍스처의 오프셋 값 설정
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

        // MuzzleFlash 비활성화
        muzzleFlash.enabled = false;
    }
}
