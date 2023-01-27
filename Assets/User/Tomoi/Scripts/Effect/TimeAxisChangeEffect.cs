using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

/// <summary>
/// 時間軸を変更するエフェクト
/// </summary>
public class TimeAxisChangeEffect : MonoBehaviour
{
    [SerializeField, Header("時間軸変更のエフェクト用マテリアル")]
    private Material effectMaterial;

    /// <summary>
    /// 時間軸を未来に変更するときに見せる色
    /// </summary>
    [SerializeField, ColorUsage(false, true)]
    private Color featureColor;

    /// <summary>
    /// 時間軸を過去に変更するときに見せる色
    /// </summary>
    [SerializeField, ColorUsage(false, true)]
    private Color pastColor;

    /// <summary>
    /// 壁の色を変えるときのY軸の高さ (Max x , Min Y)
    /// </summary>
    [SerializeField] private Vector2 WallHeight;

    /// <summary>
    /// 壁の色を変えるときの速さ 
    /// </summary>
    private float wallColorChangeSpeed;

    /// <summary>
    /// マテリアルのColorChangeElapsedTimeの値を計算するのに使用する変数
    /// </summary>
    private float tempColorChangeElapsedTime = 0;

    //マテリアルのプロパティID
    //波紋用のプロパティ
    private static readonly int IsHoloView = Shader.PropertyToID("_isHoloView");
    private static readonly int HoloViewColor = Shader.PropertyToID("_HoloViewColor");
    private static readonly int HoloViewTime = Shader.PropertyToID("_HoloViewTime");

    //壁用のプロパティ
    private static readonly int WallBaseColor = Shader.PropertyToID("_WallBaseColor");
    private static readonly int WallAfterColor = Shader.PropertyToID("_WallAfterColor");
    private static readonly int ColorChangeElapsedTime = Shader.PropertyToID("_ColorChangeElapsedTime");


    /// <summary>
    /// エフェクトの処理を停止させるためのUniTaskのCancellationTokenSource
    /// </summary>
    private CancellationTokenSource _cts;

    /// <summary>
    /// エフェクトの処理を停止させるためのUniTaskのCancellationToken
    /// </summary>
    private CancellationToken _ct;

    private void Start()
    {
        _cts = new CancellationTokenSource();
        _ct = _cts.Token;

        TouchArmToChangeTimeAxisManager.Instance.ClickResultObserver.Subscribe(_ =>
            {
                PlayEffect(UniTaskCancel()).Forget();
            })
            .AddTo(this);
    }

    /// <summary>
    /// 時間軸変更のエフェクトを再生する
    /// </summary>
    private async UniTask PlayEffect(CancellationToken _ctToken)
    {
        //マテリアルの初期化
        var mat = SpatialMeshManager.Instance.SpatialMeshMaterial;
        //時間軸変更時の波紋のプロパティを初期化
        mat.SetInt(IsHoloView, 1);
        mat.SetColor(HoloViewColor, GetColor().AfterColor);

        //壁のプロパティを初期化
        mat.SetColor(WallBaseColor, GetColor().BaseColor);
        mat.SetColor(WallAfterColor, GetColor().AfterColor);
        tempColorChangeElapsedTime = WallHeight.x;
        mat.SetFloat(ColorChangeElapsedTime, tempColorChangeElapsedTime);

        //1フレームの間にどのくらい値を変化させるか = 1フレーム / 移動量 * 実行時間
        wallColorChangeSpeed = Time.deltaTime / (WallHeight.x - WallHeight.y) * 1;

        //時間の更新
        float time = 0;
        while (time < 1)
        {
            //マテリアルの更新
            mat.SetFloat(HoloViewTime, time);
            tempColorChangeElapsedTime -= wallColorChangeSpeed;
            mat.SetFloat(ColorChangeElapsedTime, tempColorChangeElapsedTime);

            //キャンセルの通知が来たら処理を終了する
            _ctToken.ThrowIfCancellationRequested();

            time += Time.deltaTime;
            await UniTask.Yield();
        }

        //終了処理
        mat.SetInt(IsHoloView, 0);
        mat.SetColor(WallBaseColor, GetColor().AfterColor);
    }

    /// <summary>
    /// 現在の色 BaseColorと次に変更する色 AfterColorを取得する関数
    /// </summary>
    private (Color BaseColor, Color AfterColor) GetColor()
    {
        return TimeAxisManager.Instance.Axis switch
        {
            TimeAxisManager.axis.past => (pastColor, featureColor),
            TimeAxisManager.axis.future=> (featureColor, pastColor),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    /// <summary>
    /// UniTaskの処理をキャンセルし新しいキャンセルトークンを発行する
    /// </summary>
    private CancellationToken UniTaskCancel()
    {
        _cts.Cancel();
        _cts = new CancellationTokenSource();
        _ct = _cts.Token;
        return _ct;
    }
}