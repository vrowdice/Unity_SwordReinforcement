using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChestPanel : BasePanel
{
    [Header("Chest")]
    /// <summary>
    /// 검 강화 보여주는 패널
    /// </summary>
    public GameObject toolReinPanel = null;

    /// <summary>
    /// 상자 패널
    /// </summary>
    public GameObject chestPanel = null;

    /// <summary>
    /// 드롭 다운
    /// </summary>
    public Dropdown dropdown = null;

    /// <summary>
    /// 스크롤 바
    /// </summary>
    public Scrollbar scrollbar = null;

    [Header("Button Array")]
    /// <summary>
    /// 가방 버튼
    /// </summary>
    public ChestBtn[] chestBtn = null;

    /// <summary>
    /// 가방 강화 버튼
    /// </summary>
    public ChestReinBtn[] chestReinBtn = null;

    /// <summary>
    /// 검 이미지
    /// </summary>
    Image panelImage = null;

    /// <summary>
    /// 검 설명
    /// </summary>
    Text panelExplain = null;

    /// <summary>
    /// 검 강화 패널 설명 텍스트
    /// </summary>
    Text reinPanelText = null;

    /// <summary>
    /// 패널의 매니징 버튼
    /// </summary>
    GameObject panelManageBtn = null;

    /// <summary>
    /// 매니지 버튼 텍스트
    /// </summary>
    Text manageBtnText = null;

    /// <summary>
    /// 패널의 판매 버튼
    /// </summary>
    GameObject panelSellBtn = null;

    /// <summary>
    /// 아이템 사용 플래그
    /// </summary>
    bool itemUseFlag = false;

    /// <summary>
    /// 현재 물건 코드
    /// </summary>
    int nowCode = 0;

    /// <summary>
    /// 강화 횟수
    /// </summary>
    int nowReinforceCount = 0;

    /// <summary>
    /// 현재 상자 타입
    /// </summary>
    PanelType.ChestType nowChestType = new PanelType.ChestType();

    protected override void OnPanelOpen()
    {
        base.OnPanelOpen();
        
        Setting();
    }

    /// <summary>
    /// 셋팅
    /// </summary>
    void Setting()
    {
        if (GameDataManager == null)
        {
            Debug.LogError("Required managers not found!");
            return;
        }

        InitializeChestButtons();
        InitializeReinforceButtons();
        InitializePanelComponents();
        
        chestPanel.SetActive(false);
    }

    /// <summary>
    /// 상자 버튼들 초기화
    /// </summary>
    private void InitializeChestButtons()
    {
        if (chestBtn == null) return;

        for (int i = 0; i < chestBtn.Length; i++)
        {
            if (chestBtn[i] != null)
            {
                chestBtn[i].image = chestBtn[i].transform.Find("Image")?.GetComponent<Image>();
                chestBtn[i].text = chestBtn[i].transform.Find("Text")?.GetComponent<Text>();
                chestBtn[i].toggle = chestBtn[i].transform.Find("Toggle")?.GetComponent<Toggle>();
            }
        }
    }

    /// <summary>
    /// 강화 버튼들 초기화
    /// </summary>
    private void InitializeReinforceButtons()
    {
        if (chestReinBtn == null) return;

        for (int i = 0; i < chestReinBtn.Length; i++)
        {
            if (chestReinBtn[i] != null)
            {
                chestReinBtn[i].image = chestReinBtn[i].transform.Find("Image")?.GetComponent<Image>();
                chestReinBtn[i].reintext = chestReinBtn[i].transform.Find("ReinText")?.GetComponent<Text>();
                chestReinBtn[i].countText = chestReinBtn[i].transform.Find("CountText")?.GetComponent<Text>();
                chestReinBtn[i].toggle = chestReinBtn[i].transform.Find("Toggle")?.GetComponent<Toggle>();
            }
        }
    }

    /// <summary>
    /// 패널 컴포넌트들 초기화
    /// </summary>
    private void InitializePanelComponents()
    {
        if (chestPanel == null || toolReinPanel == null) return;

        var chestPanelChild = chestPanel.transform.GetChild(0);
        if (chestPanelChild != null)
        {
            panelImage = chestPanelChild.Find("Image")?.GetComponent<Image>();
            panelExplain = chestPanelChild.Find("ExplainText")?.GetComponent<Text>();
            panelManageBtn = chestPanelChild.Find("ManageBtn")?.gameObject;
            panelSellBtn = chestPanelChild.Find("SellBtn")?.gameObject;
            
            if (panelManageBtn != null)
            {
                manageBtnText = panelManageBtn.transform.Find("Text")?.GetComponent<Text>();
            }
        }

        reinPanelText = toolReinPanel.transform.Find("Text")?.GetComponent<Text>();
    }

    /// <summary>
    /// 현재 상자 타입 업데이트
    /// </summary>
    public void NowImfoUpdate()
    {
        if (GameDataManager != null)
        {
            GameDataManager.UpdateChest(nowChestType);
        }
    }

    /// <summary>
    /// 상자 버튼 선택
    /// </summary>
    public void ChestBtn(int argCode)
    {
        if (GameDataManager == null) return;

        nowCode = argCode;
        
        if(nowChestType == PanelType.ChestType.Tool)
        {
            HandleToolSelection(argCode);
        }
        else if(nowChestType == PanelType.ChestType.Item)
        {
            SetChestPanel();
        }
    }

    /// <summary>
    /// 툴 선택 처리
    /// </summary>
    /// <param name="toolCode">툴 코드</param>
    private void HandleToolSelection(int toolCode)
    {
        GameDataManager.UpdateToolReinChest(toolCode);
        
        var toolData = GameDataManager.GetToolData(toolCode);
        if (toolData != null && reinPanelText != null)
        {
            reinPanelText.text = $"검 선택 창: {toolData.name}";
        }
        
        if (toolReinPanel != null)
        {
            toolReinPanel.SetActive(true);
        }
    }

    /// <summary>
    /// 매니지 버튼 컨트롤
    /// </summary>
    public void ManageBtnControll()
    {
        if (nowChestType == PanelType.ChestType.Tool)
        {
            ChestPutOffTool();
            if (toolReinPanel != null)
                toolReinPanel.SetActive(false);
        }
        else if (nowChestType == PanelType.ChestType.Item)
        {
            if (itemUseFlag)
            {
                UseItem();
            }
            else
            {
                DisuseItem();
            }
        }

        NowImfoUpdate();
        if (chestPanel != null)
            chestPanel.SetActive(false);
    }

    /// <summary>
    /// 판매 버튼 컨트롤
    /// </summary>
    public void SellBtnControll()
    {
        if (nowChestType == PanelType.ChestType.Tool)
        {
            ChestSellTool();
            if (toolReinPanel != null)
                toolReinPanel.SetActive(false);
        }
        else if (nowChestType == PanelType.ChestType.Item)
        {
            SellItem();
        }

        NowImfoUpdate();
        if (chestPanel != null)
            chestPanel.SetActive(false);
    }

    /// <summary>
    /// 강화횟수 툴 버튼 클릭 시
    /// </summary>
    /// <param name="argRein">강화 횟수</param>
    public void SelectToolRein(int argRein)
    {
        nowReinforceCount = argRein;
        SetChestPanel();
    }

    /// <summary>
    /// 패널 설정
    /// </summary>
    public void SetChestPanel()
    {   
        if (GameDataManager == null) return;

        if (nowChestType == PanelType.ChestType.Tool)
        {
            SetToolPanel();
        }
        else if(nowChestType == PanelType.ChestType.Item)
        {
            SetItemPanel();
        }
        else
        {
            // 기타 타입의 경우 버튼들 비활성화
            if (panelManageBtn != null) panelManageBtn.SetActive(false);
            if (panelSellBtn != null) panelSellBtn.SetActive(false);
        }

        if (chestPanel != null)
            chestPanel.SetActive(true);
    }

    /// <summary>
    /// 툴 패널 설정
    /// </summary>
    private void SetToolPanel()
    {
        if (panelManageBtn != null) panelManageBtn.SetActive(true);
        if (panelSellBtn != null) panelSellBtn.SetActive(true);

        var toolData = GameDataManager.GetToolData(nowCode);
        if (toolData != null)
        {
            if (panelImage != null)
            {
                panelImage.sprite = toolData.image;
                panelImage.preserveAspect = true;
            }
            
            if (panelExplain != null)
                panelExplain.text = toolData.explanation;
        }

        if (manageBtnText != null)
            manageBtnText.text = "검꺼내기";
    }

    /// <summary>
    /// 아이템 패널 설정
    /// </summary>
    private void SetItemPanel()
    {
        if (panelManageBtn != null) panelManageBtn.SetActive(true);
        if (panelSellBtn != null) panelSellBtn.SetActive(true);

        // 아이템 마스터 데이터 조회
        var itemMasterData = GameDataManager.GetItemMasterData(nowCode);
        if (itemMasterData != null)
        {
            if (panelImage != null)
            {
                panelImage.sprite = itemMasterData.image;
                panelImage.preserveAspect = true;
            }
            
            if (panelExplain != null)
                panelExplain.text = itemMasterData.explanation;
        }

        // 아이템 사용 상태 확인
        var userItemData = GameDataManager.GetItemData(nowCode);
        if (userItemData != null && manageBtnText != null)
        {
            if (userItemData.isInUse)
            {
                manageBtnText.text = "비활성화";
                itemUseFlag = false;
            }
            else
            {
                manageBtnText.text = "사용하기";
                itemUseFlag = true;
            }
        }
    }

    /// <summary>
    /// 상자에서 검 꺼내기
    /// </summary>
    public void ChestPutOffTool()
    {
        if (GameDataManager == null) return;

        if (GameDataManager.ManageTool(nowCode, nowReinforceCount) <= 0)
        {
            GameManager.Instance?.Warning("꺼낼 수 없습니다.");
            return;
        }

        GameDataManager.ManageTool(nowCode, nowReinforceCount, -1);
        
        if (GameManager.Instance?.toolManager != null)
        {
            GameManager.Instance.toolManager.ToolPutInChest();
            GameManager.Instance.toolManager.MakeTool(nowCode, nowReinforceCount);
        }
        
        NowImfoUpdate();
    }

    /// <summary>
    /// 상자 안의 검 팔기
    /// </summary>
    public void ChestSellTool()
    {
        if (GameDataManager == null) return;

        if (GameManager.Instance?.toolManager != null)
        {
            GameManager.Instance.toolManager.SellTool(nowCode, nowReinforceCount);
        }
        
        GameDataManager.ManageTool(nowCode, nowReinforceCount, -1);
        
        // 아이템이 모두 팔렸는지 확인
        if (GameDataManager.ManageTool(nowCode, nowReinforceCount) == 0)
        {
            // 더 이상 아이템이 없으므로 패널 닫기
            if (chestPanel != null)
                chestPanel.SetActive(false);
        }
    }

    /// <summary>
    /// 선택 버튼 클릭 시
    /// </summary>
    /// <param name="argIndex">선택 버튼 인덱스</param>
    public void SelectBtn(int argIndex)
    {
        // 상자 타입 설정
        switch (argIndex)
        {
            case 0:
                nowChestType = PanelType.ChestType.Tool;
                break;
            case 1:
                nowChestType = PanelType.ChestType.Item;
                break;
            default:
                nowChestType = PanelType.ChestType.Odd;
                break;
        }

        if (GameDataManager != null)
        {
            GameDataManager.UpdateChest(nowChestType);
        }
    }

    /// <summary>
    /// 아이템 사용
    /// </summary>
    public void UseItem()
    {
        if (GameManager.Instance?.toolManager != null)
        {
            GameManager.Instance.toolManager.ActiveItem(nowCode);
        }
        
        if (gameObject != null)
            gameObject.SetActive(false);
    }

    /// <summary>
    /// 아이템 비활성화
    /// </summary>
    public void DisuseItem()
    {
        if (GameManager.Instance?.toolManager != null)
        {
            GameManager.Instance.toolManager.DisableItem(nowCode);
        }
        
        if (gameObject != null)
            gameObject.SetActive(false);
    }

    /// <summary>
    /// 아이템 판매
    /// </summary>
    public void SellItem()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SellSetSlider(nowCode);
        }
        
        if (gameObject != null)
            gameObject.SetActive(false);
    }

    /// <summary>
    /// 가방 필터
    /// </summary>
    /// <param name="argIndex">필터 인덱스</param>
    public void ChestFilter(int argIndex)
    {
        if (dropdown == null) return;

        switch (argIndex)
        {
            case 0: // 툴 필터
                SetupToolFilter();
                break;
            case 1: // 아이템 필터
                SetupItemFilter();
                break;
            case 2: // 필터 없음
                dropdown.gameObject.SetActive(false);
                break;
        }
    }

    /// <summary>
    /// 툴 필터 설정
    /// </summary>
    private void SetupToolFilter()
    {
        dropdown.gameObject.SetActive(true);
        dropdown.options.Clear();

        string[] toolNames = { "검", "창", "방패", "망치", "낫", "삽", "도끼" };
        
        foreach (string toolName in toolNames)
        {
            dropdown.options.Add(new Dropdown.OptionData(toolName));
        }

        // 드롭다운 값 초기화 (트릭: 1로 설정했다가 0으로 변경하여 이벤트 발생)
        dropdown.value = 1;
        dropdown.value = 0;
    }

    /// <summary>
    /// 아이템 필터 설정
    /// </summary>
    private void SetupItemFilter()
    {
        dropdown.gameObject.SetActive(true);
        dropdown.options.Clear();

        string[] itemTypes = { "노멀", "캐시" };
        
        foreach (string itemType in itemTypes)
        {
            dropdown.options.Add(new Dropdown.OptionData(itemType));
        }

        // 드롭다운 값 초기화
        dropdown.value = 1;
        dropdown.value = 0;
    }
}
