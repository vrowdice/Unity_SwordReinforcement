using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolPanel : BasePanel
{
    [Header("Tool Panel UI Components")]
    /// <summary>
    /// 개선 버튼
    /// </summary>
    [SerializeField] GameObject improveBtn = null;

    /// <summary>
    /// 검 이미지
    /// </summary>
    [SerializeField] Image toolImage = null;

    /// <summary>
    /// 검 텍스트
    /// </summary>
    [SerializeField] Text toolText = null;

    /// <summary>
    /// 판매비용 택스트
    /// </summary>
    [SerializeField] Text sellCostTxt = null;

    /// <summary>
    /// 강화 비용
    /// </summary>
    [SerializeField] Text reinforceCostTxt = null;

    /// <summary>
    /// 강화 확률
    /// </summary>
    [SerializeField] Text reinforcePercentTxt = null;

    /// <summary>
    /// 현재 모드 텍스트
    /// </summary>
    [SerializeField] Text modeText = null;

    /// <summary>
    /// 상자 패널의 검 이미지
    /// </summary>
    [SerializeField] Image chestToolImg;

    /// <summary>
    /// 상자 패널의 검 설명
    /// </summary>
    [SerializeField] Text chestToolExp;

    [Header("Item Quick Use UI")]
    public GameObject icon = null;
    public GameObject itemIconSortPanel = null;
    public GameObject itemQuickUseBtn = null;
    public GameObject itemQuickUseContent = null;

    /// <summary>
    /// 현재 툴 타입
    /// </summary>
    StuffType.ToolType nowToolType = new StuffType.ToolType();

    /// <summary>
    /// 현재 도구 패널 모드
    /// </summary>
    PanelType.ImproveType nowImproveMode = new PanelType.ImproveType();

    /// <summary>
    /// 현재 강화횟수
    /// </summary>
    private int nowToolCode = 0;

    /// <summary>
    /// 현재 툴 강화횟수
    /// </summary>
    private int nowToolRein = 0;

    /// <summary>
    /// 현재 선택한 툴 코드
    /// </summary>
    int nowChestToolCode = 0;

    /// <summary>
    /// 현재 선택한 툴 강화횟수
    /// </summary>
    int nowChestToolRein = 0;

    /// <summary>
    /// 강화 확률
    /// </summary>
    float reinforcePercent = 0.0f;

    /// <summary>
    /// 추가될 강화 확률
    /// </summary>
    float addRP = 0.0f;

    /// <summary>
    /// 검 파괴방지 플래그
    /// </summary>
    bool noDestroyFlag = false;

    /// <summary>
    /// 판매 비용
    /// </summary>
    long sellCost = 0;

    /// <summary>
    /// 강화 비용
    /// </summary>
    long reinforceCost = 0;

    /// <summary>
    /// 무작위 값을 산출할 볌위
    /// </summary>
    public float reinforceRange = 0.0f;

    protected override void OnPanelOpen()
    {
        base.OnPanelOpen();
        
        Setting();
    }

    /// <summary>
    /// 초기 셋팅
    /// </summary>
    void Setting()
    {
        // SerializeField로 인스펙터에서 할당하므로 Find 코드들을 제거하거나 null 체크로 대체
        if (toolImage == null)
            toolImage = gameObject.transform.Find("ToolImage")?.GetComponent<Image>();
        if (modeText == null)
            modeText = gameObject.transform.Find("ModeText")?.GetComponent<Text>();
        if (toolText == null)
            toolText = gameObject.transform.Find("ToolText")?.GetComponent<Text>();
        if (sellCostTxt == null)
            sellCostTxt = gameObject.transform.Find("SellCostText")?.GetComponent<Text>();
        if (reinforceCostTxt == null)
            reinforceCostTxt = gameObject.transform.Find("CostText")?.GetComponent<Text>();
        if (reinforcePercentTxt == null)
            reinforcePercentTxt = gameObject.transform.Find("PercentText")?.GetComponent<Text>();
        if (improveBtn == null)
            improveBtn = gameObject.transform.Find("ImproveBtn")?.gameObject;

        SelectToolType(StuffType.ToolType.Sword);
        ChangeMode(PanelType.ImproveType.Upgrade);
    }

    /// <summary>
    /// 강화하기 원하는 도구 타입 선택
    /// </summary>
    /// <param name="argType">도구 타입</param>
    public void SelectToolType(StuffType.ToolType argType)
    {
        if (!CheckMainCode(IsNowToolCode))
        {
            ToolPutInChest();
        }
        ResetNowToolImfo();
        
        IsNowToolCode = ParseToolCode(argType, 0);
        IsNowToolType = argType;

        UpdateToolImfo();
    }

    /// <summary>
    /// 넣은 인수에 해당하는 툴을 툴 패널에 생성
    /// </summary>
    /// <param name="argIndex">툴 인덱스</param>
    /// <param name="argRein">툴 강화횟수</param>
    public void MakeTool(int argCode, int argRein)
    {
        IsNowToolCode = argCode;
        IsNowReinCount = argRein;
        
        UpdateToolImfo();
    }

    /// <summary>
    /// 다른 모드로 변경
    /// </summary>
    public void OtherMode()
    {
        if(nowImproveMode == PanelType.ImproveType.Reinforce)
        {
            ChangeMode(PanelType.ImproveType.Upgrade);
        }
        else
        {
            ChangeMode(PanelType.ImproveType.Reinforce);
        }
    }

    /// <summary>
    /// 모드 변경
    /// </summary>
    /// <param name="argType">모드 타입</param>
    void ChangeMode(PanelType.ImproveType argType)
    {
        nowImproveMode = argType;

        string _string = string.Format("업그레이드");
        if (argType == PanelType.ImproveType.Reinforce)
        {
            _string = string.Format("강화");
        }

        modeText.text = string.Format(_string + " 모드");
        improveBtn.transform.GetChild(0).gameObject.GetComponent<Text>().text = string.Format(_string + "하기");
    }

    /// <summary>
    /// 툴 개선
    /// </summary>
    public void ImproveTool()
    {
        ToolData _data = null;
        GameDataManager.Instance.GetToolData(IsNowToolCode);
        if (_data == null)
        {
            GameManager.Instance.Warning("다음 도구가 없습니다.");
            return;
        }

        if (nowImproveMode == PanelType.ImproveType.Reinforce)
        {
            if (CheckMainCode(IsNowToolCode))
            {
                GameManager.Instance.Warning("베이스 도구는 강화가 불가능합니다.");
                return;
            }

            ReinforceTool(IsNowToolCode, IsNowReinCount);

            IsImprovePercent = Random.Range(0, reinforceRange);
            if (IsImprovePercent + IsAddRP <= GameDataManager.Instance.GetToolPercentData(ParseToolType(IsNowToolCode)).reinforcePercent[IsNowReinCount])
            {
                IsNowReinCount += 1;
                UpdateToolImfo();
            }
            else
            {
                ResetNowToolImfo();
            }
        }
        else
        {
            UpgradeTool(IsNowToolCode, IsNowReinCount);

            IsImprovePercent = Random.Range(0, reinforceRange);
            if (IsImprovePercent + IsAddRP <= GameDataManager.Instance.GetToolPercentData(ParseToolType(IsNowToolCode)).UpgradePercent[IsNowToolCode % 1000])
            {
                IsNowToolCode += 1;
                UpdateToolImfo();
            }
            else
            {
                ResetNowToolImfo();
            }
        }
    }

    /// <summary>
    /// 현재 강화된 검 상자에 넣기
    /// </summary>
    public void ToolPutInChest()
    {
        if (CheckMainCode(IsNowToolCode))
        {
            GameManager.Instance.Warning("창고에 넣을 수 없습니다.");
            return;
        }
        
        GameDataManager.Instance.ManageTool(IsNowToolCode, IsNowReinCount, 1);
        ResetNowToolImfo();
    }

    /// <summary>
    /// 도구 판매
    /// </summary>
    public void SellTool()
    {
        if (IsNowToolCode == 0)
        {
            GameManager.Instance.Warning("판매가 불가능합니다.");
            return;
        }
        
        SellTool(IsNowToolCode, IsNowReinCount);
        ResetNowToolImfo();
    }
    
    /// <summary>
    /// 검 판매시 돈 추가
    /// </summary>
    /// <param name="argRein">각 검의 강화횟수</param>
    public void SellTool(int argCode, int argRein)
    {
        long _tmp = PriceTool(argCode, argRein);
        GameDataManager.Instance.Bronze += _tmp;
    }

    /// <summary>
    /// 검 업그레이드 시 돈 차감
    /// </summary>
    /// <param name="argCode"></param>
    /// <param name="argRein"></param>
    public bool UpgradeTool(int argCode, int argRein)
    {
        long _tmp = 0;
        if (argRein <= 0)
        {
            _tmp += (long)(PriceTool(argCode, argRein) * 0.3);
        }
        else
        {
            _tmp += (long)(PriceTool(argCode, argRein) * 0.1);
            _tmp += (long)(PriceTool(argCode, argRein) * 0.3);
        }

        bool _check = GameDataManager.Instance.ChangeBronze(_tmp);
        if(_check == false)
        {
            return false;
        }
        GameDataManager.Instance.Bronze -= _tmp;
        return true;
    }

    /// <summary>
    /// 검 강화 시 돈 차감
    /// </summary>
    /// <param name="argRein">강화횟수</param>
    public bool ReinforceTool(int argCode, int argRein)
    {
        long _tmp = (long)(PriceTool(argCode, argRein) * 0.1);

        bool _check = GameDataManager.Instance.ChangeBronze(_tmp);
        if (_check == false)
        {
            return false;
        }
        GameDataManager.Instance.Bronze -= _tmp;
        return true;
    }
    
    /// <summary>
    /// 강화비용 텍스트와 확률 텍스트, 판매비용 텍스트 변경
    /// </summary>
    /// <param name="argRein">강화 횟수</param>
    public void UpdateToolImfo()
    {
        ToolData _data = null;
        _data = GameDataManager.Instance.GetToolData(IsNowToolCode);

        toolText.text = _data.name + " +" + IsNowReinCount;
        toolImage.sprite = _data.image;

        long _reinCost = PriceTool(IsNowToolCode, IsNowReinCount);
        IsImproveCost = _reinCost * 3 / 10;
        IsSellCost = _reinCost;
        if (nowImproveMode == PanelType.ImproveType.Reinforce)
        {
            IsImprovePercent = GameDataManager.Instance.GetToolPercentData(ParseToolType(IsNowToolCode)).reinforcePercent[IsNowReinCount];
        }
        else
        {
            IsImprovePercent = GameDataManager.Instance.GetToolPercentData(ParseToolType(IsNowToolCode)).UpgradePercent[IsNowToolCode % 1000];
        }
        UpdateToolText();
    }

    /// <summary>
    /// 도구 모두 초기화
    /// </summary>
    void ResetNowToolImfo()
    {
        int _mainCode = ParseToolCode(ParseToolType(nowToolCode), 0);
        toolImage.sprite = GameDataManager.Instance.GetToolData(_mainCode).image;
        toolText.text = GameDataManager.Instance.GetToolData(_mainCode).name;

        IsNowToolCode = _mainCode;
        IsNowReinCount = 0;
        IsImprovePercent = 100;
        IsImproveCost = 0;
        IsSellCost = 0;
        
        if (IsNowToolCode < 10000)
        {
            IsNowToolCode = 10000;
        }

        UpdateToolText();
    }

    /// <summary>
    /// 도구 텍스트 업데이트
    /// </summary>
    void UpdateToolText()
    {
        string _string = string.Empty;
        _string = string.Format("업그레이드");
        if (nowImproveMode == PanelType.ImproveType.Reinforce)
        {
            _string = "강화";
        }

        if (IsAddRP <= 0)
        {
            IsAddRP = 0;
            reinforcePercentTxt.text = string.Format(_string + "확률 : {0:.0}", IsImprovePercent);
        }
        else
        {
            reinforcePercentTxt.text = string.Format(_string + "확률 : {0:.0}", IsImprovePercent + " + " + IsAddRP);
        }

        sellCostTxt.text = string.Format("판매비용 : {0}", sellCost);
        reinforceCostTxt.text = string.Format(_string + "비용 : {0}", reinforceCost);
    }

    /// <summary>
    /// 상자에서 해당하는 툴 선택
    /// </summary>
    public void SelectTool()
    {

    }

    /// <summary>
    /// 상자에서 검 선택
    /// </summary>
    public void SelectRein(int argRein)
    {
        if (GameDataManager.Instance.GetToolData(argRein) == null)
        {
            return;
        }
        
        nowChestToolRein = argRein;

        base.gameObject.SetActive(true);
        chestToolImg.sprite = GameDataManager.Instance.GetToolData(nowChestToolCode).image;
        chestToolImg.GetComponent<Image>().preserveAspect = true;
        chestToolExp.GetComponentInChildren<Text>().text = GameDataManager.Instance.GetToolData(nowChestToolCode).explanation;
    }

    /// <summary>
    /// 도구 도매가 판별
    /// </summary>
    /// <param name="argCode">툴 코드</param>
    /// <param name="argRein">강화 횟수 </param>
    /// <returns>도구 가격</returns>
    public long PriceTool(int argCode, int argRein)
    {
        long _price = GameDataManager.Instance.GetToolData(argCode).price;
        float _persent = 0.0f;
        for (int i = 0; i < argRein; i++)
        {
            if (i <= 5)
            {
                _persent += 10;
            }
            else if (5 < i && i <= 10)
            {
                _persent += 20;
            }
            else if (10 < i && i <= 15)
            {
                _persent += 30;
            }
            else
            {
                _persent += 50;
            }
        }

        _price += (long)(_price * _persent / 100);
        return _price;
    }

    /// <summary>
    /// 메인 코드인지 확인
    /// </summary>
    /// <returns>맞으면 true 아니면 false</returns>
    bool CheckMainCode(int argCode)
    {
        if(argCode % 1000 == 0)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 툴 코드 분석
    /// </summary>
    /// <param name="argType">툴 타입</param>
    /// <param name="argClass">툴 클래스</param>
    /// <returns>툴 코드</returns>
    int ParseToolCode(StuffType.ToolType argType, int argClass)
    {
        int _mainCode = 0;

        switch (argType)
        {
            case StuffType.ToolType.Sword:
                _mainCode = 10000;
                break;
            case StuffType.ToolType.Spear:
                _mainCode = 11000;
                break;
            case StuffType.ToolType.Shild:
                _mainCode = 12000;
                break;
            case StuffType.ToolType.Hammer:
                _mainCode = 13000;
                break;
            case StuffType.ToolType.Sickle:
                _mainCode = 14000;
                break;
            case StuffType.ToolType.Shovle:
                _mainCode = 15000;
                break;
            case StuffType.ToolType.Axe:
                _mainCode = 16000;
                break;
        }
        
        return _mainCode += argClass;
    }

    /// <summary>
    /// 툴 코드 분석
    /// </summary>
    /// <param name="argType">툴 타입</param>
    /// <param name="argClass">툴 클래스</param>
    /// <returns>툴 코드</returns>
    StuffType.ToolType ParseToolType(int argCode)
    {
        StuffType.ToolType _type = new StuffType.ToolType();

        if(argCode / 1000 == 10)
        {
            _type = StuffType.ToolType.Sword;
        }
        if (argCode / 1000 == 11)
        {
            _type = StuffType.ToolType.Spear;
        }
        if (argCode / 1000 == 12)
        {
            _type = StuffType.ToolType.Shild;
        }
        if (argCode / 1000 == 13)
        {
            _type = StuffType.ToolType.Hammer;
        }
        if (argCode / 1000 == 14)
        {
            _type = StuffType.ToolType.Sickle;
        }
        if (argCode / 1000 == 15)
        {
            _type = StuffType.ToolType.Shovle;
        }
        if (argCode / 1000 == 16)
        {
            _type = StuffType.ToolType.Axe;
        }

        return _type;
    }

    public StuffType.ToolType IsNowToolType
    {
        get
        {
            return nowToolType;
        }
        set
        {
            nowToolType = value;
        }
    }

    /// <summary>
    /// 현재 업그레이드 횟수 변경
    /// </summary>
    public int IsNowToolCode
    {
        get
        {
            return nowToolCode;
        }
        set
        {
            nowToolCode = value;
        }
    }
    
    /// <summary>
    /// 현재 강화 횟수 변경
    /// </summary>
    public int IsNowReinCount
    {
        get
        {
            return nowToolRein;
        }
        set
        {
            nowToolRein = value;
        }
    }

    /// <summary>
    /// 판매 가격 변경
    /// </summary>
    public long IsSellCost
    {
        get
        {
            return sellCost;
        }
        set
        {
            sellCost = value;
            if (sellCost <= 0)
            {
                sellCost = 0;
            }
        }
    }

    /// <summary>
    /// 강화 
    /// </summary>
    public long IsImproveCost
    {
        get
        {
            return reinforceCost;
        }
        set
        {
            reinforceCost = value;
            if (reinforceCost <= 0)
            {
                reinforceCost = 0;
            }
        }
    }

    /// <summary>
    /// 강화 퍼센트 변경시
    /// </summary>
    public float IsImprovePercent
    {
        get
        {
            return reinforcePercent;
        }
        set
        {
            reinforcePercent = value;
            if (reinforcePercent >= reinforceRange)
            {
                reinforcePercent = reinforceRange;
            }
        }
    }

    /// <summary>
    /// 추가될 강화 퍼센트 변경시
    /// </summary>
    public float IsAddRP
    {
        get
        {
            return addRP;
        }
        set
        {
            addRP = value;
            if(addRP <= 0)
            {
                addRP = 0;
            }
        }
    }

    /// <summary>
    /// 강화파괴 방치 플래그 변경 시
    /// </summary>
    public bool IsNoDestroy
    {
        get
        {
            return noDestroyFlag;
        }
        set
        {
            noDestroyFlag = value;
        }
    }

    // ===== 아이템 관련 기능들 (ItemPanel에서 이양) =====

    /// <summary>
    /// 아이템 활성화
    /// </summary>
    /// <param name="argItemCode">아이템 코드</param>
    public void ActiveItem(int argItemCode)
    {
        if (GameDataManager == null) return;

        var itemData = GameDataManager.GetItemData(argItemCode);
        if (itemData == null || itemData.amount <= 0 || itemData.isInUse)
        {
            GameManager.Instance?.Warning("아이템 사용이 불가능합니다.");
            return;
        }
        
        // 아이템 사용 상태로 변경
        if (!GameDataManager.SetItemUsage(argItemCode, true))
        {
            return;
        }

        // 아이템 갯수 감소
        GameDataManager.ChangeItemAmount(argItemCode, -1);
        
        // 아이템 효과 적용
        ApplyItemEffect(argItemCode);
        
        // UI 업데이트
        UpdateQuickItemUse();
        GameManager.Instance?.chestManager?.NowImfoUpdate();
        GameManager.Instance?.UiManager?.UpdateAllMainText();

        // 아이콘 생성
        CreateItemIcon(argItemCode);
    }

    /// <summary>
    /// 아이템 비활성화
    /// </summary>
    /// <param name="argItemCode">아이템 코드</param>
    public void DisableItem(int argItemCode)
    {
        if (GameDataManager == null) return;

        var itemData = GameDataManager.GetItemData(argItemCode);
        if (itemData == null || !itemData.isInUse)
        {
            GameManager.Instance?.Warning("아이템 비활성화가 불가능합니다.");
            return;
        }
        
        // 아이템 사용 해제
        if (!GameDataManager.SetItemUsage(argItemCode, false))
        {
            return;
        }

        // 아이템 갯수 복구
        GameDataManager.ChangeItemAmount(argItemCode, 1);
        
        // 아이템 효과 제거
        RemoveItemEffect(argItemCode);
        
        // UI 업데이트
        UpdateQuickItemUse();
        GameManager.Instance?.UiManager?.UpdateAllMainText();
        
        // 아이콘 제거
        RemoveItemIcon(argItemCode);
    }

    /// <summary>
    /// 아이템 효과 적용
    /// </summary>
    /// <param name="itemCode">아이템 코드</param>
    private void ApplyItemEffect(int itemCode)
    {
        var itemMasterData = GameDataManager?.GetItemMasterData(itemCode);
        if (itemMasterData == null) return;

        // 아이템별 효과 적용 로직
        switch (itemCode)
        {
            case 30000: // 예시: 경험치 증가 아이템
                // 경험치 증가 효과 적용
                ApplyExperienceBoost();
                break;
            case 30001: // 예시: 골드 증가 아이템
                // 골드 증가 효과 적용
                ApplyGoldBoost();
                break;
            case 30002: // 예시: 강화 확률 증가 아이템
                // 강화 확률 증가 효과 적용
                ApplyReinforceBoost();
                break;
            // 다른 아이템들의 효과 추가...
            default:
                Debug.Log($"아이템 효과가 정의되지 않음: {itemCode}");
                break;
        }
    }

    /// <summary>
    /// 아이템 효과 제거
    /// </summary>
    /// <param name="itemCode">아이템 코드</param>
    private void RemoveItemEffect(int itemCode)
    {
        var itemMasterData = GameDataManager?.GetItemMasterData(itemCode);
        if (itemMasterData == null) return;

        // 아이템별 효과 제거 로직
        switch (itemCode)
        {
            case 30000: // 예시: 경험치 증가 아이템
                // 경험치 증가 효과 제거
                RemoveExperienceBoost();
                break;
            case 30001: // 예시: 골드 증가 아이템
                // 골드 증가 효과 제거
                RemoveGoldBoost();
                break;
            case 30002: // 예시: 강화 확률 증가 아이템
                // 강화 확률 증가 효과 제거
                RemoveReinforceBoost();
                break;
            // 다른 아이템들의 효과 제거...
            default:
                Debug.Log($"아이템 효과 제거가 정의되지 않음: {itemCode}");
                break;
        }
    }

    /// <summary>
    /// 아이템 아이콘 생성
    /// </summary>
    /// <param name="itemCode">아이템 코드</param>
    private void CreateItemIcon(int itemCode)
    {
        var itemMasterData = GameDataManager.GetItemMasterData(itemCode);
        if (itemMasterData == null || itemIconSortPanel == null) return;

        if (icon == null) return;

        GameObject newIcon = Instantiate(icon);
        newIcon.transform.SetParent(itemIconSortPanel.transform);
        newIcon.transform.localScale = Vector3.one;
        newIcon.transform.SetAsFirstSibling();
        
        var imageComponent = newIcon.transform.Find("Image")?.GetComponent<Image>();
        if (imageComponent != null)
        {
            imageComponent.sprite = itemMasterData.image;
            imageComponent.preserveAspect = true;
        }
    }

    /// <summary>
    /// 아이템 아이콘 제거
    /// </summary>
    /// <param name="itemCode">아이템 코드</param>
    private void RemoveItemIcon(int itemCode)
    {
        if (itemIconSortPanel == null || GameDataManager == null) return;

        var usedItems = GameDataManager.GetUsedItems();
        int iconIndex = usedItems.IndexOf(itemCode);
        if (iconIndex >= 0 && iconIndex < itemIconSortPanel.transform.childCount)
        {
            Destroy(itemIconSortPanel.transform.GetChild(iconIndex).gameObject);
        }
    }

    /// <summary>
    /// 사용한 아이템과 효과 제거
    /// </summary>
    public void UseItemDestroy()
    {
        if (GameDataManager == null) return;

        var usedItems = GameDataManager.GetUsedItems();
        for(int i = usedItems.Count - 1; i >= 0; i--)
        {
            int itemCode = usedItems[i];
            RemoveItemEffect(itemCode);
            
            // 아이콘 제거
            if (itemIconSortPanel != null && 
                i < itemIconSortPanel.transform.childCount)
            {
                Destroy(itemIconSortPanel.transform.GetChild(i).gameObject);
            }
        }
        
        // 사용 중인 아이템 목록 초기화
        GameDataManager.ClearUsedItems();
    }

    /// <summary>
    /// 빠른 아이템 사용 초기화
    /// </summary>
    public void InitializeQuickItemUse()
    {
        if (GameDataManager == null) return;

        if (itemQuickUseContent == null) return;

        // 기존 퀵 아이템 목록 초기화 (필요한 경우)
        // 여기서는 프리팹에서 설정된 아이템 코드들을 사용
        for(int i = 0; i < itemQuickUseContent.transform.childCount; i++)
        {
            var quickBtn = itemQuickUseContent.transform.GetChild(i).GetComponent<QuickItemUseBtn>();
            if (quickBtn != null)
            {
                // 퀵 아이템 리스트에 추가 (중복 체크)
                GameDataManager.AddQuickUseItem(quickBtn.ownitemCode);
            }
        }
    }

    /// <summary>
    /// 게임 데이터 매니저의 빠른아이템사용 정보 참조해 빠른아이템사용 콘텐츠에 추가
    /// </summary>
    public void LoadQuickItemUse()
    {
        if (GameDataManager == null || itemQuickUseBtn == null || itemQuickUseContent == null)
            return;

        var quickUseItems = GameDataManager.GetQuickUseItems();
        for (int i = 0; i < quickUseItems.Count; i++)
        {
            int itemCode = quickUseItems[i];
            var itemMasterData = GameDataManager.GetItemMasterData(itemCode);
            if (itemMasterData == null) continue;

            GameObject btn = Instantiate(itemQuickUseBtn);
            btn.transform.SetParent(itemQuickUseContent.transform);
            btn.transform.localScale = Vector3.one;
            
            var imageComponent = btn.transform.Find("ItemImage")?.GetComponent<Image>();
            if (imageComponent != null)
            {
                imageComponent.sprite = itemMasterData.image;
            }
            
            var textComponent = btn.transform.Find("ItemText")?.GetComponent<Text>();
            if (textComponent != null)
            {
                textComponent.text = itemMasterData.name;
            }
            
            var quickBtnComponent = btn.GetComponent<QuickItemUseBtn>();
            if (quickBtnComponent != null)
            {
                quickBtnComponent.ownitemCode = itemCode;
                quickBtnComponent.ownToggle = btn.transform.Find("Toggle")?.GetComponent<Toggle>();
            }
        }
    }

    /// <summary>
    /// 빠른 아이템 사용 새로고침
    /// </summary>
    public void UpdateQuickItemUse()
    {
        if (GameDataManager == null || itemQuickUseContent == null)
            return;

        var quickUseItems = GameDataManager.GetQuickUseItems();
        for(int i = 0; i < quickUseItems.Count && i < itemQuickUseContent.transform.childCount; i++)
        {
            int itemCode = quickUseItems[i];
            var quickBtnComponent = itemQuickUseContent.transform.GetChild(i).GetComponent<QuickItemUseBtn>();
            
            if (quickBtnComponent?.ownToggle != null)
            {
                var itemData = GameDataManager.GetItemData(itemCode);
                quickBtnComponent.ownToggle.isOn = itemData?.isInUse ?? false;
            }
        }
    }

    // 구체적인 아이템 효과 메서드들
    private void ApplyExperienceBoost()
    {
        // 경험치 증가 효과 구현
        // 예: 경험치 획득량 2배
        Debug.Log("경험치 증가 효과 적용");
    }

    private void RemoveExperienceBoost()
    {
        // 경험치 증가 효과 제거
        Debug.Log("경험치 증가 효과 제거");
    }

    private void ApplyGoldBoost()
    {
        // 골드 증가 효과 구현
        // 예: 골드 획득량 1.5배
        Debug.Log("골드 증가 효과 적용");
    }

    private void RemoveGoldBoost()
    {
        // 골드 증가 효과 제거
        Debug.Log("골드 증가 효과 제거");
    }

    private void ApplyReinforceBoost()
    {
        // 강화 확률 증가 효과 구현
        IsAddRP += 10.0f; // 강화 확률 10% 증가
        Debug.Log("강화 확률 증가 효과 적용");
    }

    private void RemoveReinforceBoost()
    {
        // 강화 확률 증가 효과 제거
        IsAddRP -= 10.0f; // 강화 확률 증가 해제
        if (IsAddRP < 0) IsAddRP = 0;
        Debug.Log("강화 확률 증가 효과 제거");
    }
}
