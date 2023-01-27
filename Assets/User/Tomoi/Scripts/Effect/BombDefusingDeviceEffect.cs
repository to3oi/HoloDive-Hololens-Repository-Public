using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// 爆弾起動時に表示するエフェクトを登録するスクリプト
/// </summary>
[RequireComponent(typeof(DefaultObserverEventHandler))]
public class BombDefusingDeviceEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem _warningParticleSystem;
    private bool isPlayedEffct = false;

    void Awake()
    {
        //VuforiaのEventHandlerにイベントの登録
        var handler = GetComponent<DefaultObserverEventHandler>();
        handler.OnTargetFound.AddListener(WarningPlayEffect);
    }
    
    /// <summary>
    /// 爆弾の起動を警告するエフェクトを一度だけ生成する
    /// </summary>
    private void WarningPlayEffect()
    {
        if(isPlayedEffct){return;}

        isPlayedEffct = true;
        Instantiate(_warningParticleSystem, Camera.main.transform.position + Vector3.down * 0.5f ,quaternion.identity);
    }
}