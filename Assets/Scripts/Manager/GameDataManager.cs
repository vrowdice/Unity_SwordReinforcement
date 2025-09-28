using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// 유저 툴 데이터
/// </summary>
[Serializable]
public class UserToolData
{
    /// <summary>
    /// 툴 코드
    /// </summary>
    public int code = 0;

    /// <summary>
    /// 툴 갯수
    /// </summary>
    public int amount = 0;

    /// <summary>
    /// 강화 횟수
    /// </summary>
    public int reinforcement = 0;
}

/// <summary>
/// 유저 아이템 데이터
/// </summary>
[Serializable]
public class UserItemData
{
    /// <summary>
    /// 아이템 코드
    /// </summary>
    public int code = 0;

    /// <summary>
    /// 아이템 갯수
    /// </summary>
    public int amount = 0;

    /// <summary>
    /// 아이템 사용 여부
    /// </summary>
    public bool isInUse = false;
}

/// <summary>
/// 플레이어 게임 데이터
/// </summary>
[Serializable]
public class PlayerGameData
{
    public long gold = 0;
    public long bronze = 0;
    public int level = 0;
    public int hammerCode = 0;
    public long currentExp = 0;
    public long maxExp = 0;
    public int currentChestTypeIndex = 0;
    public int currentChestInfoIndex = 0;
    public List<UserToolData> toolInventory = new List<UserToolData>();
    public Dictionary<int, UserItemData> itemInventory = new Dictionary<int, UserItemData>();
    public List<int> usedItems = new List<int>();
    public List<int> quickUseItems = new List<int>();
}

public class GameDataManager : MonoBehaviour
{
    // 상수 정의
    private const float ITESELL_RATE = 0.7f; // 70% 판매가
    private const int DEFAULT_BRONZE = 100000;
    private const int DEFAULT_GOLD = 1000;

    [Header("Game Configuration")]
    /// <summary>
    /// 최대 강화 횟수
    /// </summary>
    public int maxReinforcementCount = 0;

    [Header("Data Assets")]
    /// <summary>
    /// 도구 데이터 리스트
    /// </summary>
    public List<ToolData> toolDataList = new List<ToolData>();

    /// <summary>
    /// 도구 퍼센트 데이터 리스트
    /// </summary>
    public List<ToolPercentPriceData> toolPercentDataList = new List<ToolPercentPriceData>();

    /// <summary>
    /// 아이템 데이터 리스트
    /// </summary>
    public List<ItemData> itemDataList = new List<ItemData>();

    // 런타임 데이터
    private PlayerGameData playerData = new PlayerGameData();
    
    // 캐시된 딕셔너리들
    private Dictionary<int, ToolData> toolDataDictionary = new Dictionary<int, ToolData>();
    private Dictionary<ToolType.TYPE, ToolPercentPriceData> toolPercentDictionary = new Dictionary<ToolType.TYPE, ToolPercentPriceData>();
    private Dictionary<int, ItemData> itemDataDictionary = new Dictionary<int, ItemData>();
    private Dictionary<int, int> toolCodeAmountDictionary = new Dictionary<int, int>();

    // 이벤트 시스템
    public static event Action<long> OnGoldChanged;
    public static event Action<long> OnBronzeChanged;
    public static event Action<int> OnLevelChanged;
    public static event Action<long, long> OnExpChanged;

    // 싱글톤
    private static GameDataManager instance;
    public static GameDataManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameDataManager>();
                if (instance == null)
                {
                    Debug.LogError("GameDataManager instance not found in scene!");
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        DontDestroyOnLoad(gameObject);
        
        InitializeDataDictionaries();
    }

    private void Start()
    {
        InitializePlayerData();
    }

