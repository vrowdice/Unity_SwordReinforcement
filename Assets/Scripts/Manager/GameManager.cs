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
    /// 갯수 설정 패널
    /// </summary>
    public GameObject exchangePanel = null;

    [Header("Player Imfo Viewer")]
    /// <summary>
    /// 경고 패널
    /// </summary>
    public GameObject warningPanel = null;

    /// <summary>
    /// 골드(캐쉬) 텍스트
    /// </summary>
    public Text GoldText = null;

    /// <summary>
    /// 브론즈(게임머니) 텍스트
    /// </summary>
    public Text BronzeText = null;

    /// <summary>
    /// 레벨 텍스트
    /// </summary>
    public Text levelText = null;

    /// <summary>
    /// 경험치 슬라이더
    /// </summary>
    public Slider expSlider = null;
    
    [Header("Exchange")]
    
    /// <summary>
    /// 자신의 정보 타입
    /// </summary>
    public int exchangeType = 0;

    /// <summary>
    /// 자신의 아이템 코드
    /// </summary>
    public int exchangeCode = 0;

    /// <summary>
    /// 구매 판매 플래그
    /// Buy = true, Sell = false
    /// </summary>
    public bool exchangeFlag = true;

    /// <summary>
    /// 슬라이더의 아이템 갯수 텍스트
    /// </summary>
    Text itemSetAmountT = null;

    /// <summary>
    /// 슬라이더의 아이템 갯수에 따른 가격 텍스트
    /// </summary>
    Text itemSetAmountPriceT = null;

    /// <summary>
    /// 아이템 갯수를 정하는 슬라이더
    /// </summary>
    Slider itemAmountSlider = null;
    
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
        itemSetAmountT = exchangePanel.transform.GetChild(0).Find("AmountText").gameObject.GetComponent<Text>();
        itemSetAmountPriceT = exchangePanel.transform.GetChild(0).Find("PriceText").gameObject.GetComponent<Text>();
        itemAmountSlider = exchangePanel.transform.GetChild(0).Find("Slider").gameObject.GetComponent<Slider>();
    }

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
    
    /// <summary>
    /// 구매 시 슬라이더 셋팅
    /// </summary>
    public void BuySetSlider(int argItemCode)
    {
        exchangeCode = argItemCode;

        if (argItemCode <= -1)
        {
            Warning("아이템이 선택되지 않았습니다.");
            return;
        }

        exchangePanel.SetActive(true);
        itemAmountSlider.value = 1;
        itemAmountSlider.maxValue = 20;
        exchangeFlag = true;
        UpdateSlider();
    }

    /// <summary>
    /// 판매 시 슬라이더 셋팅
    /// </summary>
    public void SellSetSlider(int argItemCode)
    {
        exchangeCode = argItemCode;
        var itemData = GameDataManager.Instance.GetItemData(argItemCode);
        if (itemData == null || itemData.amount <= 0)
        {
            Warning("아이템이 없습니다.");
            return;
        }

        if (argItemCode <= -1)
        {
            Warning("아이템이 선택되지 않았습니다.");
            return;
        }

        exchangePanel.SetActive(true);
        itemAmountSlider.value = 1;
        itemAmountSlider.maxValue = 20;
        if (itemData.amount <= itemAmountSlider.maxValue)
        {
            itemAmountSlider.maxValue = itemData.amount;
        }
        exchangeFlag = false;
        UpdateSlider();
    }

    /// <summary>
    /// 슬라이더 변경 시
    /// </summary>
    public void UpdateSlider()
    {
        if (exchangeFlag == true)
        {
            if (itemAmountSlider.value <= 0)
            {
                itemAmountSlider.value = 1;
            }
            itemSetAmountT.text = string.Format("{0} / {1}", itemAmountSlider.value, itemAmountSlider.maxValue);
            var itemData = GameDataManager.Instance.GetItemMasterData(exchangeCode);
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
            var userItemData = GameDataManager.Instance.GetItemData(exchangeCode);
            var itemMasterData = GameDataManager.Instance.GetItemMasterData(exchangeCode);
            
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
        if (exchangeFlag == true)
        {
            storManager.BuyItem(exchangeCode, (int)itemAmountSlider.value);
        }
        else
        {
            storManager.SellItem(exchangeCode, (int)itemAmountSlider.value);
        }
        exchangePanel.SetActive(false);
    }

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
