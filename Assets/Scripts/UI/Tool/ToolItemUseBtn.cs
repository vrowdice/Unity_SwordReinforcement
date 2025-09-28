using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolItemUseBtn : MonoBehaviour
{
    [SerializeField] Image iconImage = null;
    [SerializeField] Text nameText = null;
    [SerializeField] Text countText = null;

    private ToolItemUsePanel toolItemUsePanel = null;
    private ItemData itemCode = null;

    public void Initialize(ToolItemUsePanel argToolItemUsePanel, ItemData argItemData)
    {
        toolItemUsePanel = argToolItemUsePanel;
    }
}
