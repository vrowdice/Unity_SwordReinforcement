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
    public int code = 0;

    /// <summary>
    /// 툴 타입
    /// </summary>
    public ToolType.TYPE toolType = new ToolType.TYPE();

    /// <summary>
    /// 업그레이드 횟수
    /// </summary>
    public int upgradeCount = 0;
    
    /// <summary>
    /// 이미지
    /// </summary>
    public Sprite image = null;

    /// <summary>
    /// 이름
    /// </summary>
    public string name = string.Empty;

    /// <summary>
    /// 가격
    /// </summary>
    public long price = 0;

    [TextArea]
    /// <summary>
    /// 설명
    /// </summary>
    public string explanation = string.Empty;
}
