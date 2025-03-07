using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelType : MonoBehaviour
{
    /// <summary>
    /// 가방 타입
    /// </summary>
    public enum ChestType
    {
        Tool,
        Item,
        Odd
    }

    /// <summary>
    /// 제련 패널 개선 타입
    /// </summary>
    public enum ImproveType
    {
        Reinforce,
        Upgrade
    }
}
