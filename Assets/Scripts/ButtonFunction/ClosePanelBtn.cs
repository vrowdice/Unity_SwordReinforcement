using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosePanelBtn : MonoBehaviour
{
    /// <summary>
    /// 타겟 패널
    /// </summary>
    public GameObject m_panel;

    /// <summary>
    /// 클릭 시
    /// </summary>
    public void Click()
    {
        m_panel.SetActive(false);
    }
}
