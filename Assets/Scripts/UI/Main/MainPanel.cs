using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPanel : BasePanel
{
    protected override void OnPanelOpen()
    {
        base.OnPanelOpen();
        
        // 빠른 아이템 사용 초기화
        InitializeQuickItemUse();
    }

    /// <summary>
    /// 빠른 아이템 사용 초기화
    /// </summary>
    private void InitializeQuickItemUse()
    {
        var toolManager = GameManager.Instance?.toolManager;
        if (toolManager != null)
        {
            toolManager.InitializeQuickItemUse();
            toolManager.LoadQuickItemUse();
        }
    }
}
