using UnityEngine;

public class ImageRecognition : MonoBehaviour
{
    private Transform ts = null;

    // Start is called before the first frame update
    void Start()
    {
        ts = this.gameObject.transform;
    }

    /// <summary>
    /// プレイヤーが画像認識したら、
    /// 画像に対応したオブジェクトの座標を返す
    /// </summary>
    /// <returns>  </returns>
    public Vector3? GetObjectPosition() { return gameObject.activeSelf ? ts.position : (Vector3?)null; }
}
