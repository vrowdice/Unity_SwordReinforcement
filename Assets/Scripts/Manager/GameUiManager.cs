using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUiManager : MonoBehaviour, IUIManager
{
    [Header("Canvas")]
    /// <summary>
    /// 캔버스 트랜스폼 (IUIManager 인터페이스 구현용)
    /// </summary>
    [SerializeField] private Transform canvasTransform = null;

    [Header ("Panels")]
    /// <summary>
    /// 각 패널들 (BasePanel 기반)
    /// </summary>
    [SerializeField] private BasePanel[] mainPanels = null;

    /// <summary>
    /// 플래이어 수치 패널
    /// </summary>
    [SerializeField] private GameObject playInfoPanel = null;

    /// <summary>
    /// 빠른 이동 패널
    /// </summary>
    [SerializeField] private GameObject quickMovePanel = null;

    [Header("Chest")]
    /// <summary>
    /// 변경 스크롤뷰
    /// </summary>
    [SerializeField] private GameObject chestScrollView = null;

    /// <summary>
    /// 상자의 아이템 종류 선택 드롭다운
    /// </summary>
    [SerializeField] private Dropdown chestDropdown = null;

    [Header("Stor")]
    /// <summary>
    /// 상점의 아이탬 선택 스크롤뷰
    /// </summary>
    [SerializeField] private GameObject[] storScrollView = null;

    [Header("UI Text Elements")]
    /// <summary>
    /// 메인 텍스트들 (자동 업데이트용)
    /// </summary>
    [SerializeField] private Text[] mainTexts = null;

    [Header("Player Info UI")]
    /// <summary>
    /// 골드(캐쉬) 텍스트
    /// </summary>
    public Text goldText = null;

    /// <summary>
    /// 브론즈(게임머니) 텍스트
    /// </summary>
    public Text bronzeText = null;

    /// <summary>
    /// 레벨 텍스트
    /// </summary>
    public Text levelText = null;

    /// <summary>
    /// 경험치 슬라이더
    /// </summary>
    public Slider expSlider = null;

    [Header("Exchange Panel")]
    /// <summary>
    /// 갯수 설정 패널
    /// </summary>
    public GameObject exchangePanel = null;

    /// <summary>
    /// 자신의 정보 타입
    /// </summary>
    public int exchangeType = 0;

    /// <summary>
    /// 자신의 아이템 코드
    /// </summary>
    public int exchangeCode = 0;

    /// <summary>
    /// 구매 판매 플래그 (Buy = true, Sell = false)
    /// </summary>
    public bool exchangeFlag = true;

    /// <summary>
    /// 현재 활성화된 패널 인덱스
    /// </summary>
    private int currentPanelIndex = -1;

    /// <summary>
    /// 게임 데이터 매니저 캐시
    /// </summary>
    private GameDataManager gameDataManager = null;

    /// <summary>
    /// 게임 매니저 캐시
    /// </summary>
    private GameManager gameManager = null;

    /// <summary>
    /// 초기화 완료 여부
    /// </summary>
    private bool isInitialized = false;
    
    /// <summary>
    /// 이벤트 구독 상태
    /// </summary>
    private bool isEventSubscribed = false;

    /// <summary>
    /// Exchange 패널 UI 요소들
    /// </summary>
    private Text itemSetAmountT = null;
    private Text itemSetAmountPriceT = null;
    private Slider itemAmountSlider = null;

    // (제거됨) 씬마다 다른 GameUiManager가 있으므로 싱글톤 패턴 사용하지 않음

    #region IUIManager 인터페이스 구현

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

        // 메인 텍스트들 업데이트
        UpdateMainTextElements();

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
        InitializeScrollViews();
        InitializeExchangePanel();
        
        if (playInfoPanel != null)
            playInfoPanel.SetActive(true);

        // 기본 패널 활성화 (메인 패널)
        ChangePanel(0);

        // 이벤트 구독
        SubscribeToDataEvents();
        
        // 초기 텍스트 업데이트
        UpdateAllMainText();

        isInitialized = true;
        Debug.Log("GameUiManager initialized successfully!");
    }

    #endregion

    #region Event Management
    
    /// <summary>
    /// 데이터 이벤트 구독
    /// </summary>
    private void SubscribeToDataEvents()
    {
        if (isEventSubscribed) return;
        
        // GameDataManager 이벤트 구독
        GameDataManager.OnGoldChanged += OnGoldChanged;
        GameDataManager.OnBronzeChanged += OnBronzeChanged;
        GameDataManager.OnLevelChanged += OnLevelChanged;
        GameDataManager.OnExpChanged += OnExpChanged;
        
        isEventSubscribed = true;
        Debug.Log("GameUiManager subscribed to data events");
    }
    
    /// <summary>
    /// 데이터 이벤트 구독 해제
    /// </summary>
    private void UnsubscribeFromDataEvents()
    {
        if (!isEventSubscribed) return;
        
        // GameDataManager 이벤트 구독 해제
        GameDataManager.OnGoldChanged -= OnGoldChanged;
        GameDataManager.OnBronzeChanged -= OnBronzeChanged;
        GameDataManager.OnLevelChanged -= OnLevelChanged;
        GameDataManager.OnExpChanged -= OnExpChanged;
        
        isEventSubscribed = false;
        Debug.Log("GameUiManager unsubscribed from data events");
    }
    
    /// <summary>
    /// 골드 변경 이벤트 핸들러
    /// </summary>
    /// <param name="newGoldAmount">새로운 골드 양</param>
    private void OnGoldChanged(long newGoldAmount)
    {
        if (goldText != null)
        {
            goldText.text = $"골드 (G) : {newGoldAmount:#,###}";
        }
        
        // 메인 텍스트들도 업데이트
        UpdateGoldInMainTexts(newGoldAmount);
    }
    
    /// <summary>
    /// 브론즈 변경 이벤트 핸들러
    /// </summary>
    /// <param name="newBronzeAmount">새로운 브론즈 양</param>
    private void OnBronzeChanged(long newBronzeAmount)
    {
        if (bronzeText != null)
        {
            bronzeText.text = $"브론즈 (B) : {newBronzeAmount:#,###}";
        }
        
        // 메인 텍스트들도 업데이트
        UpdateBronzeInMainTexts(newBronzeAmount);
    }
    
    /// <summary>
    /// 레벨 변경 이벤트 핸들러
    /// </summary>
    /// <param name="newLevel">새로운 레벨</param>
    private void OnLevelChanged(int newLevel)
    {
        if (levelText != null)
        {
            levelText.text = $"LV.{newLevel}";
        }
        
        // 메인 텍스트들도 업데이트
        UpdateLevelInMainTexts(newLevel);
    }
    
    /// <summary>
    /// 경험치 변경 이벤트 핸들러
    /// </summary>
    /// <param name="currentExp">현재 경험치</param>
    /// <param name="maxExp">최대 경험치</param>
    private void OnExpChanged(long currentExp, long maxExp)
    {
        if (expSlider != null)
        {
            expSlider.maxValue = maxExp;
            expSlider.value = currentExp;
        }
        
        // 메인 텍스트들도 업데이트
        UpdateExpInMainTexts(currentExp, maxExp);
    }
    
    #endregion

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
    /// 메인 텍스트 요소들 업데이트
    /// </summary>
    private void UpdateMainTextElements()
    {
        if (mainTexts == null || gameDataManager == null) return;

        // 예시: 각 텍스트별로 업데이트 로직 구현
        for (int i = 0; i < mainTexts.Length; i++)
        {
            if (mainTexts[i] != null)
            {
                // 텍스트 이름이나 태그에 따라 다른 정보 표시
                UpdateTextByName(mainTexts[i]);
            }
        }
    }

    /// <summary>
    /// 텍스트 이름에 따른 업데이트
    /// </summary>
    /// <param name="textComponent">텍스트 컴포넌트</param>
    private void UpdateTextByName(Text textComponent)
    {
        string textName = textComponent.name.ToLower();
        
        if (textName.Contains("bronze") || textName.Contains("money"))
        {
            textComponent.text = $"{gameDataManager.Bronze:#,###}";
        }
        else if (textName.Contains("gold"))
        {
            textComponent.text = $"{gameDataManager.Gold:#,###}";
        }
        else if (textName.Contains("level") || textName.Contains("lv"))
        {
            textComponent.text = $"Lv. {gameDataManager.Level}";
        }
        else if (textName.Contains("exp"))
        {
            textComponent.text = $"{gameDataManager.Exp:#,###}";
        }
        // 필요에 따라 더 많은 텍스트 타입 추가 가능
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
    /// 메인 텍스트들에서 골드 관련 텍스트 업데이트
    /// </summary>
    /// <param name="goldAmount">골드 양</param>
    private void UpdateGoldInMainTexts(long goldAmount)
    {
        if (mainTexts == null) return;
        
        foreach (var text in mainTexts)
        {
            if (text != null && text.name.ToLower().Contains("gold"))
            {
                text.text = $"{goldAmount:#,###}";
            }
        }
    }
    
    /// <summary>
    /// 메인 텍스트들에서 브론즈 관련 텍스트 업데이트
    /// </summary>
    /// <param name="bronzeAmount">브론즈 양</param>
    private void UpdateBronzeInMainTexts(long bronzeAmount)
    {
        if (mainTexts == null) return;
        
        foreach (var text in mainTexts)
        {
            if (text != null && (text.name.ToLower().Contains("bronze") || text.name.ToLower().Contains("money")))
            {
                text.text = $"{bronzeAmount:#,###}";
            }
        }
    }
    
    /// <summary>
    /// 메인 텍스트들에서 레벨 관련 텍스트 업데이트
    /// </summary>
    /// <param name="level">레벨</param>
    private void UpdateLevelInMainTexts(int level)
    {
        if (mainTexts == null) return;
        
        foreach (var text in mainTexts)
        {
            if (text != null && (text.name.ToLower().Contains("level") || text.name.ToLower().Contains("lv")))
            {
                text.text = $"Lv. {level}";
            }
        }
    }
    
    /// <summary>
    /// 메인 텍스트들에서 경험치 관련 텍스트 업데이트
    /// </summary>
    /// <param name="currentExp">현재 경험치</param>
    /// <param name="maxExp">최대 경험치</param>
    private void UpdateExpInMainTexts(long currentExp, long maxExp)
    {
        if (mainTexts == null) return;
        
        foreach (var text in mainTexts)
        {
            if (text != null && text.name.ToLower().Contains("exp"))
            {
                text.text = $"{currentExp:#,###} / {maxExp:#,###}";
            }
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
        if (playInfoPanel == null || gameDataManager == null) return;

        // 플레이어 수치 패널 내의 텍스트들 업데이트
        var texts = playInfoPanel.GetComponentsInChildren<Text>();
        foreach (var text in texts)
        {
            UpdateTextByName(text);
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
    /// 스크롤뷰들 초기화
    /// </summary>
    private void InitializeScrollViews()
    {
        if (storScrollView == null) return;

        for (int i = 0; i < storScrollView.Length; i++)
        {
            if (storScrollView[i] != null)
            {
                storScrollView[i].SetActive(true);
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
        
        // 특별한 패널별 처리
        HandleSpecialPanelLogic(argIndex);

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
    /// 특별한 패널별 로직 처리
    /// </summary>
    /// <param name="panelIndex">패널 인덱스</param>
    private void HandleSpecialPanelLogic(int panelIndex)
    {
        // 상자 패널인 경우 (인덱스 4)
        if (panelIndex == 4)
        {
            if (gameManager?.chestManager != null)
            {
                gameManager.chestManager.NowImfoUpdate();
                if (gameManager.chestManager.scrollbar != null)
                {
                    gameManager.chestManager.scrollbar.value = 1.0f;
                }
            }
        }
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

    #region Exchange Panel Management

    /// <summary>
    /// Exchange 패널 초기화
    /// </summary>
    private void InitializeExchangePanel()
    {
        if (exchangePanel == null) return;

        itemSetAmountT = exchangePanel.transform.GetChild(0).Find("AmountText")?.GetComponent<Text>();
        itemSetAmountPriceT = exchangePanel.transform.GetChild(0).Find("PriceText")?.GetComponent<Text>();
        itemAmountSlider = exchangePanel.transform.GetChild(0).Find("Slider")?.GetComponent<Slider>();
    }

    /// <summary>
    /// 구매 시 슬라이더 셋팅
    /// </summary>
    public void BuySetSlider(int argItemCode)
    {
        exchangeCode = argItemCode;

        if (argItemCode <= -1)
        {
            GameManager.Instance?.Warning("아이템이 선택되지 않았습니다.");
            return;
        }

        if (exchangePanel != null)
        {
            exchangePanel.SetActive(true);
        }
        
        if (itemAmountSlider != null)
        {
            itemAmountSlider.value = 1;
            itemAmountSlider.maxValue = 20;
        }
        
        exchangeFlag = true;
        UpdateSlider();
    }

    /// <summary>
    /// 판매 시 슬라이더 셋팅
    /// </summary>
    public void SellSetSlider(int argItemCode)
    {
        exchangeCode = argItemCode;
        var itemData = GameDataManager.Instance?.GetItemData(argItemCode);
        if (itemData == null || itemData.amount <= 0)
        {
            GameManager.Instance?.Warning("아이템이 없습니다.");
            return;
        }

        if (argItemCode <= -1)
        {
            GameManager.Instance?.Warning("아이템이 선택되지 않았습니다.");
            return;
        }

        if (exchangePanel != null)
        {
            exchangePanel.SetActive(true);
        }
        
        if (itemAmountSlider != null)
        {
            itemAmountSlider.value = 1;
            itemAmountSlider.maxValue = 20;
            if (itemData.amount <= itemAmountSlider.maxValue)
            {
                itemAmountSlider.maxValue = itemData.amount;
            }
        }
        
        exchangeFlag = false;
        UpdateSlider();
    }

    /// <summary>
    /// 슬라이더 변경 시
    /// </summary>
    public void UpdateSlider()
    {
        if (itemAmountSlider == null || itemSetAmountT == null || itemSetAmountPriceT == null) return;

        if (exchangeFlag == true)
        {
            if (itemAmountSlider.value <= 0)
            {
                itemAmountSlider.value = 1;
            }
            itemSetAmountT.text = string.Format("{0} / {1}", itemAmountSlider.value, itemAmountSlider.maxValue);
            var itemData = GameDataManager.Instance?.GetItemMasterData(exchangeCode);
            if (itemData != null)
            {
                itemSetAmountPriceT.text = string.Format("{0}$", itemAmountSlider.value * itemData.price);
            }
        }
        else
        {
            if (itemAmountSlider.value <= 0)
            {
                itemAmountSlider.value = 1;
            }
            var userItemData = GameDataManager.Instance?.GetItemData(exchangeCode);
            var itemMasterData = GameDataManager.Instance?.GetItemMasterData(exchangeCode);
            
            if (userItemData != null && itemMasterData != null)
            {
                if (userItemData.amount <= itemAmountSlider.maxValue)
                {
                    itemSetAmountT.text = string.Format("{0} / {1}", itemAmountSlider.value, userItemData.amount);
                }
                else
                {
                    itemSetAmountT.text = string.Format("{0} / {1}", itemAmountSlider.value, itemAmountSlider.maxValue);
                }
                itemSetAmountPriceT.text = string.Format("{0}$", (int)(itemAmountSlider.value * itemMasterData.price * 0.7f));
            }
        }
    }

    /// <summary>
    /// 확인 클릭 시
    /// </summary>
    public void ClickConfirm()
    {
        var gameManager = GameManager.Instance;
        if (gameManager?.storManager == null || itemAmountSlider == null) return;

        if (exchangeFlag == true)
        {
            gameManager.storManager.BuyItem(exchangeCode, (int)itemAmountSlider.value);
        }
        else
        {
            gameManager.storManager.SellItem(exchangeCode, (int)itemAmountSlider.value);
        }
        
        if (exchangePanel != null)
        {
            exchangePanel.SetActive(false);
        }
    }

    #endregion
    
    /// <summary>
    /// 오브젝트 파괴 시 이벤트 구독 해제
    /// </summary>
    private void OnDestroy()
    {
        UnsubscribeFromDataEvents();
    }
}
