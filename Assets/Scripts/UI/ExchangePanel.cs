using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExchangePanel : MonoBehaviour
{
    [Header("Exchange Panel UI")]
    [SerializeField] private GameObject exchangePanel = null;
    [SerializeField] private Text itemSetAmountT = null;
    [SerializeField] private Text itemSetAmountPriceT = null;
    [SerializeField] private Slider itemAmountSlider = null;

    [Header("Exchange Settings")]
    [SerializeField] private int exchangeType = 0;
    [SerializeField] private int exchangeCode = 0;
    [SerializeField] private bool exchangeFlag = true; // Buy = true, Sell = false

    /// <summary>
    /// 부모 StorePanel 참조
    /// </summary>
    private StorePanel parentStorePanel = null;

    /// <summary>
    /// Exchange 패널 초기화
    /// </summary>
    public void Initialize(StorePanel storePanel = null)
    {
        // 슬라이더 이벤트 연결
        if (itemAmountSlider != null)
        {
            itemAmountSlider.onValueChanged.AddListener(OnSliderValueChanged);
        }
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
    /// 슬라이더 값 변경 이벤트
    /// </summary>
    private void OnSliderValueChanged(float value)
    {
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
        if (itemAmountSlider == null) return;

        if (exchangeFlag == true)
        {
            // StorePanel의 BuyItem 메서드 직접 호출
            if (parentStorePanel != null)
            {
                parentStorePanel.BuyItem(exchangeCode, (int)itemAmountSlider.value);
            }
            else
            {
                Debug.LogError("Parent StorePanel not found!");
            }
        }
        else
        {
            // StorePanel의 SellItem 메서드 직접 호출
            if (parentStorePanel != null)
            {
                parentStorePanel.SellItem(exchangeCode, (int)itemAmountSlider.value);
            }
            else
            {
                Debug.LogError("Parent StorePanel not found!");
            }
        }
        
        if (exchangePanel != null)
        {
            exchangePanel.SetActive(false);
        }
    }

    /// <summary>
    /// 패널 닫기
    /// </summary>
    public void ClosePanel()
    {
        if (exchangePanel != null)
        {
            exchangePanel.SetActive(false);
        }
    }

    void Start()
    {
        // 부모 StorePanel 자동 찾기
        if (parentStorePanel == null)
        {
            parentStorePanel = GetComponentInParent<StorePanel>();
        }
        
        Initialize(parentStorePanel);
    }
}
   