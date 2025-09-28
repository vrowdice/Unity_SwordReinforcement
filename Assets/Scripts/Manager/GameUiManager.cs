using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUiManager : MonoBehaviour, IUIManager
{
    [Header ("Panels")]
    [SerializeField] private BasePanel[] mainPanels = null;
    [SerializeField] private GameObject quickMovePanel = null;

    [Header("Player Info UI")]
    [SerializeField] private Text goldText = null;
    [SerializeField] private Text bronzeText = null;
    [SerializeField] private Text levelText = null;
    [SerializeField] private Slider expSlider = null;

    private int currentPanelIndex = -1;
    private GameDataManager gameDataManager = null;
    private GameManager gameManager = null;
    private Transform canvasTransform = null;
    private bool isInitialized = false;

    /// <summary>
    /// 캔버스 트랜스폼 프로퍼티
    /// </summary>
    public Transform CanvasTrans 
    { 
        get 
        { 
            if (canvasTransform == null)
            {
                // 캔버스가 설정되지 않은 경우 자동으로 찾기
                var canvas = GetComponentInParent<Canvas>();
                if (canvas != null)
                {
                    canvasTransform = canvas.transform;
                }
                else
                {
                    // 최후의 수단으로 자신의 트랜스폼 반환
                    canvasTransform = transform;
                }
            }
            return canvasTransform; 
        } 
    }

    /// <summary>
    /// 모든 메인 텍스트 업데이트
    /// </summary>
    public void UpdateAllMainText()
    {
        if (!isInitialized || gameDataManager == null)
        {
            return;
        }

        // 플레이어 정보 UI 업데이트
        UpdatePlayerInfoUI();

        // 현재 활성화된 패널의 텍스트도 업데이트
        var currentPanel = GetCurrentPanel();
        if (currentPanel != null)
        {
            // 패널별 텍스트 업데이트 로직
            UpdatePanelSpecificText(currentPanel);
        }

        // 플레이어 수치 패널 업데이트
        UpdatePlayerValuePanel();
    }

    /// <summary>
    /// UI 매니저 초기화
    /// </summary>
    /// <param name="argGameManager">게임 매니저</param>
    /// <param name="argGameDataManager">게임 데이터 매니저</param>
    public void Initialize(GameManager argGameManager, GameDataManager argGameDataManager)
    {
        if (isInitialized)
        {
            Debug.LogWarning("GameUiManager is already initialized!");
            return;
        }

        // 매니저 참조 설정
        gameManager = argGameManager;
        gameDataManager = argGameDataManager;

        if (gameManager == null || gameDataManager == null)
        {
            Debug.LogError("GameManager or GameDataManager is null in GameUiManager.Initialize!");
            return;
        }

        // UI 초기화
        InitializePanels();

        // 기본 패널 활성화 (메인 패널)
        ChangePanel(0);

        // 초기 텍스트 업데이트
        UpdateAllMainText();

        isInitialized = true;
        Debug.Log("GameUiManager initialized successfully!");
    }

    public void Start()
    {
        // GameManager에서 Initialize가 호출되지 않은 경우를 위한 fallback
        if (!isInitialized)
        {
            var gameManager = GameManager.Instance;
            var gameDataManager = GameDataManager.Instance;
            
            if (gameManager != null && gameDataManager != null)
            {
                Initialize(gameManager, gameDataManager);
            }
            else
            {
                Debug.LogWarning("GameManager or GameDataManager not found in Start(), will retry...");
                StartCoroutine(RetryInitialization());
            }
        }
    }

    /// <summary>
    /// 초기화 재시도 코루틴
    /// </summary>
    private IEnumerator RetryInitialization()
    {
        float timeout = 5.0f; // 5초 타임아웃
        float elapsed = 0.0f;

        while (elapsed < timeout && !isInitialized)
        {
            var gameManager = GameManager.Instance;
            var gameDataManager = GameDataManager.Instance;
            
            if (gameManager != null && gameDataManager != null)
            {
                Initialize(gameManager, gameDataManager);
                yield break;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (!isInitialized)
        {
            Debug.LogError("Failed to initialize GameUiManager - GameManager or GameDataManager not found!");
        }
    }

    /// <summary>
    /// 플레이어 정보 UI 업데이트 (직접 참조)
    /// </summary>
    public void UpdatePlayerInfoUI()
    {
        if (gameDataManager == null) return;

        if (goldText != null)
        {
            goldText.text = $"골드 (G) : {gameDataManager.Gold:#,###}";
        }

        if (bronzeText != null)
        {
            bronzeText.text = $"브론즈 (B) : {gameDataManager.Bronze:#,###}";
        }

        if (levelText != null)
        {
            levelText.text = $"LV.{gameDataManager.Level}";
        }

        if (expSlider != null)
        {
            expSlider.maxValue = gameDataManager.MaxExp;
            expSlider.value = gameDataManager.Exp;
        }
    }

    /// <summary>
    /// 패널별 특정 텍스트 업데이트
    /// </summary>
    /// <param name="panel">업데이트할 패널</param>
    private void UpdatePanelSpecificText(BasePanel panel)
    {
        // 각 패널 타입에 따른 특별한 텍스트 업데이트 로직
        if (panel is ChestPanel chestPanel)
        {
            // 상자 패널 특정 업데이트
        }
        else if (panel is StorePanel storePanel)
        {
            // 상점 패널 특정 업데이트
        }
        // 다른 패널들도 필요에 따라 추가
    }

    /// <summary>
    /// 플레이어 수치 패널 업데이트
    /// </summary>
    private void UpdatePlayerValuePanel()
    {
        if (bronzeText != null)
        {
            bronzeText.text = $"{gameDataManager.Bronze:#,###}";
        }

        if (goldText != null)
        {
            goldText.text = $"{gameDataManager.Gold:#,###}";
        }

        if (levelText != null)
        {
            levelText.text = $"Lv. {gameDataManager.Level}";
        }
        
        if (expSlider != null)
        {
            expSlider.maxValue = gameDataManager.MaxExp;
            expSlider.value = gameDataManager.Exp;
        }
    }

    /// <summary>
    /// 패널들 초기화
    /// </summary>
    private void InitializePanels()
    {
        if (mainPanels == null) return;

        for (int i = 0; i < mainPanels.Length; i++)
        {
            if (mainPanels[i] != null)
            {
                // 모든 패널을 비활성화 상태로 초기화
                mainPanels[i].gameObject.SetActive(false);
                mainPanels[i].transform.localScale = Vector3.zero;
            }
        }
    }

    /// <summary>
    /// 다른 패널로 변경
    /// </summary>
    /// <param name="argIndex">패널 인덱스</param>
    public void ChangePanel(int argIndex)
    {
        if (mainPanels == null || argIndex < 0 || argIndex >= mainPanels.Length)
        {
            Debug.LogError($"Invalid panel index: {argIndex}");
            return;
        }

        if (mainPanels[argIndex] == null)
        {
            Debug.LogError($"Panel at index {argIndex} is null");
            return;
        }

        // 현재 패널 닫기
        if (currentPanelIndex >= 0 && currentPanelIndex < mainPanels.Length && 
            mainPanels[currentPanelIndex] != null)
        {
            ClosePanel(currentPanelIndex);
        }

        // 새 패널 열기
        OpenPanel(argIndex);

        currentPanelIndex = argIndex;

        if (quickMovePanel != null)
            quickMovePanel.SetActive(false);

        // 패널 변경 후 텍스트 업데이트
        UpdateAllMainText();
    }

    /// <summary>
    /// 패널 열기
    /// </summary>
    /// <param name="panelIndex">패널 인덱스</param>
    private void OpenPanel(int panelIndex)
    {
        if (panelIndex < 0 || panelIndex >= mainPanels.Length || mainPanels[panelIndex] == null)
            return;

        var panel = mainPanels[panelIndex];
        
        // BasePanel의 OnOpen 메서드 호출
        panel.OnOpen(gameDataManager, this);
        
        // 패널 스케일 및 활성화
        panel.transform.localScale = Vector3.one;
    }

    /// <summary>
    /// 패널 닫기
    /// </summary>
    /// <param name="panelIndex">패널 인덱스</param>
    private void ClosePanel(int panelIndex)
    {
        if (panelIndex < 0 || panelIndex >= mainPanels.Length || mainPanels[panelIndex] == null)
            return;

        var panel = mainPanels[panelIndex];
        
        // BasePanel의 OnClose 메서드 호출
        panel.OnClose();
        
        // 패널 스케일 변경 (완전히 비활성화하지 않고 스케일만 조정)
        panel.transform.localScale = Vector3.zero;
    }

    /// <summary>
    /// 현재 활성화된 패널 가져오기
    /// </summary>
    /// <returns>현재 패널, 없으면 null</returns>
    public BasePanel GetCurrentPanel()
    {
        if (currentPanelIndex >= 0 && currentPanelIndex < mainPanels.Length)
        {
            return mainPanels[currentPanelIndex];
        }
        return null;
    }

    /// <summary>
    /// 특정 패널 가져오기
    /// </summary>
    /// <param name="panelIndex">패널 인덱스</param>
    /// <returns>패널, 없으면 null</returns>
    public BasePanel GetPanel(int panelIndex)
    {
        if (panelIndex >= 0 && panelIndex < mainPanels.Length)
        {
            return mainPanels[panelIndex];
        }
        return null;
    }

    /// <summary>
    /// 패널 개수 가져오기
    /// </summary>
    /// <returns>패널 개수</returns>
    public int GetPanelCount()
    {
        return mainPanels?.Length ?? 0;
    }
}