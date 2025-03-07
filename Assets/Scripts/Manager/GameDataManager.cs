using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 유저 툴 데이터
/// </summary>
public class UserToolData
{
    /// <summary>
    /// 툴 코드
    /// </summary>
    public int m_code = 0;

    /// <summary>
    /// 툴 갯수
    /// </summary>
    public int m_amount = 0;

    /// <summary>
    /// 강화 횟수
    /// </summary>
    public int m_rein = 0;
}

/// <summary>
/// 유저 아이템 데이터
/// </summary>
public class UserItemData
{
    /// <summary>
    /// 아이템 코드
    /// </summary>
    public int m_code = 0;

    /// <summary>
    /// 아이템 갯수
    /// </summary>
    public int m_amount = 0;

    /// <summary>
    /// 아이템 사용 여부
    /// </summary>
    public bool m_use = false;
}

public class GameDataManager : MonoBehaviour
{
    /// <summary>
    /// 골드(캐쉬)
    /// </summary>
    long m_gold = 0;

    /// <summary>
    /// 브론즈(게임머니)
    /// </summary>
    long m_bronze = 0;

    /// <summary>
    /// 레벨
    /// </summary>
    int m_level = 0;

    /// <summary>
    /// 망치 코드
    /// </summary>
    int m_hammerCode = 0;

    /// <summary>
    /// 경험치
    /// </summary>
    long m_nowExp = 0;

    /// <summary>
    /// 최대 경험치
    /// </summary>
    long m_maxExp = 0;

    [Header("Chest")]
    /// <summary>
    /// 현재 상자 타입 인덱스
    /// </summary>
    public int m_nowChestTypeIndex = 0;

    /// <summary>
    /// 현재 상자 정보 인덱스
    /// </summary>
    public int m_nowChestImfoIndex = 0;

    [Header("Tool")]
    /// <summary>
    /// 최대 강화 횟수
    /// </summary>
    public int m_maxReinCount = 0;

    /// <summary>
    /// 도구 데이터 리스트
    /// </summary>
    public List<ToolData> m_toolData = new List<ToolData>();

    /// <summary>
    /// 도구 퍼센트 데이터 리스트
    /// </summary>
    public List<ToolPercentPriceData> m_toolPercentData = new List<ToolPercentPriceData>();

    /// <summary>
    /// 도구 데이터 딕셔너리
    /// </summary>
    public Dictionary<int, ToolData> m_toolDic = new Dictionary<int, ToolData>();

    /// <summary>
    /// 도구 퍼센트 딕셔너리
    /// </summary>
    public Dictionary<StuffType.ToolType, ToolPercentPriceData> m_toolPercentDic = new Dictionary<StuffType.ToolType, ToolPercentPriceData>();
    
    /// <summary>
    /// 코드에 따른 툴 갯수 정보 딕셔너리
    /// </summary>
    public Dictionary<int, int> m_toolCodeAmountDic = new Dictionary<int, int>();

    /// <summary>
    /// 유저가 갖고있는 도구 정보
    /// </summary>
    List<UserToolData> m_toolChest = new List<UserToolData>();

    [Header("Item")]
    /// <summary>
    /// 아이템 데이터 리스트
    /// </summary>
    public List<ItemData> m_itemData = new List<ItemData>();

    /// <summary>
    /// 아이템 데이터 딕셔너리
    /// </summary>
    public Dictionary<int, ItemData> m_itemDic = new Dictionary<int, ItemData>();

    /// <summary>
    /// 유저가 갖고있는 아이템 정보
    /// </summary>
    Dictionary<int, UserItemData> m_itemChest = new Dictionary<int, UserItemData>();

    /// <summary>
    /// 사용한 아이템
    /// </summary>
    public List<int> m_useItem = new List<int>();

    /// <summary>
    /// 빠른 아이템 사용 버튼 아이템 코드 리스트
    /// </summary>
    public List<int> m_quickItemUse = new List<int>();

    /// <summary>
    /// 글로벌화
    /// </summary>
    static GameDataManager g_gameDataManager = null;

    public void OnEnable()
    {
        if(g_gameDataManager != null)
        {
            Destroy(gameObject);
        }
        else
        {
            g_gameDataManager = this;
            DontDestroyOnLoad(gameObject);
        }

        for (int i = 0; i < m_toolData.Count; i++)
        {
            m_toolDic.Add(m_toolData[i].m_code, m_toolData[i]);
        }

        for(int i = 0; i < m_toolPercentData.Count; i++)
        {
            m_toolPercentDic.Add(m_toolPercentData[i].m_toolType, m_toolPercentData[i]);
        }

        for (int i = 0; i < m_itemData.Count; i++)
        {
            m_itemDic.Add(m_itemData[i].m_code, m_itemData[i]);
        }

        m_toolData.Clear();
        m_toolData = null;
        m_toolPercentData.Clear();
        m_toolPercentData = null;
        m_itemData.Clear();
        m_itemData = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        Setting();
    }

