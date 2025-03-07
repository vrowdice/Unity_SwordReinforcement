using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Percent Data", menuName = "New Percent Data", order = 1)]
public class ToolPercentPriceData : ScriptableObject
{
    /// <summary>
    /// 툴 타입
    /// </summary>
    public StuffType.ToolType m_toolType = new StuffType.ToolType();

    /// <summary>
    /// 업그레이드 확률
    /// </summary>
    public float[] m_UpgradePercent = null;

    /// <summary>
    /// 강화 확률
    /// </summary>
    public float[] m_reinforcePercent = null;
}
