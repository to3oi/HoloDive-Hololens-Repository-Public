using UnityEngine;
using Cysharp.Threading.Tasks;

public class VuforiaCameraParentChange : MonoBehaviour
{
    private GameObject vuforiaCamera;
    async void Start()
    {
        await UniTask.Delay(1000);
        
        vuforiaCamera = GameObject.Find("TextureBufferCamera");

        Debug.Log($"TextureBufferCamera find is {vuforiaCamera}");
        vuforiaCamera.transform.parent = Camera.main.transform;
        vuforiaCamera.transform.position = Vector3.zero;
    }
}
