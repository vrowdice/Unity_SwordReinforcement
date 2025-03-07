using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GManager : MonoBehaviour
{
    [Header("Scripts and Panel")]
    /// <summary>
    /// 도구 매니저
    /// </summary>
    public ToolManager m_toolManager = null;

    /// <summary>
    /// 아이템 매니저
    /// </summary>
    public ItemManager m_itemManager = null;

    /// <summary>
    /// 가방 매니저
    /// </summary>
    public ChestManager m_chestManager = null;

    /// <summary>
    /// 상점 매니저
    /// </summary>
    public StoreManager m_storManager = null;

    /// <summary>
    /// 갯수 설정 패널
    /// </summary>
    public GameObject m_exchangePanel = null;

    [Header("Player Imfo Viewer")]
    /// <summary>
    /// 경고 패널
    /// </summary>
    public GameObject m_warningPanel = null;

    /// <summary>
    /// 골드(캐쉬) 텍스트
    /// </summary>
    public Text m_GoldText = null;

    /// <summary>
    /// 브론즈(게임머니) 텍스트
    /// </summary>
    public Text m_BronzeText = null;

    /// <summary>
    /// 레벨 텍스트
    /// </summary>
    public Text m_levelText = null;

    /// <summary>
    /// 경험치 슬라이더
    /// </summary>
    public Slider m_expSlider = null;
    
    [Header("Exchange")]
    
    /// <summary>
    /// 자신의 정보 타입
    /// </summary>
    public int m_exchangeType = 0;

    /// <summary>
    /// 자신의 아이템 코드
    /// </summary>
    public int m_exchangeCode = 0;

    /// <summary>
    /// 구매 판매 플래그
    /// Buy = true, Sell = false
    /// </summary>
    public bool m_exchangeFlag = true;

    /// <summary>
    /// 슬라이더의 아이템 갯수 텍스트
    /// </summary>
    Text m_itemSetAmountT = null;

    /// <summary>
    /// 슬라이더의 아이템 갯수에 따른 가격 텍스트
    /// </summary>
    Text m_itemSetAmountPriceT = null;

    /// <summary>
    /// 아이템 갯수를 정하는 슬라이더
    /// </summary>
    Slider m_itemAmountSlider = null;
    
    /// <summary>
    /// 게임 매니저
    /// </summary>
    static GManager g_GameManager = null;

    public void OnEnable()
    {
        g_GameManager = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_itemSetAmountT = m_exchangePanel.transform.GetChild(0).Find("AmountText").gameObject.GetComponent<Text>();
        m_itemSetAmountPriceT = m_exchangePanel.transform.GetChild(0).Find("PriceText").gameObject.GetComponent<Text>();
        m_itemAmountSlider = m_exchangePanel.transform.GetChild(0).Find("Slider").gameObject.GetComponent<Slider>();
    }

    /// <summary>
    /// 경고 패널 띄우기
    /// </summary>
    /// <param name="argText">띄울 메세지</param>
    /// <param name="argType">적용할 타입</param>
    public void Warning(string argText)
    {
        m_warningPanel.SetActive(true);
        m_warningPanel.transform.GetChild(0).Find("Text").gameObject.GetComponent<Text>().text = string.Format(argText);
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
    
    /// <summary>
    /// 구매 시 슬라이더 셋팅
    /// </summary>
    public void BuySetSlider(int argItemCode)
    {
        m_exchangeCode = argItemCode;

        if (argItemCode <= -1)
        {
            Warning("아이탬이 선택되지 않았습니다.");
            return;
        }

        m_exchangePanel.SetActive(true);
        m_itemAmountSlider.value = 1;
        m_itemAmountSlider.maxValue = 20;
        m_exchangeFlag = true;
        UpdateSlider();
    }

    /// <summary>
    /// 판매 시 슬라이더 셋팅
    /// </summary>
    public void SellSetSlider(int argItemCode)
    {
        m_exchangeCode = argItemCode;
        if (GameDataManager.Instance.ManageItem(argItemCode).m_amount <= 0)
        {
            Warning("아이탬이 없습니다.");
            return;
        }

        if (argItemCode <= -1)
        {
            Warning("아이탬이 선택되지 않았습니다.");
            return;
        }

        gameObject.SetActive(true);
        m_itemAmountSlider.value = 1;
        m_itemAmountSlider.maxValue = 20;
        if (GameDataManager.Instance.ManageItem(argItemCode).m_amount <= m_itemAmountSlider.maxValue)
        {
            m_itemAmountSlider.maxValue = GameDataManager.Instance.ManageItem(argItemCode).m_amount;
        }
        m_exchangeFlag = false;
        UpdateSlider();
    }

    /// <summary>
    /// 슬라이더 변경 시
    /// </summary>
    public void UpdateSlider()
    {
        if (m_exchangeFlag == true)
        {
            if (m_itemAmountSlider.value <= 0)
            {
                m_itemAmountSlider.value = 1;
            }
            m_itemSetAmountT.text = string.Format("{0} / {1}", m_itemAmountSlider.value, m_itemAmountSlider.maxValue);
            m_itemSetAmountPriceT.text = string.Format("{0}$", m_itemAmountSlider.value * GameDataManager.Instance.m_itemDic[m_exchangeCode].m_price);
        }
        else
        {
            if (m_itemAmountSlider.value <= 0)
            {
                m_itemAmountSlider.value = 1;
            }
            if (GameDataManager.Instance.ManageItem(m_exchangeCode).m_amount <= m_itemAmountSlider.maxValue)
            {
                m_itemSetAmountT.text = string.Format("{0} / {1}", m_itemAmountSlider.value, GameDataManager.Instance.m_itemDic[m_exchangeCode]);
            }
            else
            {
                m_itemSetAmountT.text = string.Format("{0} / {1}", m_itemAmountSlider.value, m_itemAmountSlider.maxValue);
            }
            m_itemSetAmountPriceT.text = string.Format("{0}$", m_itemAmountSlider.value * GameDataManager.Instance.m_itemDic[m_exchangeCode].m_price * 7 / 10);
        }

    }

    /// <summary>
    /// 확인 클릭 시
    /// </summary>
    public void ClickConfirm()
    {
        if (m_exchangeFlag == true)
        {
            m_itemManager.BuyItem(m_exchangeCode, (int)m_itemAmountSlider.value);
        }
        else
        {
            m_itemManager.SellItem(m_exchangeCode, (int)m_itemAmountSlider.value);
        }
        m_exchangePanel.SetActive(false);
    }

    /// <summary>
    /// 인스턴스
    /// </summary>
    public static GManager Instance
    {
        get
        {
            return g_GameManager;
        }
    }
}
