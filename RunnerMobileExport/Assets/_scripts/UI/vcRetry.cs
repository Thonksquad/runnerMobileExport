using UnityEngine.EventSystems;

public class vcRetry : Retry
{

    public override void OnPointerDown(PointerEventData eventData)
    {
        ActionSystem.onUpdateCoins();
        base.OnPointerDown(eventData);
    }

}