    /// <summary>
    /// 초기 셋팅
    /// </summary>
    void Setting()
    {
        IsBronze += 100000;
        IsGold += 1000;
    }

    /// <summary>
    /// 저장
    /// </summary>
    public void Save()
    {
        
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

        if (!m_toolDic.TryGetValue(argCode, out _toolData))
        {
            return;
        }
        
        for (int i = 0; i < m_toolChest.Count; i++)
        {
            if(m_toolChest[i].m_code == argCode && m_toolChest[i].m_rein == argRein)
            {
                m_toolChest[i].m_amount += argAmount;
                if(m_toolChest[i].m_amount == 0)
                {
                    m_toolChest.RemoveAt(i);
                }
                _isAdd = true;
                break;
            }
        }

        if (!_isAdd)
        {
            UserToolData _data = new UserToolData();
            _data.m_code = argCode;
            _data.m_rein = argRein;
            _data.m_amount = argAmount;
            m_toolChest.Add(_data);
        }
        
        if (!m_toolCodeAmountDic.TryGetValue(argCode, out _amount))
        {
            m_toolCodeAmountDic.Add(argCode, argAmount);
        }
        else
        {
            m_toolCodeAmountDic[argCode] += argAmount;
        }
        if(m_toolCodeAmountDic[argCode] <= 0)
        {
            m_toolCodeAmountDic.Remove(argCode);
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
        for (int i = 0; i < m_toolChest.Count; i++)
        {
            if (m_toolChest[i].m_code == argCode && m_toolChest[i].m_rein == argRein)
            {
                return m_toolChest[i].m_amount;
            }
        }
        return 0;
    }
    
    /// <summary>
    /// 아이템 갯수 추가
    /// </summary>
    /// <param name="argCode">아이템 코드</param>
    /// <param name="argAmount">추가할 아이템 갯수</param>
    public void ManageItem(int argCode, int argAmount)
    {
        AddItemImfo(argCode, argAmount);
        m_itemChest[argCode].m_amount += argAmount;
        if(m_itemChest[argCode].m_amount <= 0)
        {
            m_itemChest.Remove(argCode);
        }
    }

    /// <summary>
    /// 아이템 사용
    /// </summary>
    /// <param name="argCode"></param>
    /// <param name="argUse"></param>
    public void ManageItem(int argCode, bool argUse)
    {
        CheckChestItem(argCode);

        if(m_useItem.Contains(argCode))
        {
            if (argUse)
            {
                GManager.Instance.Warning("아이템이 이미 사용 중입니다");
                return;
            }
            m_useItem.Remove(argCode);
        }

        if (argUse)
        {
            m_useItem.Add(argCode);
        }

        m_itemChest[argCode].m_use = argUse;
    }

    /// <summary>
    /// 아이템 확인
    /// </summary>
    /// <param name="argCode">아이템 코드</param>
    /// <returns>갯수</returns>
    public UserItemData ManageItem(int argCode)
    {
        if (!CheckChestItem(argCode))
        {
            return null;
        }
        
        return m_itemChest[argCode];
    }


    /// <summary>
    /// 아이템 정보 추가
    /// </summary>
    /// <param name="argCode">아이템 코드</param>
    /// <param name="argAmount">추가할 아이템 갯수</param>
    void AddItemImfo(int argCode, int argAmount)
    {
        CheckChestItem(argCode);

        UserItemData _data = null;
        m_itemChest.TryGetValue(argCode, out _data);
        if (_data == null)
        {
            UserItemData _tmpData = new UserItemData();

            _tmpData.m_code = argCode;
            _tmpData.m_amount = 0;
            _tmpData.m_use = false;
            m_itemChest.Add(argCode, _tmpData);
            return;
        }
    }

    /// <summary>
    /// 아이템 존재 여부 확인
    /// </summary>
    /// <param name="argCode">코드</param>
    public bool CheckChestItem(int argCode)
    {
        UserItemData _data = null;
        m_itemChest.TryGetValue(argCode, out _data);
        if (_data == null)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 상자 업데이트
    /// </summary>
    public void UpdateChest(PanelType.ChestType argType)
    {
        ResetChest();

        if(argType == PanelType.ChestType.Tool)
        {
            int _count = 0;
            foreach (KeyValuePair<int, int> item in m_toolCodeAmountDic)
            {
                GManager.Instance.m_chestManager.m_chestBtn[_count].SetBtn(
                   item.Key,
                   m_toolDic[item.Key].m_image,
                   item.Value.ToString(),
                   false
                   );

                _count++;
            }
        }
        else if(argType == PanelType.ChestType.Item)
        {
            int _count = 0;
            foreach (KeyValuePair<int, UserItemData> item in m_itemChest)
            {
                GManager.Instance.m_chestManager.m_chestBtn[_count].SetBtn(
                    item.Key,
                    m_itemDic[item.Key].m_image,
                    item.Value.m_amount.ToString(),
                    ManageItem(item.Key).m_use
                    );

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
        for (int i = 0; i < GManager.Instance.m_chestManager.m_chestReinBtn.Length; i++)
        {
            GManager.Instance.m_chestManager.m_chestReinBtn[i].SetBtn(
                -1,
                null,
                string.Empty,
                string.Empty,
                false
                );
        }

        int _count = 0;
        m_toolChest.Sort((a, b) =>
        {
            return b.m_rein.CompareTo(a.m_rein);
        });

        foreach(UserToolData item in m_toolChest)
        {
            if(item.m_code == argCode)
            {
                GManager.Instance.m_chestManager.m_chestReinBtn[_count].SetBtn(
                item.m_rein,
                m_toolDic[item.m_code].m_image,
                "+" + item.m_rein.ToString(),
                item.m_amount.ToString(),
                false
                );

                _count++;
            }
        }
    }

    /// <summary>
    /// 상자 리셋
    /// </summary>
    void ResetChest()
    {
        for (int i = 0; i < GManager.Instance.m_chestManager.m_chestBtn.Length; i++)
        {
            GManager.Instance.m_chestManager.m_chestBtn[i].SetBtn(
                0,
                null,
                string.Empty,
                false
                );
        }
    }
    
    /// <summary>
    /// 브론즈 청구, 확인
    /// </summary>
    /// <param name="argBronze">브론즈</param>
    /// <returns>가능 여부</returns>
    public bool ChangeBronze(long argBronze)
    {
        if(IsBronze < argBronze)
        {
            NoMoney();
            return false;
        }
        if(IsBronze <= 0)
        {
            IsBronze = 0;
            NoMoney();
            return false;
        }
        return true;
    }

    /// <summary>
    /// 골드 청구, 확인
    /// </summary>
    /// <param name="argBronze">골드</param>
    /// <returns>가능 여부</returns>
    public bool ChangeGold(long argBronze)
    {
        if (IsGold < argBronze)
        {
            NoMoney();
            return false;
        }
        if (IsGold <= 0)
        {
            IsGold = 0;
            NoMoney();
            return false;
        }
        return true;
    }

    /// <summary>
    /// 브론즈 변경
    /// </summary>
    public long IsBronze
    {
        get
        {
            return m_bronze;
        }
        set
        {
            m_bronze = value;
            if(m_bronze <= 0)
            {
                GManager.Instance.m_BronzeText.text = string.Format("브론즈 (B) : 0");
                return;
            }
            GManager.Instance.m_BronzeText.text = string.Format("브론즈 (B) : {0:#,###}", m_bronze);
        }
    }

    /// <summary>
    /// 골드 변경
    /// </summary>
    public long IsGold
    {
        get
        {
            return m_gold;
        }
        set
        {
            m_gold = value;
            if (m_gold <= 0)
            {
                GManager.Instance.m_BronzeText.text = string.Format("골드 (G) : 0");
                return;
            }
            GManager.Instance.m_GoldText.text = string.Format("골드 (G) : {0:#,###}", m_gold);
        }
    }

    /// <summary>
    /// 레벨 변경
    /// </summary>
    public int IsLevel
    {
        get
        {
            return m_level;
        }
        set
        {
            m_level = value;
            if (m_level <= 0)
            {
                GManager.Instance.m_levelText.text = string.Format("LV.0");
                return;
            }
            GManager.Instance.m_levelText.text = string.Format("LV.{0}", m_level);
        }
    }

    /// <summary>
    /// 현재 경험치 변경
    /// </summary>
    public long IsNowExp
    {
        get
        {
            return m_nowExp;
        }
        set
        {
            m_nowExp = value;
            GManager.Instance.m_expSlider.value = m_nowExp;
        }
    }

    /// <summary>
    /// 최대 경험치 변경
    /// </summary>
    public long IsMaxExp
    {
        get
        {
            return m_maxExp;
        }
        set
        {
            m_nowExp = value;
            GManager.Instance.m_expSlider.maxValue = m_maxExp;
        }
    }

    /// <summary>
    /// 돈 없음
    /// </summary>
    void NoMoney()
    {
        GManager.Instance.Warning("돈이 없습니다");
    }

    /// <summary>
    /// 인스턴스
    /// </summary>
    public static GameDataManager Instance
    {
        get
        {
            return g_gameDataManager;
        }
    }
}
