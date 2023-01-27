using UniRx;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// 手の甲に表示するデカールを更新するスクリプト
/// </summary>
[RequireComponent(typeof(DecalProjector))]
public class TimeChangeUI : MonoBehaviour
{
    [SerializeField, Header("未来のテクスチャ")] private Texture featureTexture;
    [SerializeField, Header("過去のテクスチャ")] private Texture pastTexture;

    private Material _decalMaterial;
    private DecalProjector _decalProjector;

    private void Start()
    {
        _decalProjector = GetComponent<DecalProjector>();
        _decalMaterial = new Material(_decalProjector.material);
        _decalProjector.material = _decalMaterial;
        
        //setup decal texture
        UpdateDecalTexture(TimeAxisManager.Instance.Axis);
        
        //時間軸の更新を購読し、変更があればテクスチャを更新する
        TimeAxisManager.Instance.TimeAxisObserver.Subscribe(axis => { UpdateDecalTexture(axis); }).AddTo(this);
    }

    /// <summary>
    /// 手の甲に表示するデカールのテクスチャを変更
    /// </summary>
    /// <param name="axis"></param>
    private void UpdateDecalTexture(TimeAxisManager.axis axis)
    {
        switch (axis)
        {
            case TimeAxisManager.axis.future:
                _decalMaterial.SetTexture("Base_Map", featureTexture);
                break;
            case TimeAxisManager.axis.past:
                _decalMaterial.SetTexture("Base_Map", pastTexture);
                break;
        }
    }
}