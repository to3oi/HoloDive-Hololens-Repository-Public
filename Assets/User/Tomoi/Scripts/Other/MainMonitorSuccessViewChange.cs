using UnityEngine;
using UnityEngine.UI;

public class MainMonitorSuccessViewChange : MonoBehaviour
{
    [SerializeField] private Sprite successSprite;
    [SerializeField] private Image mainMonitorImage;

    public void UpdateMonitor()
    {
        mainMonitorImage.sprite = successSprite;
    }
}
