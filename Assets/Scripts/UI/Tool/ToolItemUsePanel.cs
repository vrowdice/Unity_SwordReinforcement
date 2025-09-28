using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolItemUsePanel : MonoBehaviour
{
    [Header("Item Quick Use UI")]
    [SerializeField] private GameObject icon = null;
    [SerializeField] private GameObject itemIconSortPanel = null;
    [SerializeField] private GameObject itemQuickUseBtn = null;
    [SerializeField] private GameObject itemQuickUseContent = null;

    /// <summary>
    /// 부모 ToolPanel 참조
    /// </summary>
    private ToolPanel parentToolPanel = null;
    private GameDataManager gameDataManager = null;

    /// <summary>
    /// 아이템 사용 패널 초기화
    /// </summary>
    public void Initialize(ToolPanel toolPanel, GameDataManager argGameDataManager)
    {
        parentToolPanel = toolPanel;
        gameDataManager = argGameDataManager;

        if (gameDataManager == null) return;

        InitializeQuickItemUse();
        LoadQuickItemUse();
    }

    /// <summary>
    /// 아이템 활성화
    /// </summary>
    /// <param name="argItemCode">아이템 코드</param>
    public void ActiveItem(int argItemCode)
    {
        if (gameDataManager == null) return;

        var itemData = gameDataManager.GetItemData(argItemCode);
        if (itemData == null || itemData.amount <= 0 || itemData.isInUse)
        {
            GameManager.Instance?.Warning("아이템 사용이 불가능합니다.");
            return;
        }
        
        // 아이템 사용 상태로 변경
        if (!gameDataManager.SetItemUsage(argItemCode, true))
        {
            return;
        }

        // 아이템 갯수 감소
        gameDataManager.ChangeItemAmount(argItemCode, -1);
        
        // 아이템 효과 적용
        ApplyItemEffect(argItemCode);
        
        // UI 업데이트
        UpdateQuickItemUse();
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
        if (gameDataManager == null) return;

        var itemData = gameDataManager.GetItemData(argItemCode);
        if (itemData == null || !itemData.isInUse)
        {
            GameManager.Instance?.Warning("아이템 비활성화가 불가능합니다.");
            return;
        }
        
        // 아이템 사용 해제
        if (!gameDataManager.SetItemUsage(argItemCode, false))
        {
            return;
        }

        // 아이템 갯수 복구
        gameDataManager.ChangeItemAmount(argItemCode, 1);
        
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
        var itemMasterData = gameDataManager?.GetItemMasterData(itemCode);
        if (itemMasterData == null) return;

        // 아이템별 효과 적용 로직
        switch (itemCode)
        {
            case 30000: // 예시: 경험치 증가 아이템
                ApplyExperienceBoost();
                break;
            case 30001: // 예시: 골드 증가 아이템
                ApplyGoldBoost();
                break;
            case 30002: // 예시: 강화 확률 증가 아이템
                ApplyReinforceBoost();
                break;
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
        var itemMasterData = gameDataManager?.GetItemMasterData(itemCode);
        if (itemMasterData == null) return;

        // 아이템별 효과 제거 로직
        switch (itemCode)
        {
            case 30000: // 예시: 경험치 증가 아이템
                RemoveExperienceBoost();
                break;
            case 30001: // 예시: 골드 증가 아이템
                RemoveGoldBoost();
                break;
            case 30002: // 예시: 강화 확률 증가 아이템
                RemoveReinforceBoost();
                break;
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
        var itemMasterData = gameDataManager.GetItemMasterData(itemCode);
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
        if (itemIconSortPanel == null || gameDataManager == null) return;

        var usedItems = gameDataManager.GetUsedItems();
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
        if (gameDataManager == null) return;

        var usedItems = gameDataManager.GetUsedItems();
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
        gameDataManager.ClearUsedItems();
    }

    /// <summary>
    /// 빠른 아이템 사용 초기화
    /// </summary>
    public void InitializeQuickItemUse()
    {
        if (gameDataManager == null) return;

        if (itemQuickUseContent == null) return;

        // 기존 퀵 아이템 목록 초기화 (필요한 경우)
        for(int i = 0; i < itemQuickUseContent.transform.childCount; i++)
        {
            var quickBtn = itemQuickUseContent.transform.GetChild(i).GetComponent<QuickItemUseBtn>();
            if (quickBtn != null)
            {
                // 퀵 아이템 리스트에 추가 (중복 체크)
                gameDataManager.AddQuickUseItem(quickBtn.ownitemCode);
            }
        }
    }

    /// <summary>
    /// 게임 데이터 매니저의 빠른아이템사용 정보 참조해 빠른아이템사용 콘텐츠에 추가
    /// </summary>
    public void LoadQuickItemUse()
    {
        if (gameDataManager == null || itemQuickUseBtn == null || itemQuickUseContent == null)
            return;

        var quickUseItems = gameDataManager.GetQuickUseItems();
        for (int i = 0; i < quickUseItems.Count; i++)
        {
            int itemCode = quickUseItems[i];
            var itemMasterData = gameDataManager.GetItemMasterData(itemCode);
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
        if (gameDataManager == null || itemQuickUseContent == null)
            return;

        var quickUseItems = gameDataManager.GetQuickUseItems();
        for(int i = 0; i < quickUseItems.Count && i < itemQuickUseContent.transform.childCount; i++)
        {
            int itemCode = quickUseItems[i];
            var quickBtnComponent = itemQuickUseContent.transform.GetChild(i).GetComponent<QuickItemUseBtn>();
            
            if (quickBtnComponent?.ownToggle != null)
            {
                var itemData = gameDataManager.GetItemData(itemCode);
                quickBtnComponent.ownToggle.isOn = itemData?.isInUse ?? false;
            }
        }
    }

    // 구체적인 아이템 효과 메서드들
    private void ApplyExperienceBoost()
    {
        Debug.Log("경험치 증가 효과 적용");
    }

    private void RemoveExperienceBoost()
    {
        Debug.Log("경험치 증가 효과 제거");
    }

    private void ApplyGoldBoost()
    {
        Debug.Log("골드 증가 효과 적용");
    }

    private void RemoveGoldBoost()
    {
        Debug.Log("골드 증가 효과 제거");
    }

    private void ApplyReinforceBoost()
    {
        // 부모 ToolPanel의 강화 확률 증가
        if (parentToolPanel != null)
        {
            parentToolPanel.IsAddRP += 10.0f;
        }
        Debug.Log("강화 확률 증가 효과 적용");
    }

    private void RemoveReinforceBoost()
    {
        // 부모 ToolPanel의 강화 확률 증가 해제
        if (parentToolPanel != null)
        {
            parentToolPanel.IsAddRP -= 10.0f;
            if (parentToolPanel.IsAddRP < 0) parentToolPanel.IsAddRP = 0;
        }
        Debug.Log("강화 확률 증가 효과 제거");
    }

    void Start()
    {
        // 부모 ToolPanel 자동 찾기
        if (parentToolPanel == null)
        {
            parentToolPanel = GetComponentInParent<ToolPanel>();
        }
        
        Initialize(parentToolPanel, GameDataManager.Instance);
    }
}
