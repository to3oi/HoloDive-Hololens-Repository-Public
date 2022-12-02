using UnityEngine;

public class TestUIManager : MonoBehaviour
{
    void Start()
    {
        UIManager.Instance.StockText("qwertyui", 3.0f);
        UIManager.Instance.StockText("asdfghj", 5.0f);
        UIManager.Instance.StockText("zxcvbnm,", 1.0f);
    }
}