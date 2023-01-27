using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// MasterPCCancellationDeviceManagerの入力が正解したときにマスターPCに表示されている画像を切り替えるスクリプト
/// </summary>
public class MasterPCSuccessViewChange : MonoBehaviour
{
    [SerializeField] private Sprite _SuccessSprite;
    [SerializeField] private Image _MasterPCImage;

    public void UpdateMonitor()
    {
        _MasterPCImage.sprite = _SuccessSprite;
    }
}
