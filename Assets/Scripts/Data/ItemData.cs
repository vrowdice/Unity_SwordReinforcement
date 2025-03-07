using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ItemData", menuName = "New Item Data", order = 1)]
public class ItemData : ScriptableObject
{
    [Header("Item Type")]
    /// <summary>
    /// 아이템 사용 타입
    /// </summary>
    public StuffType.ItemUseType m_useType = StuffType.ItemUseType.One;

    /// <summary>
    /// 아이템 구매 방식 타입
    /// </summary>
    public StuffType.ItemBuyType m_valueType = StuffType.ItemBuyType.None;
    
    [Header("Item Value")]
    /// <summary>
    /// 아이템코드
    /// </summary>
    public int m_code;

    /// <summary>
    /// 아이템이름
    /// </summary>
    public string m_name;

    /// <summary>
    /// 이미지
    /// </summary>
    public Sprite m_image;

    /// <summary>
    /// 아이템 가격
    /// </summary>
    public long m_price;

    /// <summary>
    /// 설명
    /// </summary>
    [TextArea]
    public string m_explanation;

    [Header("Item Effect Value")]
    /// <summary>
    /// 아이템 효과 타입
    /// </summary>
    public StuffType.ItemEffectType[] m_effectType = null;

    /// <summary>
    /// 아이템 효과 수치
    /// </summary>
    public int[] m_effectValue = null;
}
