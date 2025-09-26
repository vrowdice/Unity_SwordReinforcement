using UnityEngine;
using TMPro;

/// <summary>
/// 모든 패널의 기본 클래스
/// 공통 기능들을 관리하여 코드 중복을 줄임
/// </summary>
public abstract class BasePanel : MonoBehaviour, IUIPanel
{
    protected GameDataManager gameDataManager = null;
    protected IUIManager mainUiManager = null;
    protected GameManager gameManager = null;

    // 프로퍼티들
    public GameDataManager GameDataManager => gameDataManager;
    public IUIManager MainUiManager => mainUiManager;
    protected GameManager GameManager => gameManager;

    /// <summary>
    /// 패널이 열릴 때 호출되는 공통 로직
    /// </summary>
    /// <param name="argDataManager">게임 데이터 매니저</param>
    /// <param name="argUIManager">UI 매니저</param>
    public virtual void OnOpen(GameDataManager argDataManager, IUIManager argUIManager)
    {
        // null 체크
        if (argDataManager == null || argUIManager == null)
        {
            Debug.LogError("GameDataManager or IUIManager is null in BasePanel.OnOpen");
            return;
        }

        gameDataManager = argDataManager;
        mainUiManager = argUIManager;
        gameManager = GameManager.Instance; // GameManager 인스턴스 캐시

        // 패널 활성화
        gameObject.SetActive(true);

        // 하위 클래스에서 구현할 초기화 로직 (패널 설정 포함)
        OnPanelOpen();
    }

    /// <summary>
    /// 패널이 닫힐 때 호출되는 공통 로직
    /// </summary>
    public virtual void OnClose()
    {
        OnPanelClose();
    }

    /// <summary>
    /// 하위 클래스에서 구현할 패널 열기 로직
    /// </summary>
    protected virtual void OnPanelOpen()
    {
        // 하위 클래스에서 오버라이드
    }

    /// <summary>
    /// 하위 클래스에서 구현할 패널 닫기 로직
    /// </summary>
    protected virtual void OnPanelClose()
    {
        // 하위 클래스에서 오버라이드
    }

    /// <summary>
    /// 경고 메시지 표시
    /// </summary>
    /// <param name="message">표시할 메시지</param>
    protected void ShowWarning(string message)
    {
        gameManager?.Warning(message);
    }

    /// <summary>
    /// UI 업데이트 요청
    /// </summary>
    protected void RequestUIUpdate()
    {
        mainUiManager?.UpdateAllMainText();
    }
} 