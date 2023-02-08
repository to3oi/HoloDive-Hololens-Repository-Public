using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;

/// <summary>
/// PasscodeCubeを管理し、MasterPCの画面の切り替えをするクラス
/// </summary>
public class MasterPCCancellationDeviceManager : SingletonMonoBehaviour<MasterPCCancellationDeviceManager>
{
    /// <summary>
    /// 正解のパスコード
    /// </summary>
    [SerializeField] private List<PasscodeCubeColorEnum> _CorrectAnswerPasscode;

    //パスコードを保存
    [SerializeField] private List<PasscodeCubeColorEnum> _RegistrationPasscodeList = new List<PasscodeCubeColorEnum>();


    //メインモニターを取得
    [SerializeField] private MasterPCSuccessViewChange _MasterPC;

    //UniRx
    //パスコードの登録を外部から購読するもの
    private Subject<PasscodeCubeColorEnum> _RegistrationPasscode = new Subject<PasscodeCubeColorEnum>();
    public IObserver<PasscodeCubeColorEnum> RegistrationPasscode => _RegistrationPasscode;


    //Passcodeを再生成するもの
    private Subject<Unit> _RegenerationPasscode = new Subject<Unit>();
    public IObservable<Unit> RegenerationPasscode => _RegenerationPasscode;

    void Start()
    {
        _RegistrationPasscode.Subscribe(async passcodeCubeColorEnum =>
        {
            //パスを登録
            if (true)
            {
                _RegistrationPasscodeList.Add(passcodeCubeColorEnum);
            }

            //4桁目で登録内容を確認し、合っていれば正解、間違っていればPasscodeCubeを再生成し登録内容を破棄
            if (4 <= _RegistrationPasscodeList.Count)
            {
                //パスコードが正解ならtrueが返る
                bool answer = true;
                for (int i = 0; i < 4; i++)
                {
                    if (_CorrectAnswerPasscode[i] != _RegistrationPasscodeList[i])
                    {
                        answer = false;
                    }
                }

                //正解ならMasterPCのロックを解除する
                if (answer)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(1));
                    //SEの再生
                    SEManager.Instance.PlaySE(SEType.MasterPCCancellationDeviceManager_Success,_MasterPC.transform.position);
                    _MasterPC.UpdateMonitor();
                }
                //不正解ならPasscodeCubeを再生成し、入力された情報をクリアする
                else
                {
                    _RegenerationPasscode.OnNext(Unit.Default);
                    _RegistrationPasscodeList.Clear();
                }
            }
        }).AddTo(this);
    }
}