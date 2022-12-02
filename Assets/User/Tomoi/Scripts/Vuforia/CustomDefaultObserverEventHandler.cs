public class CustomDefaultObserverEventHandler : DefaultObserverEventHandler
{
    protected override void OnTrackingFound()
    {
        base.OnTrackingFound();
        OnTargetFound?.Invoke();
    }
    protected override void OnTrackingLost()
    {
        base.OnTrackingLost();
        OnTargetLost?.Invoke();
    }
}