    /// <summary>
    /// 데이터 딕셔너리 초기화
    /// </summary>
    private void InitializeDataDictionaries()
    {
        // 툴 데이터 딕셔너리 생성
        toolDataDictionary.Clear();
        foreach (var toolData in toolDataList)
        {
            if (!toolDataDictionary.ContainsKey(toolData.code))
            {
                toolDataDictionary.Add(toolData.code, toolData);
            }
        }

        // 툴 퍼센트 데이터 딕셔너리 생성
        toolPercentDictionary.Clear();
        foreach (var percentData in toolPercentDataList)
        {
            if (!toolPercentDictionary.ContainsKey(percentData.toolType))
            {
                toolPercentDictionary.Add(percentData.toolType, percentData);
            }
        }

        // 아이템 데이터 딕셔너리 생성
        itemDataDictionary.Clear();
        foreach (var itemData in itemDataList)
        {
            if (!itemDataDictionary.ContainsKey(itemData.code))
            {
                itemDataDictionary.Add(itemData.code, itemData);
            }
        }

        // 원본 리스트는 더 이상 필요 없으므로 정리
        toolDataList.Clear();
        toolPercentDataList.Clear();
        itemDataList.Clear();
    }

    /// <summary>
    /// 플레이어 데이터 초기화
    /// </summary>
    private void InitializePlayerData()
    {
        // 기본값 설정 (나중에 저장된 데이터로 덮어쓸 예정)
        if (playerData.bronze == 0) playerData.bronze = DEFAULT_BRONZE;
        if (playerData.gold == 0) playerData.gold = DEFAULT_GOLD;
        
        // UI 업데이트 이벤트 발생
        OnBronzeChanged?.Invoke(playerData.bronze);
        OnGoldChanged?.Invoke(playerData.gold);
        OnLevelChanged?.Invoke(playerData.level);
        OnExpChanged?.Invoke(playerData.currentExp, playerData.maxExp);
    }

    /// <summary>
    /// 데이터 저장
    /// </summary>
    public void SaveGameData()
    {
        try
        {
            string jsonData = JsonUtility.ToJson(playerData, true);
            PlayerPrefs.SetString("PlayerGameData", jsonData);
            PlayerPrefs.Save();
            Debug.Log("Game data saved successfully.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save game data: {e.Message}");
        }
    }

