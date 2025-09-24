using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ChestReinBtn : MonoBehaviour
{
    /// <summary>
    /// 버튼 이미지
    /// </summary>
    public Image image = null;

    /// <summary>
    /// 강화 텍스트
    /// </summary>
    public Text reintext = null;

    /// <summary>
    /// 갯수 텍스트
    /// </summary>
    public Text countText = null;

    /// <summary>
    /// 버튼 토글
    /// </summary>
    public Toggle toggle = null;

    /// <summary>
    /// 툴 코드
    /// </summary>
    public int toolRein = -1;

    /// <summary>
    /// 클릭 시
    /// </summary>
    public void Click()
    {
        if (toolRein <= -1)
        {
            GameManager.Instance.Warning("존재하지 않습니다.");
            return;
        }
        GameManager.Instance.chestManager.SelectToolRein(toolRein);
    }

    /// <summary>
    /// 버튼 셋팅
    /// </summary>
    /// <param name="argsprit">이미지 스프라이트</param>
    /// <param name="argText">텍스트</param>
    /// <param name="argToggle">토글</param>
    public void SetBtn(int argRein, Sprite argsprit, string argReinStr, string argCountStr, bool argToggle)
    {
        toolRein = argRein;
        image.sprite = argsprit;
        image.preserveAspect = true;
        reintext.text = string.Format(argReinStr);
        countText.text = string.Format(argCountStr);
        toggle.isOn = argToggle;
    }
}
