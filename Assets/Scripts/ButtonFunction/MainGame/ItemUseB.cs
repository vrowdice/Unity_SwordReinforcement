using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUseB : MonoBehaviour
{
    /// <summary>
    /// 타겟 오브젝트
    /// </summary>
    public GameObject m_target = null;

    /// <summary>
    /// 활성화 됬는지 안됬는지 확인하기 위한 토글
    /// </summary>
    bool m_activeToggle = false;

    /// <summary>
    /// 클릭 시
    /// </summary>
    public void Click()
    {
        if(m_target.activeSelf == true)
        {
            m_activeToggle = true;
        }
        else
        {
            m_activeToggle = false;
        }

        if(m_activeToggle == true)
        {
            m_target.SetActive(false);
            m_activeToggle = false;
        }
        else
        {
            m_target.SetActive(true);
            m_activeToggle = true;
        }
    }
}
