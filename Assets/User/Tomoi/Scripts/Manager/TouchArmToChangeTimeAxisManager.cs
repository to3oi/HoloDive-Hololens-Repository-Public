using System;
using UniRx;

/// <summary>
/// 手の甲を2回タップしたときに通知するマネージャー
/// </summary>
public class TouchArmToChangeTimeAxisManager : SingletonMonoBehaviour<TouchArmToChangeTimeAxisManager>
{
    private Subject<Unit> _clickSubject = new Subject<Unit>();
    /// <summary>
    /// 手の甲のタップを購読する
    /// </summary>
    public IObserver<Unit> ClickObserver => _clickSubject;

    private Subject<Unit> _clickResultObserverSubject = new Subject<Unit>();
    /// <summary>
    /// 2回タップを通知する
    /// </summary>
    public IObservable<Unit> ClickResultObserver => _clickResultObserverSubject;
    void Start()
    {
        //手の甲を2回タップした時の処理を登録
        var tempObserver = _clickSubject
            .Select(_ => false).Take(1) // シングルならfalse。一つだけ通す
            .Concat(_clickSubject.Select(_ => true) // 2回タップならtrue
                .Take(TimeSpan.FromSeconds(0.3f)).Take(1)) // 2回タップを判定
            .RepeatSafe(); // 判定が終わったら繰り返し
        
        //上の判定がtrueのときにClickResultObserverを購読しているものに通知
        tempObserver.Where(b => b).Skip(1).Subscribe(_ =>
        {
            _clickResultObserverSubject.OnNext(Unit.Default);
        }).AddTo(this);
    }
}
