using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinAniManager : MonoBehaviour
{
    [Header("Main Btn")]
    /// <summary>
    /// 메인 버튼들
    /// </summary>
    public GameObject m_mainBtn = null;

    [Header ("Panels")]
    /// <summary>
    /// 각 패널들
    /// </summary>
    public GameObject[] m_mainPanels = null;

    /// <summary>
    /// 플래이어 수치 패널
    /// </summary>
    public GameObject m_playerValuePanel = null;

    /// <summary>
    /// 빠른 이동 패널
    /// </summary>
    public GameObject m_quickMovePanel = null;

    [Header("Chest")]
    /// <summary>
    /// 변경 스크롤뷰
    /// </summary>
    public GameObject m_chestScrollView = null;

    /// <summary>
    /// 상자의 아이템 종류 선택 드롭다운
    /// </summary>
    public Dropdown m_chestDropdown = null;

    [Header("Stor")]
    /// <summary>
    /// 상점의 아이탬 선택 스크롤뷰
    /// </summary>
    public GameObject[] m_storScrollView = null;

    public void Start()
    {
        for (int i = 0; i < m_mainPanels.Length; i++)
        {
            m_mainPanels[i].SetActive(true);
        }

        for (int i = 0; i < m_storScrollView.Length; i++)
        {
            m_storScrollView[i].SetActive(true);
        }

        m_playerValuePanel.SetActive(true);

        ChangePanel(0);
    }

    /// <summary>
    /// 다른 패널로 변경
    /// </summary>
    public void ChangePanel(int argIndex)
    {
        for (int i = 0; i < m_mainPanels.Length; i++)
        {
            m_mainPanels[i].transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
        }

        m_mainPanels[argIndex].transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        if (argIndex != 0)
        {
            m_mainBtn.SetActive(true);
            m_playerValuePanel.transform.localPosition = new Vector3(430.0f, 465.0f);
        }
        else
        {
            m_mainBtn.SetActive(false);
            m_playerValuePanel.transform.localPosition = new Vector3(-430.0f, 465.0f);
        }
        
        if(argIndex == 4)
        {
            GManager.Instance.m_chestManager.NowImfoUpdate();
            GManager.Instance.m_chestManager.m_scrollbar.value = 1.0f;
        }

        m_quickMovePanel.SetActive(false);
    }
}
