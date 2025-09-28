using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StorePanel : BasePanel
{
    /// <summary>
    /// 상점 콘텐츠들
    /// </summary>
    public GameObject[] storContents = null;

    /// <summary>
    /// 아이템 패널
    /// </summary>
    public GameObject itemBuySellPanel = null;

    /// <summary>
    /// 상점 아이템 버튼
    /// </summary>
    public GameObject storBtn = null;

    /// <summary>
    /// 자신의 아이템 코드
    /// </summary>
    int nowItemCode = -1;

    /// <summary>
    /// 상점 스크롤 뷰
    /// </summary>
    GameObject[] storScrollView = null;

    /// <summary>
    /// 상점 콘텐츠 선택 버튼
    /// </summary>
    [SerializeField]
    GameObject[] storSelectBtn = null;

    /// <summary>
    /// 아이템 이미지
    /// </summary>
    Image itemImage = null;

    /// <summary>
    /// 아이템 이름 텍스트
    /// </summary>
    Text itemName = null;

    /// <summary>
    /// 아이템 설명 텍스트
    /// </summary>
    Text itemExplain = null;

    /// <summary>
    /// 아이템 가격 텍스트
    /// </summary>
    Text itemPrice = null;

    /// <summary>
    /// 아이템 갯수 텍스트
    /// </summary>
    Text itemAmount = null;

    protected override void OnPanelOpen()
    {
        base.OnPanelOpen();
        
        Setting();
    }

    void Setting()
    {
        if (GameDataManager == null)
        {
            Debug.LogError("GameDataManager not found!");
            return;
        }

        InitializeArrays();
        InitializeUIComponents();
        InitializeScrollViews();
        
        nowItemCode = 30000; // 기본 아이템 코드
        
        UpdateBuySellPanel(nowItemCode);
        UpdateStorItem();
        ChangeStorView(0);
    }

    /// <summary>
    /// 배열 초기화
    /// </summary>
    private void InitializeArrays()
    {
        storSelectBtn = new GameObject[3];
        storScrollView = new GameObject[3];
    }

    /// <summary>
    /// UI 컴포넌트 초기화
    /// </summary>
    private void InitializeUIComponents()
    {
        if (itemBuySellPanel == null)
        {
            Debug.LogError("Item buy/sell panel is not assigned!");
            return;
        }

        itemImage = itemBuySellPanel.transform.Find("Image")?.GetComponent<Image>();
        itemName = itemBuySellPanel.transform.Find("NameText")?.GetComponent<Text>();
        itemExplain = itemBuySellPanel.transform.Find("ExplainText")?.GetComponent<Text>();
        itemPrice = itemBuySellPanel.transform.Find("PriceText")?.GetComponent<Text>();
        itemAmount = itemBuySellPanel.transform.Find("AmountText")?.GetComponent<Text>();
    }

    /// <summary>
    /// 스크롤 뷰 초기화
    /// </summary>
    private void InitializeScrollViews()
    {
        storScrollView[0] = transform.Find("Scroll View Stor Disposable")?.gameObject;
        storScrollView[1] = transform.Find("Scroll View Stor Last")?.gameObject;
        storScrollView[2] = transform.Find("Scroll View Stor Cash")?.gameObject;

        // null 체크
        for (int i = 0; i < storScrollView.Length; i++)
        {
            if (storScrollView[i] == null)
            {
                Debug.LogError($"Store scroll view {i} not found!");
            }
        }
    }

    /// <summary>
    /// 버튼 클릭 시 메인패널의 이미지와 설명 변경 및 구매버튼 인계
    /// </summary>
    public void UpdateBuySellPanel(int argItemCode)
    {
        if (GameDataManager == null)
        {
            Debug.LogError("GameDataManager is null!");
            return;
        }

        nowItemCode = argItemCode;

        // 아이템 마스터 데이터 조회
        var itemMasterData = GameDataManager.GetItemMasterData(argItemCode);
        if (itemMasterData == null)
        {
            Debug.LogError($"Item master data not found for code: {argItemCode}");
            return;
        }

        // 유저 아이템 데이터 조회
        var userItemData = GameDataManager.GetItemData(argItemCode);
        int itemAmount = userItemData?.amount ?? 0;

        // UI 업데이트
        UpdateItemDisplay(itemMasterData, itemAmount);
    }

    /// <summary>
    /// 아이템 디스플레이 업데이트
    /// </summary>
    /// <param name="itemData">아이템 마스터 데이터</param>
    /// <param name="amount">보유 수량</param>
    private void UpdateItemDisplay(ItemData itemData, int amount)
    {
        if (itemImage != null)
        {
            itemImage.sprite = itemData.image;
            itemImage.preserveAspect = true;
        }

        if (itemName != null)
            itemName.text = itemData.name;

        if (itemExplain != null)
            itemExplain.text = itemData.explanation;

        if (itemPrice != null)
            itemPrice.text = $"가격 : {itemData.price:#,###}";

        if (itemAmount != null)
        {
            if (amount <= 0)
            {
                itemAmount.text = "0 개";
            }
            else
            {
                itemAmount.text = $"{amount:#,###} 개";
            }
        }
    }

    /// <summary>
    /// 상점 아이템 업데이트
    /// </summary>
    void UpdateStorItem()
    {
        if (GameDataManager == null || storBtn == null || storContents == null)
        {
            Debug.LogError("Required components not found for UpdateStorItem!");
            return;
        }

        // 기존 버튼들 정리
        ClearExistingButtons();

        int[] counts = new int[3]; // 각 카테고리별 아이템 수

        // 모든 아이템 데이터를 순회하면서 상점 버튼 생성
        var allItems = GetAllItemData();
        foreach (var itemPair in allItems)
        {
            int itemCode = itemPair.Key;
            ItemData itemData = itemPair.Value;

 /*           int categoryIndex = GetItemCategoryIndex(itemData);
            if (categoryIndex >= 0 && categoryIndex < storContents.Length)
            {
                CreateStoreButton(itemCode, itemData, categoryIndex);
                counts[categoryIndex]++;
            }*/
        }

        Debug.Log($"Store items created - Disposable: {counts[0]}, Lasting: {counts[1]}, Cash: {counts[2]}");
    }

    /// <summary>
    /// 기존 상점 버튼들 정리
    /// </summary>
    private void ClearExistingButtons()
    {
        for (int i = 0; i < storContents.Length; i++)
        {
            if (storContents[i] != null)
            {
                // 기존 자식 오브젝트들 제거 (런타임에서 안전하게)
                for (int j = storContents[i].transform.childCount - 1; j >= 0; j--)
                {
                    var child = storContents[i].transform.GetChild(j);
                    if (Application.isPlaying)
                    {
                        Destroy(child.gameObject);
                    }
                    else
                    {
                        DestroyImmediate(child.gameObject);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 모든 아이템 데이터 조회
    /// </summary>
    /// <returns>아이템 코드와 데이터의 딕셔너리</returns>
    private Dictionary<int, ItemData> GetAllItemData()
    {
        return GameDataManager.GetAllItemMasterData();
    }

    /// <summary>
    /// 아이템 카테고리 인덱스 조회
    /// </summary>
    /// <param name="itemData">아이템 데이터</param>
    /// <returns>카테고리 인덱스 (0: 일회용, 1: 지속형, 2: 캐시)</returns>
/*    private int GetItemCategoryIndex(ItemData itemData)
    {
        if (itemData.valueType == StuffType.ItemBuyType.Csh)
        {
            return 2; // 캐시 아이템
        }
        else if (itemData.useType == StuffType.ItemUseType.One)
        {
            return 0; // 일회용 아이템
        }
        else
        {
            return 1; // 지속형 아이템
        }
    }*/

    /// <summary>
    /// 상점 버튼 생성
    /// </summary>
    /// <param name="itemCode">아이템 코드</param>
    /// <param name="itemData">아이템 데이터</param>
    /// <param name="categoryIndex">카테고리 인덱스</param>
    private void CreateStoreButton(int itemCode, ItemData itemData, int categoryIndex)
    {
        if (categoryIndex < 0 || categoryIndex >= storContents.Length || storContents[categoryIndex] == null)
        {
            Debug.LogError($"Invalid category index or null content: {categoryIndex}");
            return;
        }

        GameObject btn = Instantiate(storBtn);
        btn.transform.SetParent(storContents[categoryIndex].transform);
        btn.transform.localScale = Vector3.one;

        // 버튼 UI 설정
        SetupButtonUI(btn, itemData);

        // 버튼 클릭 이벤트 설정
        var button = btn.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => UpdateBuySellPanel(itemCode));
        }
    }

    /// <summary>
    /// 버튼 UI 설정
    /// </summary>
    /// <param name="btn">버튼 게임오브젝트</param>
    /// <param name="itemData">아이템 데이터</param>
    private void SetupButtonUI(GameObject btn, ItemData itemData)
    {
        // 이름 텍스트 설정
        var nameText = btn.transform.Find("NameText")?.GetComponent<Text>();
        if (nameText != null)
            nameText.text = itemData.name;

        // 설명 텍스트 설정
        var explainText = btn.transform.Find("ExplainText")?.GetComponent<Text>();
        if (explainText != null)
            explainText.text = itemData.explanation;

        // 이미지 설정
        var image = btn.transform.Find("Image")?.GetComponent<Image>();
        if (image != null)
        {
            image.sprite = itemData.image;
            image.preserveAspect = true;
        }
    }

    /// <summary>
    /// 아이템 구매
    /// </summary>
    public void BuyItem()
    {
        if (nowItemCode <= -1)
        {
            Debug.LogWarning("No item selected for purchase!");
            return;
        }

        GameManager.Instance?.UiManager?.BuySetSlider(nowItemCode);
    }

    /// <summary>
    /// 아이템 구매 (직접 호출용)
    /// </summary>
    /// <param name="argItemCode">아이템 코드</param>
    /// <param name="argAmount">구매 수량</param>
    public void BuyItem(int argItemCode, int argAmount)
    {
        if (argItemCode <= -1)
        {
            GameManager.Instance?.Warning("아이템이 선택되지 않았습니다.");
            return;
        }

        if (GameDataManager == null) return;
        
        var itemMasterData = GameDataManager.GetItemMasterData(argItemCode);
        if (itemMasterData == null)
        {
            GameManager.Instance?.Warning("존재하지 않는 아이템입니다.");
            return;
        }

        long totalPrice = (long)itemMasterData.price * argAmount;
        
        if (!GameDataManager.TrySpendBronze(totalPrice))
        {
            return; // 에러 메시지는 TrySpendBronze에서 처리
        }

        GameDataManager.ChangeItemAmount(argItemCode, argAmount);
        UpdateBuySellPanel(argItemCode);
        
        // UI 업데이트
        GameManager.Instance?.UiManager?.UpdateAllMainText();
    }

    /// <summary>
    /// 아이템 판매
    /// </summary>
    public void SellItem()
    {
        if (nowItemCode <= -1)
        {
            Debug.LogWarning("No item selected for sale!");
            return;
        }

        GameManager.Instance?.UiManager?.SellSetSlider(nowItemCode);
    }

    /// <summary>
    /// 아이템 판매 (직접 호출용)
    /// </summary>
    /// <param name="argItemCode">아이템 코드</param>
    /// <param name="argAmount">판매 수량</param>
    public void SellItem(int argItemCode, int argAmount)
    {
        if (argItemCode <= -1)
        {
            GameManager.Instance?.Warning("아이템이 선택되지 않았습니다.");
            return;
        }

        if (GameDataManager == null) return;

        var itemData = GameDataManager.GetItemData(argItemCode);
        if (itemData == null || itemData.amount <= 0)
        {
            GameManager.Instance?.Warning("판매할 아이템이 없습니다.");
            return;
        }

        if (itemData.amount < argAmount)
        {
            GameManager.Instance?.Warning("보유한 아이템보다 많이 판매할 수 없습니다.");
            return;
        }

        var itemMasterData = GameDataManager.GetItemMasterData(argItemCode);
        if (itemMasterData == null) return;

        // 판매가는 구매가의 70%
        long sellPrice = (long)(itemMasterData.price * argAmount * 0.7f);
        
        GameDataManager.AddBronze(sellPrice);
        GameDataManager.ChangeItemAmount(argItemCode, -argAmount);
        
        UpdateBuySellPanel(argItemCode);
        
        // UI 업데이트
        GameManager.Instance?.UiManager?.UpdateAllMainText();
    }

    /// <summary>
    /// 상점 스크롤 뷰 변경
    /// </summary>
    /// <param name="argIndex">아이템 정보 인덱스</param>
    public void ChangeStorView(int argIndex)
    {
        if (storScrollView == null || argIndex < 0 || argIndex >= storScrollView.Length)
        {
            Debug.LogError($"Invalid store view index: {argIndex}");
            return;
        }

        // 모든 스크롤 뷰 비활성화
        for (int i = 0; i < storScrollView.Length; i++)
        {
            if (storScrollView[i] != null)
            {
                storScrollView[i].SetActive(false);
            }
        }

        // 선택된 스크롤 뷰 활성화
        if (storScrollView[argIndex] != null)
        {
            storScrollView[argIndex].SetActive(true);
        }
        else
        {
            Debug.LogError($"Store scroll view at index {argIndex} is null!");
        }
    }
}
