using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Microsoft.MixedReality.SceneUnderstanding.Samples.Unity;
using UnityEngine;

/// <summary>
/// 時間軸を変更するときのエフェクトをデバッグするスクリプト
/// </summary>
public class Debug_TimeAxisShader : MonoBehaviour
{
    [SerializeField] private Material mat;

    [SerializeField, ColorUsage(false, true)]
    private Color featureColor;

    [SerializeField, ColorUsage(false, true)]
    private Color pastColor;

    [SerializeField] private bool featureBool = false;

    /// <summary>
    /// 壁の色を変えるときのY軸の高さMax x , Min Y
    /// </summary>
    [SerializeField] private Vector2 YAxis;

    /// <summary>
    /// 壁の色を変えるときの速さ 
    /// </summary>
    private float wallColorChangeSpeed;

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
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 150, 100), "PlayEffect"))
        {
            featureBool = !featureBool;
            PlayEffect(SceneUnderstandingManager.Instance._suMeshRenderers, UniTaskCancel()).Forget();
        }
    }

    async UniTask PlayEffect(List<MeshRenderer> collection, CancellationToken _ctToken)
    {
        //1フレームの間にどのくらい値を変化させるか = 1フレーム / 移動量 * 実行時間
        wallColorChangeSpeed = Time.deltaTime / (YAxis.x - YAxis.y) * 0.1f;

        foreach (MeshRenderer mesh in collection)
        {
            //マテリアルのセットアップ
            mesh.material = new Material(mat);
            mesh.material.SetInt(IsHoloView, 1);
            //色の変更
            mesh.material.SetColor(HoloViewColor, GetColor().AfterColor);

            //壁
            mesh.material.SetColor(WallBaseColor, GetColor().BaseColor);
            mesh.material.SetColor(WallAfterColor, GetColor().AfterColor);

            tempColorChangeElapsedTime = YAxis.x;
            mesh.material.SetFloat(ColorChangeElapsedTime, tempColorChangeElapsedTime);
        }

        float _time = 0;
        while (_time < 1)
        {
            foreach (MeshRenderer mesh in collection)
            {
                mesh.material.SetFloat(HoloViewTime, _time);

                tempColorChangeElapsedTime -= wallColorChangeSpeed;
                mesh.material.SetFloat(ColorChangeElapsedTime, tempColorChangeElapsedTime);
            }

            //キャンセルの通知が来たら終了処理を実行する
            _ctToken.ThrowIfCancellationRequested();

            _time += Time.deltaTime;
            await UniTask.Yield();
        }

        foreach (MeshRenderer mesh in collection)
        {
            mesh.material.SetInt(IsHoloView, 0);
            mesh.material.SetColor(WallBaseColor, GetColor().AfterColor);
        }
    }

    (Color AfterColor, Color BaseColor) GetColor()
    {
        return featureBool ? (featureColor, pastColor) : (pastColor, featureColor);
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