using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
    아이템 관련 모든 프로그래밍은 이 스크립트가 관여
    아이템 관련 다른 스크립트가 생성되는 경우
    itemsortmanager을 불러오는 스크립트이거나
    간단한 애니매이션과 같은
    직접 스크립트를 사용하는 것이 효율적일 때만 사용

    m_selectSell을 통해 상황에 맞는 아이템 셀과 상호작용하고 있기 때문에
    주의가 필요
*/

public class ItemManager : MonoBehaviour
{
    /// <summary>
    /// 기본 UI이미지
    /// </summary>
    public Sprite m_defultUiSprite = null;

    [Header("Item")]
    /// <summary>
    /// 최대 아이템 버튼
    /// </summary>
    public int m_maxItemBtnLine = 0;

    /// <summary>
    /// 아이템 버튼 줄
    /// </summary>
    public GameObject m_itemBtnLine = null;

    /// <summary>
    /// 아이템 사용 표시 아이템 아이콘
    /// </summary>
    public GameObject m_icon = null;

    /// <summary>
    /// 아이템 선택 패널
    /// </summary>
    public GameObject m_itemChoosePanel = null;

    /// <summary>
    /// 매인 게임의 아이템 아이콘 정리 매니저
    /// </summary>
    public GameObject m_itemIconSortPanel = null;
    
    [Header("Quick Use Item")]
    /// <summary>
    /// 아이템 사용 버튼
    /// </summary>
    public GameObject m_itemQuickUseBtn = null;

    [Header("Scroll View Contents")]

    /// <summary>
    /// 빠른 아이템 사용 안의 콘텐츠
    /// </summary>
    public GameObject m_itemQuickUseContent = null;

    /// <summary>
    /// start
    /// </summary>
    private void Start()
    {
        for(int i = 0; i < m_itemQuickUseContent.transform.childCount; i++)
        {
            GameDataManager.Instance.m_quickItemUse.Add(m_itemQuickUseContent.transform.GetChild(i).gameObject.GetComponent<QuickItemUseBtn>().m_ownitemCode);
        }

        LoadQuickItemUse();
    }

    /// <summary>
    /// 아이템 활성화
    /// </summary>
    /// <param name="argItemCode">아이템 코드</param>
    public void ActiveItem(int argItemCode)
    {
        if (GameDataManager.Instance.ManageItem(argItemCode).m_amount <= 0 || GameDataManager.Instance.ManageItem(argItemCode).m_use == true)
        {
            GManager.Instance.Warning("아이템 사용이 불가능합니다.");
            return;
        }
        
        GameDataManager.Instance.ManageItem(argItemCode, true);
        GameDataManager.Instance.ManageItem(argItemCode, -1);
        AItemEffect(argItemCode);
        UpdateQuickItemUse();
        GManager.Instance.m_chestManager.NowImfoUpdate();

        GameObject _icon = Instantiate(m_icon);
        _icon.transform.SetParent(m_itemIconSortPanel.transform);
        _icon.transform.localScale = new Vector3(1, 1, 1);
        _icon.transform.SetAsFirstSibling();
        _icon.transform.Find("Image").GetComponent<Image>().sprite = GameDataManager.Instance.m_itemDic[argItemCode].m_image;
        _icon.transform.Find("Image").GetComponent<Image>().preserveAspect = true;
    }

    /// <summary>
    /// 아이템 비활성화
    /// </summary>
    /// <param name="argItemCode">아이템 코드</param>
    public void DisableItem(int argItemCode)
    {
        if(GameDataManager.Instance.ManageItem(argItemCode).m_use == false)
        {
            GManager.Instance.Warning("아이템 비활성화가 불가능합니다.");
            return;
        }
        
        GameDataManager.Instance.ManageItem(argItemCode).m_use = false;
        GameDataManager.Instance.ManageItem(argItemCode, 1);
        DItemEffect(argItemCode);
        UpdateQuickItemUse();
        Destroy(m_itemIconSortPanel.transform.GetChild(GameDataManager.Instance.m_useItem.IndexOf(argItemCode)).gameObject);
    }

    /// <summary>
    /// 사용한 아이템과 효과 제거
    /// </summary>
    public void UseItemDestroy()
    {
        for(int i = 0; i < GameDataManager.Instance.m_useItem.Count; i++)
        {

            GameDataManager.Instance.ManageItem(GameDataManager.Instance.m_useItem[i], false);
            Destroy(m_itemIconSortPanel.transform.GetChild(i).gameObject);
        }

        GManager.Instance.m_toolManager.IsNoDestroy = false;
        GManager.Instance.m_toolManager.IsImprovePercent = 0.0f;
    }

