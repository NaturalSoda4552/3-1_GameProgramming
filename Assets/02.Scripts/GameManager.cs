using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<Transform> points = new List<Transform>(); // 스폰포인트 집합
    
    public GameObject monster; // 몬스터 오브젝트 
    public float repeatTime = 3.0f; // 
    
    private bool isGameOver;
    public bool IsGameOver
    {
        get { return isGameOver; }
        set { isGameOver = value; }
    }
    
    public static GameManager instance = null; // 싱글턴 인스턴스 멤버변수 선언
    
    public List<GameObject> monsterPool = new List<GameObject>();
    public int maxMonster = 10;
    
    public int totScore = 0;
    public TMP_Text scoreText;
    
    public TMP_Text highScoreText;
    private int highScore = 0;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Transform spawnPointGroup = GameObject.Find("SpawnPointGroup")?.transform;
        foreach (Transform point in spawnPointGroup)
        {
            points.Add(point);
        }
        
        CreateMonsterPool(); // 1회만 실행 - 생성 가능한 크기만큼 공간을 만드는 개념
        // 몬스터 생성 함수를 repeatTime 간격으로 호출
        InvokeRepeating("CreateMonster", 2.0f, repeatTime);
    }
    
    public void DisplayScore(int score){
        totScore += score;
        scoreText.text = $"<color=#00ff00>SCORE :</color> <color=#ff0000>{totScore:#,##0}</color>";
    }

    void CreateMonster()
    {
        // 게임 오버 상태면 몬스터 생성 중지
        if (IsGameOver) return;
        
        // 랜덤 위치에 몬스터 생성
        int idx = Random.Range(0, points.Count);
        // Instantiate(monster, points[idx].position, points[idx].rotation);

        GameObject _monster = GetMonsterInPool();
        _monster?.transform.SetPositionAndRotation(points[idx].position,
            points[idx].rotation);
        _monster?.SetActive(true);
        
    }
    
    void Awake() {
        // 게임 시작 버튼을 누를 시 시작
        Time.timeScale = 0.0f;
        
        if (instance == null){
            instance = this;
        }
        else if (instance != this){
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }
    
    void CreateMonsterPool()
    {
        for (int i = 0; i < maxMonster; i++)
        {
            GameObject _monster = Instantiate<GameObject>(monster);
            _monster.name = "Monster_" + i;
            _monster.SetActive(false);

            // 생성한 몬스터를 오브젝트 풀에 추가
            monsterPool.Add(_monster);
        }
    }
    
    // 오브젝트 풀에서 사용 가능한 몬스터를 반환
    public GameObject GetMonsterInPool()
    {
        // 오브젝트 풀의 처음부터 끝까지 순회
        foreach (var _monster in monsterPool)
        {
            // 비활성화 된 몬스터가 있다면 
            if (_monster.activeSelf == false)
            {
                return _monster; // 몬스터 반환
            }
        }
        return null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
