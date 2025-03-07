using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivePanelBtn : MonoBehaviour
{
    /// <summary>
    /// 켜고 끌 타겟
    /// </summary>
    public GameObject m_target = null;

    public void Click()
    {
        if(m_target.activeSelf == false)
        {
            m_target.SetActive(true);
        }
        else
        {
            m_target.SetActive(false);
        }
    }
}
