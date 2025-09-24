using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickItemUseBtn : MonoBehaviour
{
    /// <summary>
    /// 자신의 아이템 코드
    /// </summary>
    public int ownitemCode = 0;

    /// <summary>
    /// 자신의 토글
    /// </summary>
    public Toggle ownToggle = null;

    // 캐시된 참조들
    private GameDataManager dataManager;
    private GameManager gameManager;

    private void Start()
    {
        dataManager = GameDataManager.Instance;
        gameManager = GameManager.Instance;
    }

    /// <summary>
    /// 아이템 빠른 사용 클릭시
    /// </summary>
    public void Click()
    {
        var itemData = dataManager.GetItemData(ownitemCode);
        if (itemData == null)
        {
            gameManager.Warning("아이템을 찾을 수 없습니다.");
            return;
        }

        if (itemData.isInUse)
        {
            gameManager.toolManager.DisableItem(ownitemCode);
        }
        else
        {
            gameManager.toolManager.ActiveItem(ownitemCode);
        }
    }
}
