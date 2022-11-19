using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

public class Triger_RightIndexFinger : MonoBehaviour
{
    [SerializeField] private GameObject _rightHandCollider;
    private GameObject _rightHandColliderRoot;
    [SerializeField] private Vector3 _scale;
        
    private void Start()
    {
        _rightHandColliderRoot = Instantiate(_rightHandCollider, transform);
        _rightHandColliderRoot.transform.localScale = _scale;
    }

    private void Update()
    {
        //右人差し指にコライダーを追従させる
        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Right, out MixedRealityPose pose))
        {
            //表示
            _rightHandColliderRoot.SetActive(true);

            //座標を指定
            _rightHandColliderRoot.transform.position = pose.Position;
            _rightHandColliderRoot.transform.rotation = pose.Rotation;
        }
        else
        {
            _rightHandColliderRoot.SetActive(false);
        }
    }
}
