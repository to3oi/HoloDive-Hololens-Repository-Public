using System;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UniRx;
using ObservableExtensions = UniRx.ObservableExtensions;
using UniRxExt = UniRx.ObservableExtensions;

[RequireComponent(typeof(TimeAxisChangeEffect))]
public class TouchArmToChangeTimeAxis : SingletonMonoBehaviour<TouchArmToChangeTimeAxis>
{
    //表示するオブジェクトを定義
    [SerializeField] private GameObject _colliderRoot;
    private GameObject _instanceColliderRoot;

    [SerializeField] private Vector3 _colliderScale;
    [SerializeField] private Vector3 _positionOffset;

    [SerializeField, Range(0.0f, 90.0f)] private float facingThreshold = 80.0f;
    private Subject<Unit> _clickSubject = new Subject<Unit>();
    public IObserver<Unit> ClickObserver => _clickSubject;

    private Subject<Unit> _clickResultObserverSubject = new Subject<Unit>();
    public IObservable<Unit> ClickResultObserver => _clickResultObserverSubject;
    
    void Start()
    {
        //大きさを調整
        _instanceColliderRoot = Instantiate(_colliderRoot, transform);
        _instanceColliderRoot.transform.localScale = _colliderScale;

        //ダブルクリックの処理
        var tempObserver = _clickSubject
            .Select(_ => false).Take(1) // シングルならfalse。一つだけ通す
            .Concat(_clickSubject.Select(_ => true) // ダブルクリックならtrue
                .Take(TimeSpan.FromSeconds(0.3f)).Take(1)) // ダブルクリック判定
            .RepeatSafe(); // 判定が終わったら繰り返し
        ObservableExtensions.Subscribe(tempObserver.Where(b => b), _ =>
        {
            _clickResultObserverSubject.OnNext(Unit.Default);
        }).AddTo(this);
        
        //TimeAxisManage側で↓を実装し、ステートを切り替える
        //TouchArmToChangeTimeAxis.Instance._clickResult.Subscribe(_ => { }).AddTo(this);
    }
    
    void Update()
    {
        //手の甲の位置を取得
        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Left, out MixedRealityPose pose))
        {
            //手の甲を向けていたら
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
            _instanceColliderRoot.SetActive(false);
        }
    }
}