    /// <summary>
    /// 일회용 아이템 이펙트 적용
    /// </summary>
    /// <param name="argItemCode">아이템 코드</param>
    public void AItemEffect(int argItemCode)
    {
        ItemData _itemData = GameDataManager.Instance.m_itemDic[argItemCode];

        for(int i = 0; i < _itemData.m_effectType.Length; i++)
        {
            switch (_itemData.m_effectType[i])
            {
                case StuffType.ItemEffectType.No_destroy:
                    GManager.Instance.m_toolManager.IsNoDestroy = true;
                    break;
                case StuffType.ItemEffectType.Reinforce_persent:
                    GManager.Instance.m_toolManager.IsAddRP = _itemData.m_effectValue[i];
                    break;
            }
        }

    }

    /// <summary>
    /// 일회용 아이템 이펙트 비활성화
    /// </summary>
    /// <param name="argItemCode">아이템 코드</param>
    public void DItemEffect(int argItemCode)
    {
        ItemData _itemData = GameDataManager.Instance.m_itemDic[argItemCode];

        for (int i = 0; i < _itemData.m_effectType.Length; i++)
        {
            switch (_itemData.m_effectType[i])
            {
                case StuffType.ItemEffectType.No_destroy:
                    GManager.Instance.m_toolManager.IsNoDestroy = false;
                    break;
                case StuffType.ItemEffectType.Reinforce_persent:
                    GManager.Instance.m_toolManager.IsImprovePercent = 0.0f;
                    break;
            }
        }
    }

    /// <summary>
    /// 아이탬 구매
    /// </summary>
    public void BuyItem(int argItemCode, int argAmount)
    {
        if (argItemCode <= -1)
        {
            GManager.Instance.Warning("아이탬이 선택되지 않았습니다.");
            return;
        }
        
        if (GameDataManager.Instance.IsBronze < GameDataManager.Instance.m_itemDic[argItemCode].m_price * argAmount)
        {
            GManager.Instance.NoMoney();
            return;
        }
        else if (GameDataManager.Instance.IsBronze < 0)
        {
            GManager.Instance.NoMoney();
            GameDataManager.Instance.IsBronze = 0;
            return;
        }

        GameDataManager.Instance.IsBronze -= GameDataManager.Instance.m_itemDic[argItemCode].m_price * argAmount;
        GameDataManager.Instance.ManageItem(argItemCode, argAmount);
        GManager.Instance.m_storManager.UpdateBuySellPanel(argItemCode);
    }

    /// <summary>
    /// 아이탬 판매
    /// </summary>
    public void SellItem(int argItemCode, int argAmount)
    {
        if (argItemCode <= -1)
        {
            GManager.Instance.Warning("아이탬이 선택되지 않았습니다.");
            return;
        }
        if(GameDataManager.Instance.ManageItem(argItemCode).m_amount <= 0)
        {
            GManager.Instance.Warning("아이탬이 없습니다.");
            return;
        }
        GameDataManager.Instance.IsBronze += GameDataManager.Instance.m_itemDic[argItemCode].m_price * argAmount * 7 / 10;
        GameDataManager.Instance.ManageItem(argItemCode, argAmount);
        GManager.Instance.m_storManager.UpdateBuySellPanel(argItemCode);
    }

    /// <summary>
    /// 게임 데이터 매니저의 빠른아이템사용 정보 참조해 빠른아이템사용 콘텐츠에 추가
    /// </summary>
    public void LoadQuickItemUse()
    {
        for (int i = 0; i < GameDataManager.Instance.m_quickItemUse.Count; i++)
        {
            ItemData _itemData = GameDataManager.Instance.m_itemDic[GameDataManager.Instance.m_quickItemUse[i]];

            GameObject _btn = Instantiate(m_itemQuickUseBtn);
            _btn.transform.SetParent(m_itemQuickUseContent.transform);
            _btn.transform.localScale = new Vector3(1, 1, 1);
            _btn.transform.Find("ItemImage").gameObject.GetComponent<Image>().sprite = _itemData.m_image;
            _btn.transform.Find("ItemText").gameObject.GetComponent<Text>().text = _itemData.m_name;
            _btn.GetComponent<QuickItemUseBtn>().m_ownitemCode = GameDataManager.Instance.m_quickItemUse[i];
            _btn.GetComponent<QuickItemUseBtn>().m_ownToggle = _btn.transform.Find("Toggle").gameObject.GetComponent<Toggle>();
        }
    }

    /// <summary>
    /// 빠른 아이템 사용 새로고침
    /// </summary>
    public void UpdateQuickItemUse()
    {
        for(int i = 0; i < GameDataManager.Instance.m_quickItemUse.Count; i++)
        {
            m_itemQuickUseContent.transform.GetChild(i).gameObject.GetComponent<QuickItemUseBtn>().m_ownToggle.isOn =
                GameDataManager.Instance.ManageItem(GameDataManager.Instance.m_quickItemUse[i]).m_use;
        }
    }
}
