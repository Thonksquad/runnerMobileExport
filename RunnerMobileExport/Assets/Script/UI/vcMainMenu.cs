using UnityEngine.EventSystems;

public class vcMainMenu : MainMenu
{

    public override void OnPointerDown(PointerEventData eventData)
    {
        ActionSystem.onUpdateCoins();
        base.OnPointerDown(eventData);
    }

}
