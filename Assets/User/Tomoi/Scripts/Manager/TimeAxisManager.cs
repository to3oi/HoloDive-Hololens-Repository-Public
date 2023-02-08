using System;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UniRx;
using UnityEngine;

public class TimeAxisManager : SingletonMonoBehaviour<TimeAxisManager>
{
    public enum axis
    {
        past,//過去
        future//未来
    }
    //public axis Axis { get;private set; }
    public axis Axis = axis.future;

    private Subject<axis> _axisSubject = new Subject<axis>();
    public IObservable<axis> TimeAxisObserver => _axisSubject;
    void Start()
    {
        //ダブルクリックを購読し、変更されたタイミングでステートを変更する
        TouchArmToChangeTimeAxisManager.Instance.ClickResultObserver.Subscribe(_ =>
        {
            GameManager.Instance.SetState(GameState.TimeShifted);
            ChangeTimeAxis();
        }).AddTo(this);
        //時間軸の変更を監視しsubjectの発行をする
        this.ObserveEveryValueChanged(manager => manager.Axis).Skip(1).Subscribe(_ =>
        {
            //SEの再生
            //左手が認識されていたら左手の位置で再生させる
            if (HandJointUtils.TryGetJointPose(TrackedHandJoint.MiddleMetacarpal, Handedness.Left, out MixedRealityPose pose))
            {
                
                SEManager.Instance.PlaySE(SEType.TimeAxisManager_TimeChange,pose.Position);
            }
            //なければカメラの位置で再生
            else
            {
                SEManager.Instance.PlaySE(SEType.TimeAxisManager_TimeChange,Camera.main.transform.position);
            }
            
            _axisSubject.OnNext(Axis);
        }).AddTo(this);
    }

    /// <summary>
    /// 今のステートが過去なら未来、未来なら過去に変更する
    /// </summary>
    void ChangeTimeAxis()
    {
        Axis = Axis switch
        {
            axis.future => axis.past,
            axis.past => axis.future,
            _ => Axis
        };
    }
}