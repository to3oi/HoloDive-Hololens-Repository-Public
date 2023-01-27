using System;
using UniRx;

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
        TouchArmToChangeTimeAxisManager.Instance.ClickResultObserver.Skip(1).Subscribe(_ =>
        {
            GameManager.Instance.SetState(GameState.TimeShifted);
            ChangeTimeAxis();
        }).AddTo(this);
        //時間軸の変更を監視しsubjectの発行をする
        this.ObserveEveryValueChanged(manager => manager.Axis).Subscribe(_ =>
        {
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