using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StuffType : MonoBehaviour
{
    /// <summary>
    /// 툴 타입
    /// </summary>
    public enum ToolType
    {
        Sword,
        Spear,
        Shild,
        Hammer,
        Sickle,
        Shovle,
        Axe
    }

    /// <summary>
    /// 사용 타입
    /// </summary>
    public enum ItemUseType
    {
        One,
        Con
    }

    /// <summary>
    /// 아이템 구매 타입
    /// </summary>
    public enum ItemBuyType
    {
        None,
        Csh
    }

    /// <summary>
    /// 아이템 효과 타입
    /// </summary>
    public enum ItemEffectType
    {
        Reinforce_persent,
        No_destroy
    }
}
