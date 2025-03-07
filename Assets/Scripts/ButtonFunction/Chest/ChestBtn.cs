using UnityEngine;
using UnityEngine.UI;

public class ChestBtn : MonoBehaviour
{
    /// <summary>
    /// 버튼 이미지
    /// </summary>
    public Image m_image = null;

    /// <summary>
    /// 버튼 텍스트
    /// </summary>
    public Text m_text = null;

    /// <summary>
    /// 버튼 토글
    /// </summary>
    public Toggle m_toggle = null;

    /// <summary>
    /// 버튼 코드
    /// </summary>
    public int m_code = 0;

    /// <summary>
    /// 클릭시
    /// </summary>
    public void Click()
    {
        if(m_code <= 0)
        {
            GManager.Instance.Warning("존재하지 않습니다.");
            return;
        }
        GManager.Instance.m_chestManager.ChestBtn(m_code);
    }

    /// <summary>
    /// 버튼 셋팅
    /// </summary>
    /// <param name="argsprit">이미지 스프라이트</param>
    /// <param name="argText">텍스트</param>
    /// <param name="argToggle">토글</param>
    public void SetBtn(int argCode, Sprite argsprit, string argText, bool argToggle)
    {
        m_code = argCode;
        m_image.sprite = argsprit;
        m_image.preserveAspect = true;
        m_text.text = string.Format(argText);
        m_toggle.isOn = argToggle;
    }
}
