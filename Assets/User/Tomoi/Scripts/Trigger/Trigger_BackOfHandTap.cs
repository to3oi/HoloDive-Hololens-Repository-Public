using UniRx;
using UnityEngine;

/// <summary>
/// 左手の手の甲をタップしたときにTouchArmToChangeTimeAxisManagerに通知するスクリプト
/// </summary>
public class Trigger_BackOfHandTap : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FingerTrigger"))
        {
            TouchArmToChangeTimeAxisManager.Instance.ClickObserver.OnNext(Unit.Default);
        }
    }
}