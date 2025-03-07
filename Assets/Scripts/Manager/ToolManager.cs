using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolManager : MonoBehaviour
{
    /// <summary>
    /// 매인패널
    /// </summary>
    public GameObject m_toolPanel = null;

    /// <summary>
    /// 도구 패널
    /// </summary>
    public GameObject m_chestToolPanel = null;

    /// <summary>
    /// 개선 버튼
    /// </summary>
    GameObject m_improveBtn = null;

    /// <summary>
    /// 검 이미지
    /// </summary>
    Image m_toolImage = null;

    /// <summary>
    /// 검 텍스트
    /// </summary>
    Text m_toolText = null;

    /// <summary>
    /// 판매비용 택스트
    /// </summary>
    Text m_sellCostTxt = null;

    /// <summary>
    /// 강화 비용
    /// </summary>
    Text m_reinforceCostTxt = null;

    /// <summary>
    /// 강화 확률
    /// </summary>
    Text m_reinforcePercentTxt = null;

    /// <summary>
    /// 현재 모드 텍스트
    /// </summary>
    Text m_modeText = null;

    /// <summary>
    /// 상자 패널의 검 이미지
    /// </summary>
    Image m_chestToolImg;

    /// <summary>
    /// 상자 패널의 검 설명
    /// </summary>
    Text m_chestToolExp;

    /// <summary>
    /// 현재 툴 타입
    /// </summary>
    StuffType.ToolType m_nowToolType = new StuffType.ToolType();

    /// <summary>
    /// 현재 도구 패널 모드
    /// </summary>
    PanelType.ImproveType m_nowImproveMode = new PanelType.ImproveType();

    /// <summary>
    /// 현재 강화횟수
    /// </summary>
    private int m_nowToolCode = 0;

    /// <summary>
    /// 현재 툴 강화횟수
    /// </summary>
    private int m_nowToolRein = 0;

    /// <summary>
    /// 현재 선택한 툴 코드
    /// </summary>
    int m_nowChestToolCode = 0;

    /// <summary>
    /// 현재 선택한 툴 강화횟수
    /// </summary>
    int m_nowChestToolRein = 0;

    /// <summary>
    /// 강화 확률
    /// </summary>
    float m_reinforcePercent = 0.0f;

    /// <summary>
    /// 추가될 강화 확률
    /// </summary>
    float m_addRP = 0.0f;

    /// <summary>
    /// 검 파괴방지 플래그
    /// </summary>
    bool m_noDestroyFlag = false;

    /// <summary>
    /// 판매 비용
    /// </summary>
    long m_sellCost = 0;

    /// <summary>
    /// 강화 비용
    /// </summary>
    long m_reinforceCost = 0;

    /// <summary>
    /// 무작위 값을 산출할 볌위
    /// </summary>
    public float m_reinforceRange = 0.0f;

    private void Start()
    {
        Setting();
    }

    /// <summary>
    /// 초기 셋팅
    /// </summary>
    void Setting()
    {
        m_toolImage = m_toolPanel.transform.Find("ToolImage").gameObject.GetComponent<Image>();
        m_modeText = m_toolPanel.transform.Find("ModeText").gameObject.GetComponent<Text>();
        m_toolText = m_toolPanel.transform.Find("ToolText").gameObject.GetComponent<Text>();
        m_sellCostTxt = m_toolPanel.transform.Find("SellCostText").gameObject.GetComponent<Text>();
        m_reinforceCostTxt = m_toolPanel.transform.Find("CostText").gameObject.GetComponent<Text>();
        m_reinforcePercentTxt = m_toolPanel.transform.Find("PercentText").gameObject.GetComponent<Text>();
        m_modeText = m_toolPanel.transform.Find("ModeText").gameObject.GetComponent<Text>();
        m_improveBtn = m_toolPanel.transform.Find("ImproveBtn").gameObject;

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
        if(m_nowImproveMode == PanelType.ImproveType.Reinforce)
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
        m_nowImproveMode = argType;

        string _string = string.Format("업그레이드");
        if (argType == PanelType.ImproveType.Reinforce)
        {
            _string = string.Format("강화");
        }

        m_modeText.text = string.Format(_string + " 모드");
        m_improveBtn.transform.GetChild(0).gameObject.GetComponent<Text>().text = string.Format(_string + "하기");
    }

    /// <summary>
    /// 툴 개선
    /// </summary>
    public void ImproveTool()
    {
        ToolData _data = null;
        GameDataManager.Instance.m_toolDic.TryGetValue(IsNowToolCode, out _data);
        if (_data == null)
        {
            GManager.Instance.Warning("다음 도구가 없습니다.");
            return;
        }

        if (m_nowImproveMode == PanelType.ImproveType.Reinforce)
        {
            if (CheckMainCode(IsNowToolCode))
            {
                GManager.Instance.Warning("베이스 도구는 강화가 불가능합니다.");
                return;
            }

            ReinforceTool(IsNowToolCode, IsNowReinCount);

            IsImprovePercent = Random.Range(0, m_reinforceRange);
            if (IsImprovePercent + IsAddRP <= GameDataManager.Instance.m_toolPercentDic[ParseToolType(IsNowToolCode)].m_reinforcePercent[IsNowReinCount])
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

            IsImprovePercent = Random.Range(0, m_reinforceRange);
            if (IsImprovePercent + IsAddRP <= GameDataManager.Instance.m_toolPercentDic[ParseToolType(IsNowToolCode)].m_UpgradePercent[IsNowToolCode % 1000])
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
            GManager.Instance.Warning("창고에 넣을 수 없습니다.");
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
            GManager.Instance.Warning("판매가 불가능합니다.");
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
        GameDataManager.Instance.IsBronze += _tmp;
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
        GameDataManager.Instance.IsBronze -= _tmp;
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
        GameDataManager.Instance.IsBronze -= _tmp;
        return true;
    }
    
    /// <summary>
    /// 강화비용 텍스트와 확률 텍스트, 판매비용 텍스트 변경
    /// </summary>
    /// <param name="argRein">강화 횟수</param>
    public void UpdateToolImfo()
    {
        ToolData _data = null;
        _data = GameDataManager.Instance.m_toolDic[IsNowToolCode];

        m_toolText.text = _data.m_name + " +" + IsNowReinCount;
        m_toolImage.sprite = _data.m_image;

        long _reinCost = PriceTool(IsNowToolCode, IsNowReinCount);
        IsImproveCost = _reinCost * 3 / 10;
        IsSellCost = _reinCost;
        if (m_nowImproveMode == PanelType.ImproveType.Reinforce)
        {
            IsImprovePercent = GameDataManager.Instance.m_toolPercentDic[ParseToolType(IsNowToolCode)].m_reinforcePercent[IsNowReinCount];
        }
        else
        {
            IsImprovePercent = GameDataManager.Instance.m_toolPercentDic[ParseToolType(IsNowToolCode)].m_UpgradePercent[IsNowToolCode % 1000];
        }
        UpdateToolText();
    }

    /// <summary>
    /// 도구 모두 초기화
    /// </summary>
    void ResetNowToolImfo()
    {
        int _mainCode = ParseToolCode(ParseToolType(m_nowToolCode), 0);
        m_toolImage.sprite = GameDataManager.Instance.m_toolDic[_mainCode].m_image;
        m_toolText.text = GameDataManager.Instance.m_toolDic[_mainCode].m_name;

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
        if (m_nowImproveMode == PanelType.ImproveType.Reinforce)
        {
            _string = "강화";
        }

        if (IsAddRP <= 0)
        {
            IsAddRP = 0;
            m_reinforcePercentTxt.text = string.Format(_string + "확률 : {0:.0}", IsImprovePercent);
        }
        else
        {
            m_reinforcePercentTxt.text = string.Format(_string + "확률 : {0:.0}", IsImprovePercent + " + " + IsAddRP);
        }

        m_sellCostTxt.text = string.Format("판매비용 : {0}", m_sellCost);
        m_reinforceCostTxt.text = string.Format(_string + "비용 : {0}", m_reinforceCost);
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
        if (GameDataManager.Instance.m_toolDic[argRein] == null)
        {
            return;
        }
        
        m_nowChestToolRein = argRein;

        gameObject.SetActive(true);
        m_chestToolImg.sprite = GameDataManager.Instance.m_toolDic[m_nowChestToolCode].m_image;
        m_chestToolImg.GetComponent<Image>().preserveAspect = true;
        m_chestToolExp.GetComponentInChildren<Text>().text = GameDataManager.Instance.m_toolDic[m_nowChestToolCode].m_explanation;
    }

    /// <summary>
    /// 도구 도매가 판별
    /// </summary>
    /// <param name="argCode">툴 코드</param>
    /// <param name="argRein">강화 횟수 </param>
    /// <returns>도구 가격</returns>
    public long PriceTool(int argCode, int argRein)
    {
        long _price = GameDataManager.Instance.m_toolDic[argCode].m_price;
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
            return m_nowToolType;
        }
        set
        {
            m_nowToolType = value;
        }
    }

    /// <summary>
    /// 현재 업그레이드 횟수 변경
    /// </summary>
    public int IsNowToolCode
    {
        get
        {
            return m_nowToolCode;
        }
        set
        {
            m_nowToolCode = value;
        }
    }
    
    /// <summary>
    /// 현재 강화 횟수 변경
    /// </summary>
    public int IsNowReinCount
    {
        get
        {
            return m_nowToolRein;
        }
        set
        {
            m_nowToolRein = value;
        }
    }

    /// <summary>
    /// 판매 가격 변경
    /// </summary>
    public long IsSellCost
    {
        get
        {
            return m_sellCost;
        }
        set
        {
            m_sellCost = value;
            if (m_sellCost <= 0)
            {
                m_sellCost = 0;
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
            return m_reinforceCost;
        }
        set
        {
            m_reinforceCost = value;
            if (m_reinforceCost <= 0)
            {
                m_reinforceCost = 0;
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
            return m_reinforcePercent;
        }
        set
        {
            m_reinforcePercent = value;
            if (m_reinforcePercent >= m_reinforceRange)
            {
                m_reinforcePercent = m_reinforceRange;
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
            return m_addRP;
        }
        set
        {
            m_addRP = value;
            if(m_addRP <= 0)
            {
                m_addRP = 0;
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
            return m_noDestroyFlag;
        }
        set
        {
            m_noDestroyFlag = value;
        }
    }
}
