using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolPanel : BasePanel
{
    [Header("Tool Panel UI Components")]
    [SerializeField] Image toolImage = null;
    [SerializeField] Text toolText = null;
    [SerializeField] Text percentText = null;
    [SerializeField] Text costText = null;
    [SerializeField] Text sellCostText = null;
    [SerializeField] Text modeText = null;

    [Header("Item Use Panel")]
    [SerializeField] private GameObject toolItemUsePanelPrefab = null;
    private ToolItemUsePanel toolItemUsePanel = null;

    ToolType.TYPE nowToolType = new ToolType.TYPE();
    ImproveType.TYPE nowImproveMode = new ImproveType.TYPE();
    private int nowToolCode = 0;
    private int nowToolRein = 0;
    private float reinforcePercent = 0.0f;
    private float addRP = 0.0f;
    private bool noDestroyFlag = false;
    private long sellCost = 0;
    private long reinforceCost = 0;
    private float reinforceRange = 0.0f;

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
        InitializeItemUsePanel();
        SelectToolType(ToolType.TYPE.Sword);
        ChangeMode(ImproveType.TYPE.Upgrade);
    }

    /// <summary>
    /// 아이템 사용 패널 초기화
    /// </summary>
    private void InitializeItemUsePanel()
    {
        if (toolItemUsePanelPrefab != null)
        {
            toolItemUsePanel = toolItemUsePanelPrefab.GetComponent<ToolItemUsePanel>();
            if (toolItemUsePanel == null)
            {
                Debug.LogError("ToolItemUsePanel component not found on toolItemUsePanelPrefab!");
            }
            else
            {
                // ToolPanel 자신과 GameDataManager를 전달
                toolItemUsePanel.Initialize(this, GameDataManager);
                Debug.Log("ToolItemUsePanel initialized successfully in ToolPanel!");
            }
        }
        else
        {
            Debug.LogWarning("toolItemUsePanelPrefab is not assigned in ToolPanel!");
        }
    }

    /// <summary>
    /// 강화하기 원하는 도구 타입 선택
    /// </summary>
    /// <param name="argType">도구 타입</param>
    public void SelectToolType(ToolType.TYPE argType)
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
        if(nowImproveMode == ImproveType.TYPE.Reinforce)
        {
            ChangeMode(ImproveType.TYPE.Upgrade);
        }
        else
        {
            ChangeMode(ImproveType.TYPE.Reinforce);
        }
    }

    /// <summary>
    /// 모드 변경
    /// </summary>
    /// <param name="argType">모드 타입</param>
    void ChangeMode(ImproveType.TYPE argType)
    {
        nowImproveMode = argType;

        string _string = string.Format("업그레이드");
        if (argType == ImproveType.TYPE.Reinforce)
        {
            _string = string.Format("강화");
        }

        modeText.text = string.Format(_string + " 모드");
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

        if (nowImproveMode == ImproveType.TYPE.Reinforce)
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
        if (nowImproveMode == ImproveType.TYPE.Reinforce)
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
        if (nowImproveMode == ImproveType.TYPE.Reinforce)
        {
            _string = "강화";
        }

        if (IsAddRP <= 0)
        {
            IsAddRP = 0;
            percentText.text = string.Format(_string + "확률 : {0:.0}", IsImprovePercent);
        }
        else
        {
            percentText.text = string.Format(_string + "확률 : {0:.0}", IsImprovePercent + " + " + IsAddRP);
        }

        sellCostText.text = string.Format("판매비용 : {0}", sellCost);
        costText.text = string.Format(_string + "비용 : {0}", reinforceCost);
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
        
        base.gameObject.SetActive(true);
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
    int ParseToolCode(ToolType.TYPE argType, int argClass)
    {
        int _mainCode = 0;

        switch (argType)
        {
            case ToolType.TYPE.Sword:
                _mainCode = 10000;
                break;
            case ToolType.TYPE.Spear:
                _mainCode = 11000;
                break;
            case ToolType.TYPE.Shild:
                _mainCode = 12000;
                break;
            case ToolType.TYPE.Hammer:
                _mainCode = 13000;
                break;
            case ToolType.TYPE.Sickle:
                _mainCode = 14000;
                break;
            case ToolType.TYPE.Shovle:
                _mainCode = 15000;
                break;
            case ToolType.TYPE.Axe:
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
    ToolType.TYPE ParseToolType(int argCode)
    {
        ToolType.TYPE _type = new ToolType.TYPE();

        if(argCode / 1000 == 10)
        {
            _type = ToolType.TYPE.Sword;
        }
        if (argCode / 1000 == 11)
        {
            _type = ToolType.TYPE.Spear;
        }
        if (argCode / 1000 == 12)
        {
            _type = ToolType.TYPE.Shild;
        }
        if (argCode / 1000 == 13)
        {
            _type = ToolType.TYPE.Hammer;
        }
        if (argCode / 1000 == 14)
        {
            _type = ToolType.TYPE.Sickle;
        }
        if (argCode / 1000 == 15)
        {
            _type = ToolType.TYPE.Shovle;
        }
        if (argCode / 1000 == 16)
        {
            _type = ToolType.TYPE.Axe;
        }

        return _type;
    }

    public ToolType.TYPE IsNowToolType
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
}
