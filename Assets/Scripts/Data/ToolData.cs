/*
 * 칼 관련 데이터는 이곳에서 다룸
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ToolData", menuName = "New Tool Data", order = 1)]
public class ToolData : ScriptableObject
{
    /// <summary>
    /// 툴 코드
    /// </summary>
    public int m_code = 0;

    /// <summary>
    /// 툴 타입
    /// </summary>
    public StuffType.ToolType m_toolType = new StuffType.ToolType();

    /// <summary>
    /// 업그레이드 횟수
    /// </summary>
    public int m_upgradeCount = 0;
    
    /// <summary>
    /// 이미지
    /// </summary>
    public Sprite m_image = null;

    /// <summary>
    /// 이름
    /// </summary>
    public string m_name = string.Empty;

    /// <summary>
    /// 가격
    /// </summary>
    public long m_price = 0;

    [TextArea]
    /// <summary>
    /// 설명
    /// </summary>
    public string m_explanation = string.Empty;
}
