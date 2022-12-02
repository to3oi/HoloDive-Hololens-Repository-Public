using UniRx;
using UnityEngine;

public class Trigger_BackOfHandTap : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FingerTrigger"))
        {
            TouchArmToChangeTimeAxis.Instance.ClickObserver.OnNext(Unit.Default);
        }
    }
}