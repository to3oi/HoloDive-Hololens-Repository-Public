using UniRx;
using UnityEngine;

/// <summary>
/// 手の甲を2回叩いたときの処理をデバッグするスクリプト
/// </summary>
public class Debug_TouchArmToChangeTimeAxis : MonoBehaviour
{
    [ContextMenu("PushButton")]
    public void PushButton()
    {
        TouchArmToChangeTimeAxisManager.Instance.ClickObserver.OnNext(Unit.Default);
        TouchArmToChangeTimeAxisManager.Instance.ClickObserver.OnNext(Unit.Default);
    }
}
