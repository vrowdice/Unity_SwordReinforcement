using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivePanelBtn : MonoBehaviour
{
    /// <summary>
    /// 켜고 끌 타겟
    /// </summary>
    public GameObject target = null;

    public void Click()
    {
        if(target.activeSelf == false)
        {
            target.SetActive(true);
        }
        else
        {
            target.SetActive(false);
        }
    }
}
