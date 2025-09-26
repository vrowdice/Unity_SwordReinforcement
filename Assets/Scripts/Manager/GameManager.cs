using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Scripts and Panel")]
    /// <summary>
    /// 도구 매니저
    /// </summary>
    public ToolPanel toolManager = null;

    // (제거됨) 아이템 매니저 레퍼런스는 ToolPanel/StorePanel로 이양됨
    // public ItemPanel itemManager = null;

    /// <summary>
    /// 가방 매니저
    /// </summary>
    public ChestPanel chestManager = null;

    /// <summary>
    /// 상점 매니저
    /// </summary>
    public StorePanel storManager = null;

    /// <summary>
    /// UI 매니저 (캔버스에서 자동으로 찾음)
    /// </summary>
    private GameUiManager uiManager = null;

    // (제거됨) Exchange 패널 관리는 GameUiManager로 이양됨
    // public GameObject exchangePanel = null;

    [Header("Warning Panel")]
    /// <summary>
    /// 경고 패널 (다른 씬에서도 사용되므로 GameManager에 유지)
    /// </summary>
    public GameObject warningPanel = null;
    
    /// <summary>
    /// 게임 매니저
    /// </summary>
    static GameManager g_GameManager = null;

    public void OnEnable()
    {
        g_GameManager = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // GameDataManager와 GameUiManager 초기화
        StartCoroutine(InitializeManagers());
    }
    
    /// <summary>
    /// 매니저들 초기화
    /// </summary>
    private IEnumerator InitializeManagers()
    {
        // GameDataManager가 초기화될 때까지 대기
        while (GameDataManager.Instance == null)
        {
            yield return null;
        }
        
        // UI 매니저 찾기
        FindUIManager();
        
        // GameUiManager 초기화
        if (uiManager != null)
        {
            uiManager.Initialize(this, GameDataManager.Instance);
        }
        else
        {
            Debug.LogError("GameUiManager not found in canvas!");
        }
        
        Debug.Log("GameManager initialization completed!");
    }
    
    /// <summary>
    /// UI 매니저 찾기 (캔버스에서)
    /// </summary>
    private void FindUIManager()
    {
        // 씬의 모든 Canvas에서 GameUiManager 찾기
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        
        foreach (Canvas canvas in canvases)
        {
            GameUiManager foundUiManager = canvas.GetComponentInChildren<GameUiManager>();
            if (foundUiManager != null)
            {
                uiManager = foundUiManager;
                Debug.Log($"GameUiManager found in canvas: {canvas.name}");
                return;
            }
        }
        
        // Canvas에서 찾지 못한 경우 씬 전체에서 찾기
        uiManager = FindObjectOfType<GameUiManager>();
        if (uiManager != null)
        {
            Debug.Log("GameUiManager found in scene (not in canvas)");
        }
        else
        {
            Debug.LogWarning("GameUiManager not found in scene!");
        }
    }

    /// <summary>
    /// UI 매니저 프로퍼티 (외부 접근용)
    /// </summary>
    public GameUiManager UiManager => uiManager;

    /// <summary>
    /// 경고 패널 띄우기
    /// </summary>
    /// <param name="argText">띄울 메세지</param>
    /// <param name="argType">적용할 타입</param>
    public void Warning(string argText)
    {
        warningPanel.SetActive(true);
        warningPanel.transform.GetChild(0).Find("Text").gameObject.GetComponent<Text>().text = string.Format(argText);
    }

    /// <summary>
    /// 돈이 없을 시
    /// </summary>
    public void NoMoney()
    {
        Warning("돈이 없습니다");
    }

    /// <summary>
    /// 확인 버튼 클릭 시
    /// </summary>
    public void Confirm()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 나가기 버튼 클릭 시
    /// </summary>
    public void Exit()
    {
        gameObject.SetActive(false);
    }
    
    // (제거됨) Exchange 관련 메서드들은 GameUiManager로 이양됨
    // BuySetSlider, SellSetSlider, UpdateSlider, ClickConfirm

    /// <summary>
    /// 인스턴스
    /// </summary>
    public static GameManager Instance
    {
        get
        {
            return g_GameManager;
        }
    }
}
