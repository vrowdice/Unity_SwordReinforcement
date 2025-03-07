using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChestManager : MonoBehaviour
{
    [Header("Chest")]
    /// <summary>
    /// 검 강화 보여주는 패널
    /// </summary>
    public GameObject m_toolReinPanel = null;

    /// <summary>
    /// 상자 패널
    /// </summary>
    public GameObject m_chestPanel = null;

    /// <summary>
    /// 드롭 다운
    /// </summary>
    public Dropdown m_dropdown = null;

    /// <summary>
    /// 스크롤 바
    /// </summary>
    public Scrollbar m_scrollbar = null;

    [Header("Button Array")]
    /// <summary>
    /// 가방 버튼
    /// </summary>
    public ChestBtn[] m_chestBtn = null;

    /// <summary>
    /// 가방 강화 버튼
    /// </summary>
    public ChestReinBtn[] m_chestReinBtn = null;

    /// <summary>
    /// 검 이미지
    /// </summary>
    Image m_panelImage = null;

    /// <summary>
    /// 검 설명
    /// </summary>
    Text m_panelExplain = null;

    /// <summary>
    /// 검 강화 패널 설명 텍스트
    /// </summary>
    Text m_reinPanelText = null;

    /// <summary>
    /// 패널의 매니징 버튼
    /// </summary>
    GameObject m_panelManageBtn = null;

    /// <summary>
    /// 매니지 버튼 텍스트
    /// </summary>
    Text m_manageBtnText = null;

    /// <summary>
    /// 패널의 판매 버튼
    /// </summary>
    GameObject m_panelSellBtn = null;

    /// <summary>
    /// 아이템 사용 플래그
    /// </summary>
    bool m_itemUseFlag = false;

    /// <summary>
    /// 현재 물건 코드
    /// </summary>
    int m_nowCode = 0;

    /// <summary>
    /// 강화 횟수
    /// </summary>
    int m_nowReinforceCount = 0;

    /// <summary>
    /// 현재 상자 타입
    /// </summary>
    PanelType.ChestType m_nowChestType = new PanelType.ChestType();

    private void Start()
    {
        Setting();
    }

    /// <summary>
    /// 셋팅
    /// </summary>
    void Setting()
    {
        d
        for (int i = 0; i < m_chestBtn.Length; i++)
        {
            m_chestBtn[i].m_image = m_chestBtn[i].transform.Find("Image").gameObject.GetComponent<Image>();
            m_chestBtn[i].m_text = m_chestBtn[i].transform.Find("Text").gameObject.GetComponent<Text>();
            m_chestBtn[i].m_toggle = m_chestBtn[i].transform.Find("Toggle").gameObject.GetComponent<Toggle>();
        }
        for (int i = 0; i < m_chestReinBtn.Length; i++)
        {
            m_chestReinBtn[i].m_image = m_chestReinBtn[i].transform.Find("Image").gameObject.GetComponent<Image>();
            m_chestReinBtn[i].m_reintext = m_chestReinBtn[i].transform.Find("ReinText").gameObject.GetComponent<Text>();
            m_chestReinBtn[i].m_countText = m_chestReinBtn[i].transform.Find("CountText").gameObject.GetComponent<Text>();
            m_chestReinBtn[i].m_toggle = m_chestReinBtn[i].transform.Find("Toggle").gameObject.GetComponent<Toggle>();
        }

        m_panelImage = m_chestPanel.transform.GetChild(0).Find("Image").gameObject.GetComponent<Image>();
        m_panelExplain = m_chestPanel.transform.GetChild(0).Find("ExplainText").gameObject.GetComponent<Text>();
        m_panelManageBtn = m_chestPanel.transform.GetChild(0).Find("ManageBtn").gameObject;
        m_panelSellBtn = m_chestPanel.transform.GetChild(0).Find("SellBtn").gameObject;
        m_manageBtnText = m_panelManageBtn.transform.Find("Text").gameObject.GetComponent<Text>();

        m_reinPanelText = m_toolReinPanel.transform.Find("Text").gameObject.GetComponent<Text>();

        m_chestPanel.SetActive(false);
    }

    /// <summary>
    /// 현재 상자 타입 업데이트
    /// </summary>
    public void NowImfoUpdate()
    {
        GameDataManager.Instance.UpdateChest(m_nowChestType);
    }

    /// <summary>
    /// 상자 버튼 선택
    /// </summary>
    public void ChestBtn(int argCode)
    {
        m_nowCode = argCode;
        
        if(m_nowChestType == PanelType.ChestType.Tool)
        {
            GameDataManager.Instance.UpdateToolReinChest(m_nowCode);
            m_reinPanelText.text = string.Format("검 선택 창: " + GameDataManager.Instance.m_toolDic[m_nowCode].m_name);
            m_toolReinPanel.SetActive(true);
        }
        else if(m_nowChestType == PanelType.ChestType.Item)
        {
            SetChestPanel();
        }
        else
        {

        }
    }

    /// <summary>
    /// 매니지 버튼 컨트롤
    /// </summary>
    public void ManageBtnControll()
    {
        if (m_nowChestType == PanelType.ChestType.Tool)
        {
            ChestPutOffTool();
            m_toolReinPanel.SetActive(false);
        }
        else if (m_nowChestType == PanelType.ChestType.Item)
        {
            if (m_itemUseFlag)
            {
                UseItem();
            }
            else
            {
                DisuseItem();
            }
        }
        else
        {
            return;
        }

        NowImfoUpdate();
        m_chestPanel.SetActive(false);
    }

    /// <summary>
    /// 판매 버튼 컨트롤
    /// </summary>
    public void SellBtnControll()
    {
        if (m_nowChestType == PanelType.ChestType.Tool)
        {
            ChestSellTool();
            m_toolReinPanel.SetActive(false);
        }
        else if (m_nowChestType == PanelType.ChestType.Item)
        {
            SellItem();
        }
        else
        {
            return;
        }

        NowImfoUpdate();
        m_chestPanel.SetActive(false);
    }

    /// <summary>
    /// 강화횟수 툴 버튼 클릭 시
    /// </summary>
    /// <param name="argRein">강화 횟수</param>
    public void SelectToolRein(int argRein)
    {
        m_nowReinforceCount = argRein;
        SetChestPanel();
    }

    /// <summary>
    /// 패널 설정
    /// </summary>
    public void SetChestPanel()
    {   
        if (m_nowChestType == PanelType.ChestType.Tool)
        {
            m_panelManageBtn.SetActive(true);
            m_panelSellBtn.SetActive(true);
            m_panelImage.sprite = GameDataManager.Instance.m_toolDic[m_nowCode].m_image;
            m_panelImage.preserveAspect = true;
            m_panelExplain.text = GameDataManager.Instance.m_toolDic[m_nowCode].m_explanation;

            m_manageBtnText.text = string.Format("검꺼내기");
        }
        else if(m_nowChestType == PanelType.ChestType.Item)
        {
            m_panelManageBtn.SetActive(true);
            m_panelSellBtn.SetActive(true);
            m_panelImage.sprite = GameDataManager.Instance.m_itemDic[m_nowCode].m_image;
            m_panelImage.preserveAspect = true;
            m_panelExplain.text = GameDataManager.Instance.m_itemDic[m_nowCode].m_explanation;

            if (GameDataManager.Instance.ManageItem(m_nowCode).m_use == true)
            {
                m_manageBtnText.text = string.Format("사용하기");
                m_itemUseFlag = true;
            }
            else
            {
                m_manageBtnText.text = string.Format("비활성화");
                m_itemUseFlag = false;
            }
        }
        else
        {
            m_panelManageBtn.SetActive(false);
            m_panelSellBtn.SetActive(false);
        }

        m_chestPanel.SetActive(true);
    }

    /// <summary>
    /// 상자에서 검 꺼내기
    /// </summary>
    public void ChestPutOffTool()
    {
        if (GameDataManager.Instance.ManageTool(m_nowCode, m_nowReinforceCount) <= 0)
        {
            GManager.Instance.Warning("꺼낼 수 없습니다.");
            return;
        }

        GameDataManager.Instance.ManageTool(m_nowCode, m_nowReinforceCount, -1);
        GManager.Instance.m_toolManager.ToolPutInChest();
        GManager.Instance.m_toolManager.MakeTool(m_nowCode, m_nowReinforceCount);
        NowImfoUpdate();
    }

    /// <summary>
    /// 상자 안의 검 팔기
    /// </summary>
    public void ChestSellTool()
    {
        GManager.Instance.m_toolManager.SellTool(m_nowCode, m_nowReinforceCount);
        GameDataManager.Instance.ManageTool(m_nowCode, m_nowReinforceCount, -1);
        if (GameDataManager.Instance.ManageTool(m_nowCode, m_nowReinforceCount) == 0)
        {
            return;
        }
    }

    /// <summary>
    /// 선택 버튼 클릭 시
    /// </summary>
    /// <param name="argIndex">선택 버튼 인덱스</param>
    public void SelectBtn(int argIndex)
    {
        if(argIndex <= 0)
        {
            m_nowChestType = PanelType.ChestType.Tool;
        }
        else if(argIndex == 1)
        {
            m_nowChestType = PanelType.ChestType.Item;
        }
        else
        {
            m_nowChestType = PanelType.ChestType.Odd;
        }

        GameDataManager.Instance.UpdateChest(m_nowChestType);
    }

    /// <summary>
    /// 아이탬 사용
    /// </summary>
    public void UseItem()
    {
        GManager.Instance.m_itemManager.ActiveItem(m_nowCode);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 아이탬 비활성화
    /// </summary>
    public void DisuseItem()
    {
        GManager.Instance.m_itemManager.DisableItem(m_nowCode);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 아이템 판매
    /// </summary>
    public void SellItem()
    {
        GManager.Instance.SellSetSlider(m_nowCode);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 가방 필터
    /// </summary>
    /// <param name="argIndex"></param>
    public void ChestFilter(int argIndex)
    {
        switch (argIndex)
        {
            case 0:
                m_dropdown.gameObject.SetActive(true);

                m_dropdown.options.Clear();
                for (int i = 0; i < 7; i++)
                {
                    m_dropdown.options.Add(new Dropdown.OptionData());
                }

                m_dropdown.options[0].text = "검";
                m_dropdown.options[1].text = "창";
                m_dropdown.options[2].text = "방패";
                m_dropdown.options[3].text = "망치";
                m_dropdown.options[4].text = "낫";
                m_dropdown.options[5].text = "삽";
                m_dropdown.options[6].text = "도끼";

                m_dropdown.value = 1;
                m_dropdown.value = 0;
                break;
            case 1:
                m_dropdown.gameObject.SetActive(true);

                m_dropdown.options.Clear();
                for (int i = 0; i < 2; i++)
                {
                    m_dropdown.options.Add(new Dropdown.OptionData());
                }

                m_dropdown.options[0].text = "노멀";
                m_dropdown.options[1].text = "캐쉬";

                m_dropdown.value = 1;
                m_dropdown.value = 0;
                break;
            case 2:
                m_dropdown.gameObject.SetActive(false);
                break;
        }
    }
}
