using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreManager : MonoBehaviour
{
    /// <summary>
    /// 상점 콘텐츠들
    /// </summary>
    public GameObject[] m_storContents = null;

    /// <summary>
    /// 아이템 패널
    /// </summary>
    public GameObject m_itemBuySellPanel = null;

    /// <summary>
    /// 상점 아이템 버튼
    /// </summary>
    public GameObject m_storBtn = null;

    /// <summary>
    /// 자신의 아이템 코드
    /// </summary>
    int m_nowItemCode = -1;

    /// <summary>
    /// 상점 스크롤 뷰
    /// </summary>
    GameObject[] m_storScrollView = null;

    /// <summary>
    /// 상점 콘텐츠 선택 버튼
    /// </summary>
    GameObject[] m_storSelectBtn = null;

    /// <summary>
    /// 아이탬 이미지
    /// </summary>
    Image m_itemImage = null;

    /// <summary>
    /// 아이탬 이름 택스트
    /// </summary>
    Text m_itemName = null;

    /// <summary>
    /// 아이탬 설명 택스트
    /// </summary>
    Text m_itemExplain = null;

    /// <summary>
    /// 아이탬 가격 택스트
    /// </summary>
    Text m_itemPrice = null;

    /// <summary>
    /// 아이템 갯수 텍스트
    /// </summary>
    Text m_itemAmount = null;

    private void Start()
    {
        Setting();
    }

    void Setting()
    {
        m_storSelectBtn = new GameObject[3];
        m_storScrollView = new GameObject[3];

        m_itemImage = m_itemBuySellPanel.transform.Find("Image").gameObject.GetComponent<Image>();
        m_itemName = m_itemBuySellPanel.transform.Find("NameText").gameObject.GetComponent<Text>();
        m_itemExplain = m_itemBuySellPanel.transform.Find("ExplainText").gameObject.GetComponent<Text>();
        m_itemPrice = m_itemBuySellPanel.transform.Find("PriceText").gameObject.GetComponent<Text>(); ;
        m_itemAmount = m_itemBuySellPanel.transform.Find("AmountText").gameObject.GetComponent<Text>();

        m_storSelectBtn[0] = transform.Find("OneToggle").gameObject;
        m_storSelectBtn[1] = transform.Find("ConToggle").gameObject;
        m_storSelectBtn[2] = transform.Find("CshToggle").gameObject;

        m_storScrollView[0] = transform.Find("Scroll View Stor Disposable").gameObject;
        m_storScrollView[1] = transform.Find("Scroll View Stor Last").gameObject;
        m_storScrollView[2] = transform.Find("Scroll View Stor Cash").gameObject;

        m_nowItemCode = 0;

        UpdateBuySellPanel(30000);
        UpdateStorItem();
        ChangeStorView(0);
    }

    /// <summary>
    /// 버튼 클릭 시 메인패널의 이미지와 설명 변경 및 구매버튼 인계
    /// </summary>
    public void UpdateBuySellPanel(int argItemCode)
    {
        m_nowItemCode = argItemCode;

        ItemData _tmpData = null;
        int _tmpAmount = 0;
        _tmpData = GameDataManager.Instance.m_itemDic[m_nowItemCode];
        if (GameDataManager.Instance.CheckChestItem(m_nowItemCode))
        {
            _tmpAmount = GameDataManager.Instance.ManageItem(m_nowItemCode).m_amount;
        }
        
        m_itemImage.sprite = _tmpData.m_image;
        m_itemImage.preserveAspect = true;
        m_itemName.text = _tmpData.m_name;
        m_itemExplain.text = _tmpData.m_explanation;
        m_itemPrice.text = "가격 : " + _tmpData.m_price.ToString();
        if (_tmpAmount <= 0)
        {
            m_itemAmount.text = "0 개";
            return;
        }
        m_itemAmount.text = _tmpAmount.ToString() + " 개";
    }

    /// <summary>
    /// 상점 아이템 아이템 데이터 참고해서 업데이트
    /// </summary>
    void UpdateStorItem()
    {
        int _count0 = 0;
        int _count1 = 0;
        int _count2 = 0;

        foreach (KeyValuePair<int, ItemData> item in GameDataManager.Instance.m_itemDic)
        {
            if(item.Value.m_valueType == StuffType.ItemBuyType.Csh)
            {
                GameObject _btn = Instantiate(m_storBtn);

                _btn.transform.SetParent(m_storContents[2].transform);
                _btn.transform.localScale = new Vector3(1, 1, 1);

                _btn.transform.Find("NameText").gameObject.GetComponent<Text>().text = item.Value.m_name;
                _btn.transform.Find("ExplainText").gameObject.GetComponent<Text>().text = item.Value.m_explanation;
                _btn.transform.Find("Image").gameObject.GetComponent<Image>().sprite = item.Value.m_image;
                _btn.transform.Find("Image").gameObject.GetComponent<Image>().preserveAspect = true;
                _btn.GetComponent<Button>().onClick.AddListener(delegate { UpdateBuySellPanel(item.Key); });

                _count2++;
            }
            else
            {
                if (item.Value.m_useType == StuffType.ItemUseType.One)
                {
                    GameObject _btn = Instantiate(m_storBtn);

                    _btn.transform.SetParent(m_storContents[0].transform);
                    _btn.transform.localScale = new Vector3(1, 1, 1);

                    _btn.transform.Find("NameText").gameObject.GetComponent<Text>().text = item.Value.m_name;
                    _btn.transform.Find("ExplainText").gameObject.GetComponent<Text>().text = item.Value.m_explanation;
                    _btn.transform.Find("Image").gameObject.GetComponent<Image>().sprite = item.Value.m_image;
                    _btn.transform.Find("Image").gameObject.GetComponent<Image>().preserveAspect = true;
                    _btn.GetComponent<Button>().onClick.AddListener(delegate { UpdateBuySellPanel(item.Key); });

                    _count0++;
                }
                else
                {
                    GameObject _btn = Instantiate(m_storBtn);

                    _btn.transform.SetParent(m_storContents[1].transform);
                    _btn.transform.localScale = new Vector3(1, 1, 1);

                    _btn.transform.Find("NameText").gameObject.GetComponent<Text>().text = item.Value.m_name;
                    _btn.transform.Find("ExplainText").gameObject.GetComponent<Text>().text = item.Value.m_explanation;
                    _btn.transform.Find("Image").gameObject.GetComponent<Image>().sprite = item.Value.m_image;
                    _btn.transform.Find("Image").gameObject.GetComponent<Image>().preserveAspect = true;
                    _btn.GetComponent<Button>().onClick.AddListener(delegate { UpdateBuySellPanel(item.Key); });

                    _count1++;
                }
            }
        }
    }

    /// <summary>
    /// 아이템 구매
    /// </summary>
    public void BuyItem()
    {
        GManager.Instance.BuySetSlider(m_nowItemCode);
    }

    /// <summary>
    /// 아이템 판매
    /// </summary>
    public void SellItem()
    {
        GManager.Instance.SellSetSlider(m_nowItemCode);
    }


    /// <summary>
    /// 상점 스크롤 뷰 변경
    /// </summary>
    /// <param name="argIndex">아이템 정보 인덱스</param>
    public void ChangeStorView(int argIndex)
    {
        for (int i = 0; i < m_storScrollView.Length; i++)
        {
            m_storScrollView[i].SetActive(false);
        }

        m_storScrollView[argIndex].SetActive(true);
    }
}
