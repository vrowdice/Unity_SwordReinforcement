using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickItemUseBtn : MonoBehaviour
{
    /// <summary>
    /// 자신의 아이템 코드
    /// </summary>
    public int m_ownitemCode = 0;

    /// <summary>
    /// 자신의 토글
    /// </summary>
    public Toggle m_ownToggle = null;

    /// <summary>
    /// 아이템 빠른 사용 클릭시
    /// </summary>
    public void Click()
    {
        if(GameDataManager.Instance.ManageItem(m_ownitemCode).m_use == true)
        {
            GManager.Instance.m_itemManager.DisableItem(m_ownitemCode);
        }
        else
        {
            GManager.Instance.m_itemManager.ActiveItem(m_ownitemCode);
        }
    }
}
