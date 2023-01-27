using UnityEngine;
/// <summary>
/// 手の甲のトリガーに触れる処理をデバッグするスクリプト
/// </summary>
public class Debug_TapGameObjectChangeColor : MonoBehaviour
{
    private MeshRenderer _renderer;

    private void Start()
    {
        _renderer = GetComponent<MeshRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Debug"))
        {
            _renderer.material.color = Color.red;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Debug"))
        {
            _renderer.material.color = Color.white;
        }
    }
}