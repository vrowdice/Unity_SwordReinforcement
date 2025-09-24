using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUseBtn : MonoBehaviour
{
    /// <summary>
    /// 타겟 오브젝트
    /// </summary>
    public GameObject target = null;

    /// <summary>
    /// 활성화 됬는지 안됬는지 확인하기 위한 토글
    /// </summary>
    bool activeToggle = false;

    /// <summary>
    /// 클릭 시
    /// </summary>
    public void Click()
    {
        if(target.activeSelf == true)
        {
            activeToggle = true;
        }
        else
        {
            activeToggle = false;
        }

        if(activeToggle == true)
        {
            target.SetActive(false);
            activeToggle = false;
        }
        else
        {
            target.SetActive(true);
            activeToggle = true;
        }
    }
}
