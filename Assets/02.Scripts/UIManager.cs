using UnityEngine;
using UnityEngine.UI;       // Unity-UI를 사용하기 위해 선언한 네임스페이스
using UnityEngine.Events;   // UnityEvent 관련 API를 사용하기 위해 선언한 네임스페이스

public class UIManager : MonoBehaviour
{
    // 버튼을 연결할 변수
    public Button startButton;
    public Button optionButton;
    public Button shopButton;

    private UnityAction action;

    void Start()
    {
        
    }

    public void OnButtonClick()
    {
        Debug.Log($"Click Button!");
    }
}