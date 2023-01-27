using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

/// <summary>
/// 両手の人差し指に判定用のコライダーを追従させるスクリプト
/// </summary>
public class Triger_RightIndexFinger : MonoBehaviour
{
    [SerializeField] private GameObject _rightHandCollider;
    private GameObject _rightHandColliderRoot;
    private GameObject _leftHandColliderRoot;
    [SerializeField] private Vector3 _scale;
        
    private void Start()
    {
        //判定用のコライダーを生成
        _rightHandColliderRoot = Instantiate(_rightHandCollider, transform);
        _rightHandColliderRoot.transform.localScale = _scale;
        
        _leftHandColliderRoot = Instantiate(_rightHandCollider, transform);
        _leftHandColliderRoot.transform.localScale = _scale;
    }

    private void Update()
    {
        //右人差し指にコライダーを追従させる
        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Right, out MixedRealityPose pose1))
        {
            //表示
            _rightHandColliderRoot.SetActive(true);

            //座標を指定
            _rightHandColliderRoot.transform.position = pose1.Position;
            _rightHandColliderRoot.transform.rotation = pose1.Rotation;
        }
        else
        {
            //右手の位置が取得できないときは非表示
            _rightHandColliderRoot.SetActive(false);
        }
        
        //左人差し指にコライダーを追従させる
        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Left, out MixedRealityPose pose2))
        {
            //表示
            _leftHandColliderRoot.SetActive(true);

            //座標を指定
            _leftHandColliderRoot.transform.position = pose2.Position;
            _leftHandColliderRoot.transform.rotation = pose2.Rotation;
        }
        else
        {
            //左手の位置が取得できないときは非表示
            _leftHandColliderRoot.SetActive(false);
        }
    }
}
