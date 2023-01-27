/// <summary>
/// VuforiaのEventHandlerをカスタムするためのラッパークラス
/// </summary>
public class CustomDefaultObserverEventHandler : DefaultObserverEventHandler
{
    protected override void OnTrackingFound()
    {
        base.OnTrackingFound();
    }
    protected override void OnTrackingLost()
    {
        base.OnTrackingLost();
    }
}
