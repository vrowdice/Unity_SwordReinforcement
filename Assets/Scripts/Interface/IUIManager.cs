using UnityEngine;

public interface IUIManager
{
    Transform CanvasTrans { get; }

    void UpdateAllMainText();

    void Initialize(GameManager argGameManager, GameDataManager argGameDataManager);
    
    void BuySetSlider(int itemCode);
    void SellSetSlider(int itemCode);
}
