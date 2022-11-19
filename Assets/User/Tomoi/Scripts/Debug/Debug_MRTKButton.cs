using UniRx;
using UnityEngine;

public class Debug_MRTKButton : MonoBehaviour
{
    [ContextMenu("PushButton")]
    public void PushButton()
    {
        TouchArmToChangeTimeAxis.Instance.ClickObserver.OnNext(Unit.Default);
        TouchArmToChangeTimeAxis.Instance.ClickObserver.OnNext(Unit.Default);
    }
}
