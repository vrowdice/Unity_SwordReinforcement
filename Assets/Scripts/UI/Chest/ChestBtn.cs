using UnityEngine;
using UnityEngine.UI;

public class ChestBtn : MonoBehaviour
{
    /// <summary>
    /// 버튼 이미지
    /// </summary>
    public Image image = null;

    /// <summary>
    /// 버튼 텍스트
    /// </summary>
    public Text text = null;

    /// <summary>
    /// 버튼 토글
    /// </summary>
    public Toggle toggle = null;

    /// <summary>
    /// 버튼 코드 
    /// </summary>
    public int code = 0;

    /// <summary>
    /// 클릭시
    /// </summary>
    public void Click()
    {
        if(code <= 0)
        {
            var gameManager = GameManager.Instance;
            gameManager?.Warning("존재하지 않습니다.");
            return;
        }
        
/*        var chestManager = GameManager.Instance?.chestManager;
        chestManager?.ChestBtn(code);*/
    }

    /// <summary>
    /// 버튼 셋팅
    /// </summary>
    /// <param name="argsprit">이미지 스프라이트</param>
    /// <param name="argText">텍스트</param>
    /// <param name="argToggle">토글</param>
    public void SetBtn(int argCode, Sprite argsprit, string argText, bool argToggle)
    {
        code = argCode;
        image.sprite = argsprit;
        image.preserveAspect = true;
        text.text = string.Format(argText);
        toggle.isOn = argToggle;
    }
}
