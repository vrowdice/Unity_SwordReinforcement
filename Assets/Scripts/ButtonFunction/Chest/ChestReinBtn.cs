using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ChestReinBtn : MonoBehaviour
{
    /// <summary>
    /// 버튼 이미지
    /// </summary>
    public Image m_image = null;

    /// <summary>
    /// 강화 텍스트
    /// </summary>
    public Text m_reintext = null;

    /// <summary>
    /// 갯수 텍스트
    /// </summary>
    public Text m_countText = null;

    /// <summary>
    /// 버튼 토글
    /// </summary>
    public Toggle m_toggle = null;

    /// <summary>
    /// 툴 코드
    /// </summary>
    public int m_toolRein = -1;

    /// <summary>
    /// 클릭 시
    /// </summary>
    public void Click()
    {
        if (m_toolRein <= -1)
        {
            GManager.Instance.Warning("존재하지 않습니다.");
            return;
        }
        GManager.Instance.m_chestManager.SelectToolRein(m_toolRein);
    }

    /// <summary>
    /// 버튼 셋팅
    /// </summary>
    /// <param name="argsprit">이미지 스프라이트</param>
    /// <param name="argText">텍스트</param>
    /// <param name="argToggle">토글</param>
    public void SetBtn(int argRein, Sprite argsprit, string argReinStr, string argCountStr, bool argToggle)
    {
        m_toolRein = argRein;
        m_image.sprite = argsprit;
        m_image.preserveAspect = true;
        m_reintext.text = string.Format(argReinStr);
        m_countText.text = string.Format(argCountStr);
        m_toggle.isOn = argToggle;
    }
}
