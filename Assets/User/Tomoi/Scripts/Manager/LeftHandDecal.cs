using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UniRxExt = UniRx.ObservableExtensions;

/// <summary>
/// 左手の手の甲にあるデカールを生成し、カメラから見える角度によっては非表示にする
/// </summary>
[RequireComponent(typeof(TimeAxisChangeEffect))]
public class LeftHandDecal : SingletonMonoBehaviour<LeftHandDecal>
{
    //表示するオブジェクトをインスペクターで指定
    [SerializeField] private GameObject _colliderRoot;
    private GameObject _instanceColliderRoot;

    [SerializeField] private Vector3 _colliderScale;
    [SerializeField] private Vector3 _positionOffset;

    [SerializeField, Range(0.0f, 90.0f)] private float facingThreshold = 80.0f;

    
    void Start()
    {
        //大きさを調整
        _instanceColliderRoot = Instantiate(_colliderRoot, transform);
        _instanceColliderRoot.transform.localScale = _colliderScale;
    }
    
    void Update()
    {
        //手の甲の位置を取得
        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Left, out MixedRealityPose pose))
        {
            //手の甲を向けている時
            if (Vector3.Angle(pose.Up, CameraCache.Main.transform.forward) > facingThreshold)
            {
                //表示
                _instanceColliderRoot.SetActive(true);

                //座標を指定
                _instanceColliderRoot.transform.position = pose.Position + _positionOffset;
                _instanceColliderRoot.transform.rotation = pose.Rotation * Quaternion.Euler(90f,0f,0f);
            }
            else
            {
                //手の甲が見えないなら非表示にする
                _instanceColliderRoot.SetActive(false);
            }
        }
        else
        {
            //手の甲が取得できていないときは非表示
            _instanceColliderRoot.SetActive(false);
        }
    }
}