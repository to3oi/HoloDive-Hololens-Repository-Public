using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>
/// SEManagerで使用されるオブジェクトプールのクラス
/// </summary>
public class SEObject : MonoBehaviour
{
    private AudioSource _audioSource;
    private void Awake()
    {
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.playOnAwake = false;
    }

    /// <summary>
    /// 引数のClipを引数のPositionの位置で一度だけ再生する
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="position"></param>
    public void PlaySE(AudioClip clip,Vector3 position)
    {
        _audioSource.clip = clip;
        _audioSource.Play();
        transform.position = position;
        SEEnd();
    }

    /// <summary>
    /// SEの再生終了を検知しオブジェクトプールにリリースする
    /// </summary>
    private async UniTask SEEnd()
    {
        //再生を開始してから最初の isPlaying == false を検知してからオブジェクトプールにリリースする
        await UniTask.WaitUntil(() => _audioSource.isPlaying);
        await UniTask.WaitUntil(() => !_audioSource.isPlaying);
        Release();
    }
    
    /// <summary>
    /// オブジェクトプールにリリースする
    /// </summary>
    void Release()
    {
        SEManager.Instance.ReleaseSEObject(this);
    }
}
