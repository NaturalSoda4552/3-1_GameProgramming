using UnityEngine;
using UnityEngine.UI;      
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{
    // 버튼을 연결할 변수
    public Button startButton;
    public Button optionButton;
    public Button shopButton;
    
    private UnityAction action;

    private GameObject panel;
    private GameObject HpScorePanel;

    void Start()
    {
        panel = GameObject.FindGameObjectWithTag("Panel");
        HpScorePanel = GameObject.FindGameObjectWithTag("HpAndScorePanel");
        
        // UnityAction을 사용한 이벤트 연결 방식
        action = () => OnButtonClick(startButton.name);
        startButton.onClick.AddListener(action);
        
        optionButton.onClick.AddListener(
            delegate {OnButtonClick(optionButton.name);});
        
        shopButton.onClick.AddListener(()=> OnButtonClick(shopButton.name));
    }

    public void OnButtonClick(string msg)
    {
        // 체력/점수 패널 활성화
        HpScorePanel.gameObject.SetActive(true);
        
        // Debug.Log($"Click Button : {msg}");
        panel.gameObject.SetActive(false);
        Time.timeScale = 1.0f;
    }
    
    void SaveScore(int score)
    {
        PlayerPrefs.SetInt("HighScore", score);
        PlayerPrefs.Save(); // 저장 즉시 반영 (꼭 쓰지 않아도 되지만 명시적 사용 권장)
    }
}