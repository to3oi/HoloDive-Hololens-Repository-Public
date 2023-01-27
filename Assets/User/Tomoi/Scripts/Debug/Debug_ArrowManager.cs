using UnityEngine;

/// <summary>
/// ArrowManagerが機能するかデバッグするスクリプト
/// </summary>
public class Debug_ArrowManager : MonoBehaviour
{
    [SerializeField] private GameObject contentObject;
    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 150, 100), "SetupArrow"))
        {
            ArrowManager.Instance.SetupArrow(contentObject);
        }
    }
}
