using UnityEngine;
using Random = UnityEngine.Random;
/// <summary>
/// SEManagerの機能をデバッグするスクリプト
/// </summary>
public class Debug_SEManager : MonoBehaviour
{
    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 150, 100), "PlayEffect"))
        {
            SEManager.Instance.PlaySE((SEType)Random.Range(0,2),new Vector3(Random.Range(-10,10),Random.Range(-10,10),Random.Range(-10,10)));
        }
    }
}