    /// <summary>
    /// 데이터 로드
    /// </summary>
    public void LoadGameData()
    {
        try
        {
            if (PlayerPrefs.HasKey("PlayerGameData"))
            {
                string jsonData = PlayerPrefs.GetString("PlayerGameData");
                playerData = JsonUtility.FromJson<PlayerGameData>(jsonData);
                
                // UI 업데이트 이벤트 발생
                OnBronzeChanged?.Invoke(playerData.bronze);
                OnGoldChanged?.Invoke(playerData.gold);
                OnLevelChanged?.Invoke(playerData.level);
                OnExpChanged?.Invoke(playerData.currentExp, playerData.maxExp);
                
                Debug.Log("Game data loaded successfully.");
            }
            else
            {
                Debug.Log("No saved game data found. Using default values.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load game data: {e.Message}");
            InitializePlayerData(); // 로드 실패 시 기본값으로 초기화
        }
    }

    /// <summary>
    /// 툴 갯수 관리
    /// </summary>
    /// <param name="argCode">툴 코드</param>
    /// <param name="argRein">툴 강화 횟수</param>
    /// <param name="argAmount">추가할 툴 갯수</param>
    public void ManageTool(int argCode, int argRein, int argAmount)
    {
        ToolData _toolData = null;
        bool _isAdd = false;
        int _amount = 0;

        if (!toolDataDictionary.TryGetValue(argCode, out _toolData))
        {
            return;
        }
        
        for (int i = 0; i < playerData.toolInventory.Count; i++)
        {
            if(playerData.toolInventory[i].code == argCode && playerData.toolInventory[i].reinforcement == argRein)
            {
                playerData.toolInventory[i].amount += argAmount;
                if(playerData.toolInventory[i].amount == 0)
                {
                    playerData.toolInventory.RemoveAt(i);
                }
                _isAdd = true;
                break;
            }
        }

        if (!_isAdd)
        {
            UserToolData _data = new UserToolData();
            _data.code = argCode;
            _data.reinforcement = argRein;
            _data.amount = argAmount;
            playerData.toolInventory.Add(_data);
        }
        
        if (!toolCodeAmountDictionary.TryGetValue(argCode, out _amount))
        {
            toolCodeAmountDictionary.Add(argCode, argAmount);
        }
        else
        {
            toolCodeAmountDictionary[argCode] += argAmount;
        }
        if(toolCodeAmountDictionary[argCode] <= 0)
        {
            toolCodeAmountDictionary.Remove(argCode);
        }
    }

    /// <summary>
    /// 툴 갯수 확인
    /// </summary>
    /// <param name="argCode">툴 코드</param>
    /// <param name="argRein">툴 강화 횟수</param>
    /// <returns></returns>
    public int ManageTool(int argCode, int argRein)
    {
        for (int i = 0; i < playerData.toolInventory.Count; i++)
        {
            if (playerData.toolInventory[i].code == argCode && playerData.toolInventory[i].reinforcement == argRein)
            {
                return playerData.toolInventory[i].amount;
            }
        }
        return 0;
    }
    
    /// <summary>
    /// 아이템 추가/제거
    /// </summary>
    /// <param name="itemCode">아이템 코드</param>
    /// <param name="amount">추가할 아이템 갯수 (음수면 제거)</param>
    /// <returns>성공 여부</returns>
    public bool ChangeItemAmount(int itemCode, int amount)
    {
        if (!itemDataDictionary.ContainsKey(itemCode))
        {
            Debug.LogError($"Invalid item code: {itemCode}");
            return false;
        }

        EnsureItemExists(itemCode);
        
        int newAmount = playerData.itemInventory[itemCode].amount + amount;
        
        if (newAmount < 0)
        {
            if (GameManager.Instance != null)
                GameManager.Instance.Warning("아이템이 부족합니다.");
            return false;
        }
        
        playerData.itemInventory[itemCode].amount = newAmount;
        
        if (newAmount == 0)
        {
            playerData.itemInventory.Remove(itemCode);
        }
        
        return true;
    }

    /// <summary>
    /// 아이템 사용/해제
    /// </summary>
    /// <param name="itemCode">아이템 코드</param>
    /// <param name="use">사용 여부</param>
    /// <returns>성공 여부</returns>
    public bool SetItemUsage(int itemCode, bool use)
    {
        if (!itemDataDictionary.ContainsKey(itemCode))
        {
            Debug.LogError($"Invalid item code: {itemCode}");
            return false;
        }

        EnsureItemExists(itemCode);

        if (playerData.usedItems.Contains(itemCode))
        {
            if (use)
            {
                if (GameManager.Instance != null)
                    GameManager.Instance.Warning("아이템이 이미 사용 중입니다");
                return false;
            }
            playerData.usedItems.Remove(itemCode);
        }

        if (use)
        {
            if (playerData.itemInventory[itemCode].amount <= 0)
            {
                if (GameManager.Instance != null)
                    GameManager.Instance.Warning("사용할 아이템이 없습니다.");
                return false;
            }
            playerData.usedItems.Add(itemCode);
        }

        playerData.itemInventory[itemCode].isInUse = use;
        return true;
    }

    /// <summary>
    /// 아이템 정보 조회
    /// </summary>
    /// <param name="itemCode">아이템 코드</param>
    /// <returns>아이템 데이터 (없으면 null)</returns>
    public UserItemData GetItemData(int itemCode)
    {
        if (!HasItem(itemCode))
        {
            return null;
        }
        
        return playerData.itemInventory[itemCode];
    }

    /// <summary>
    /// 아이템 보유 여부 확인
    /// </summary>
    /// <param name="itemCode">아이템 코드</param>
    /// <returns>보유 여부</returns>
    public bool HasItem(int itemCode)
    {
        return playerData.itemInventory.ContainsKey(itemCode) && 
               playerData.itemInventory[itemCode].amount > 0;
    }


    /// <summary>
    /// 아이템이 인벤토리에 존재하는지 확인하고, 없으면 생성
    /// </summary>
    /// <param name="itemCode">아이템 코드</param>
    private void EnsureItemExists(int itemCode)
    {
        if (!playerData.itemInventory.ContainsKey(itemCode))
        {
            UserItemData newItemData = new UserItemData
            {
                code = itemCode,
                amount = 0,
                isInUse = false
            };
            playerData.itemInventory.Add(itemCode, newItemData);
        }
    }

    /// <summary>
    /// 툴 데이터 조회
    /// </summary>
    /// <param name="toolCode">툴 코드</param>
    /// <returns>툴 데이터 (없으면 null)</returns>
    public ToolData GetToolData(int toolCode)
    {
        toolDataDictionary.TryGetValue(toolCode, out ToolData toolData);
        return toolData;
    }

    /// <summary>
    /// 아이템 데이터 조회
    /// </summary>
    /// <param name="itemCode">아이템 코드</param>
    /// <returns>아이템 데이터 (없으면 null)</returns>
    public ItemData GetItemMasterData(int itemCode)
    {
        itemDataDictionary.TryGetValue(itemCode, out ItemData itemData);
        return itemData;
    }

    /// <summary>
    /// 툴 퍼센트 데이터 조회
    /// </summary>
    /// <param name="toolType">툴 타입</param>
    /// <returns>툴 퍼센트 데이터 (없으면 null)</returns>
    public ToolPercentPriceData GetToolPercentData(ToolType.TYPE toolType)
    {
        toolPercentDictionary.TryGetValue(toolType, out ToolPercentPriceData percentData);
        return percentData;
    }

    /// <summary>
    /// 빠른 사용 아이템 추가
    /// </summary>
    /// <param name="itemCode">아이템 코드</param>
    /// <returns>성공 여부</returns>
    public bool AddQuickUseItem(int itemCode)
    {
        if (!itemDataDictionary.ContainsKey(itemCode))
        {
            Debug.LogError($"Invalid item code: {itemCode}");
            return false;
        }

        if (!playerData.quickUseItems.Contains(itemCode))
        {
            playerData.quickUseItems.Add(itemCode);
            return true;
        }
        
        return false; // 이미 존재함
    }

    /// <summary>
    /// 빠른 사용 아이템 제거
    /// </summary>
    /// <param name="itemCode">아이템 코드</param>
    /// <returns>성공 여부</returns>
    public bool RemoveQuickUseItem(int itemCode)
    {
        return playerData.quickUseItems.Remove(itemCode);
    }

    /// <summary>
    /// 빠른 사용 아이템 목록 조회
    /// </summary>
    /// <returns>빠른 사용 아이템 코드 목록</returns>
    public List<int> GetQuickUseItems()
    {
        return new List<int>(playerData.quickUseItems);
    }

    /// <summary>
    /// 사용 중인 아이템 목록 조회
    /// </summary>
    /// <returns>사용 중인 아이템 코드 목록</returns>
    public List<int> GetUsedItems()
    {
        return new List<int>(playerData.usedItems);
    }

    /// <summary>
    /// 사용 중인 아이템 목록 초기화
    /// </summary>
    public void ClearUsedItems()
    {
        playerData.usedItems.Clear();
    }

    /// <summary>
    /// 모든 아이템 마스터 데이터 조회
    /// </summary>
    /// <returns>아이템 코드와 데이터의 딕셔너리</returns>
    public Dictionary<int, ItemData> GetAllItemMasterData()
    {
        return new Dictionary<int, ItemData>(itemDataDictionary);
    }

    // ========== 기존 코드와의 호환성을 위한 래퍼 메서드들 ==========
    
    /// <summary>
    /// 아이템 갯수 관리 (기존 호환성)
    /// </summary>
    public void ManageItem(int argCode, int argAmount)
    {
        ChangeItemAmount(argCode, argAmount);
    }

    /// <summary>
    /// 아이템 사용 관리 (기존 호환성)
    /// </summary>
    public void ManageItem(int argCode, bool argUse)
    {
        SetItemUsage(argCode, argUse);
    }

    /// <summary>
    /// 아이템 확인 (기존 호환성)
    /// </summary>
    public UserItemData ManageItem(int argCode)
    {
        return GetItemData(argCode);
    }

    /// <summary>
    /// 브론즈 변경 (기존 호환성)
    /// </summary>
    public bool ChangeBronze(long argBronze)
    {
        return TrySpendBronze(argBronze);
    }

    /// <summary>
    /// 골드 변경 (기존 호환성)
    /// </summary>
    public bool ChangeGold(long argGold)
    {
        return TrySpendGold(argGold);
    }

    // ========== UI 업데이트 메서드들 ==========

    /// <summary>
    /// 상자 업데이트
    /// </summary>
    public void UpdateChest(ChestType.TYPE argType)
    {
        ResetChest();

        if(argType == ChestType.TYPE.Tool)
        {
            int _count = 0;
            foreach (KeyValuePair<int, int> item in toolCodeAmountDictionary)
            {
                var chestManager = GameManager.Instance?.chestManager;
                if (chestManager != null && _count < chestManager.chestBtn.Length)
                {
                    chestManager.chestBtn[_count].SetBtn(
                        item.Key,
                        toolDataDictionary[item.Key].image,
                        item.Value.ToString(),
                        false
                    );
                }

                _count++;
            }
        }
        else if(argType == ChestType.TYPE.Item)
        {
            int _count = 0;
            foreach (KeyValuePair<int, UserItemData> item in playerData.itemInventory)
            {
                var chestManager = GameManager.Instance?.chestManager;
                if (chestManager != null && _count < chestManager.chestBtn.Length)
                {
                    chestManager.chestBtn[_count].SetBtn(
                        item.Key,
                        itemDataDictionary[item.Key].image,
                        item.Value.amount.ToString(),
                        item.Value.isInUse
                    );
                }

                _count++;
            }
        }
        else
        {

        }

    }

    /// <summary>
    /// 검 강화 상자 업데이트
    /// </summary>
    public void UpdateToolReinChest(int argCode)
    {
        var chestManager = GameManager.Instance?.chestManager;
        if (chestManager?.chestReinBtn == null) return;

        for (int i = 0; i < chestManager.chestReinBtn.Length; i++)
        {
            chestManager.chestReinBtn[i].SetBtn(
                -1,
                null,
                string.Empty,
                string.Empty,
                false
            );
        }

        int _count = 0;
        playerData.toolInventory.Sort((a, b) =>
        {
            return b.reinforcement.CompareTo(a.reinforcement);
        });

        foreach(UserToolData item in playerData.toolInventory)
        {
            if(item.code == argCode)
            {
                if (_count < chestManager.chestReinBtn.Length)
                {
                    chestManager.chestReinBtn[_count].SetBtn(
                        item.reinforcement,
                        toolDataDictionary[item.code].image,
                        "+" + item.reinforcement.ToString(),
                        item.amount.ToString(),
                        false
                    );
                }

                _count++;
            }
        }
    }

    /// <summary>
    /// 상자 리셋
    /// </summary>
    void ResetChest()
    {
        var chestManager = GameManager.Instance?.chestManager;
        if (chestManager?.chestBtn == null) return;

        for (int i = 0; i < chestManager.chestBtn.Length; i++)
        {
            chestManager.chestBtn[i].SetBtn(
                0,
                null,
                string.Empty,
                false
            );
        }
    }
    
    /// <summary>
    /// 브론즈 사용 가능 여부 확인 및 차감
    /// </summary>
    /// <param name="amount">사용할 브론즈 양</param>
    /// <returns>사용 가능 여부</returns>
    public bool TrySpendBronze(long amount)
    {
        if (playerData.bronze < amount || amount < 0)
        {
            if (GameManager.Instance != null)
                GameManager.Instance.Warning("브론즈가 부족합니다.");
            return false;
        }
        
        Bronze -= amount;
        return true;
    }

    /// <summary>
    /// 골드 사용 가능 여부 확인 및 차감
    /// </summary>
    /// <param name="amount">사용할 골드 양</param>
    /// <returns>사용 가능 여부</returns>
    public bool TrySpendGold(long amount)
    {
        if (playerData.gold < amount || amount < 0)
        {
            if (GameManager.Instance != null)
                GameManager.Instance.Warning("골드가 부족합니다.");
            return false;
        }
        
        Gold -= amount;
        return true;
    }

    /// <summary>
    /// 브론즈 추가
    /// </summary>
    /// <param name="amount">추가할 브론즈 양</param>
    public void AddBronze(long amount)
    {
        if (amount > 0)
        {
            Bronze += amount;
        }
    }

    /// <summary>
    /// 골드 추가
    /// </summary>
    /// <param name="amount">추가할 골드 양</param>
    public void AddGold(long amount)
    {
        if (amount > 0)
        {
            Gold += amount;
        }
    }

    /// <summary>
    /// 경험치 추가 및 레벨업 처리
    /// </summary>
    /// <param name="expAmount">추가할 경험치</param>
    public void AddExperience(long expAmount)
    {
        if (expAmount <= 0) return;

        Exp += expAmount;
        
        // 레벨업 체크
        while (Exp >= MaxExp && MaxExp > 0)
        {
            Exp -= MaxExp;
            Level++;
            
            // 다음 레벨 최대 경험치 계산 (예시: 레벨 * 1000)
            MaxExp = Level * 1000;
            
            if (GameManager.Instance != null)
                GameManager.Instance.Warning($"레벨 업! 현재 레벨: {Level}");
        }
    }

    // ========== 프로퍼티들 (UI 업데이트 포함) ==========

    /// <summary>
    /// 브론즈 (게임머니)
    /// </summary>
    public long Bronze
    {
        get => playerData.bronze;
        set
        {
            if (playerData.bronze != value)
            {
                playerData.bronze = Math.Max(0, value);
                OnBronzeChanged?.Invoke(playerData.bronze);
            }
        }
    }

    /// <summary>
    /// 골드 (캐시)
    /// </summary>
    public long Gold
    {
        get => playerData.gold;
        set
        {
            if (playerData.gold != value)
            {
                playerData.gold = Math.Max(0, value);
                OnGoldChanged?.Invoke(playerData.gold);
            }
        }
    }

    /// <summary>
    /// 플레이어 레벨
    /// </summary>
    public int Level
    {
        get => playerData.level;
        set
        {
            if (playerData.level != value)
            {
                playerData.level = Math.Max(0, value);
                OnLevelChanged?.Invoke(playerData.level);
            }
        }
    }

    /// <summary>
    /// 현재 경험치
    /// </summary>
    public long Exp
    {
        get => playerData.currentExp;
        set
        {
            if (playerData.currentExp != value)
            {
                playerData.currentExp = Math.Max(0, value);
                OnExpChanged?.Invoke(playerData.currentExp, playerData.maxExp);
            }
        }
    }

    /// <summary>
    /// 최대 경험치
    /// </summary>
    public long MaxExp
    {
        get => playerData.maxExp;
        set
        {
            if (playerData.maxExp != value)
            {
                playerData.maxExp = Math.Max(1, value);
                OnExpChanged?.Invoke(playerData.currentExp, playerData.maxExp);
            }
        }
    }



    /// <summary>
    /// 게임 종료 시 자동 저장
    /// </summary>
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveGameData();
        }
    }

    /// <summary>
    /// 게임 종료 시 자동 저장
    /// </summary>
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            SaveGameData();
        }
    }

    /// <summary>
    /// 앱 종료 시 자동 저장
    /// </summary>
    private void OnDestroy()
    {
        SaveGameData();
    }
}
