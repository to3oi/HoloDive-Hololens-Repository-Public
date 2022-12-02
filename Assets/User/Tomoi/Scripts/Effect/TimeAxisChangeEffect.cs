using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class TimeAxisChangeEffect : MonoBehaviour
{
    [SerializeField, Header("時間軸変更のエフェクト用マテリアル")]
    private Material effectMaterial;

    [SerializeField, ColorUsage(false, true)]
    private Color featureColor;

    [SerializeField, ColorUsage(false, true)]
    private Color pastColor;

    void Start()
    {
        TouchArmToChangeTimeAxis.Instance.ClickResultObserver.Subscribe(_ => { PlayEffect().Forget(); }).AddTo(this);
    }

    async UniTask PlayEffect()
    {
        //マテリアルの初期化
        var mat = SpatialMeshManager.Instance.SpatialMeshMaterial;
        mat.SetInt("_isHoloView", 1);
        mat.SetColor("_HoloColor", GetColor());

        //時間の更新
        // TODO:同時に複数回実行した際に表示がおかしくなるので要修正
        float _time = 0;
        while (_time < 1)
        {
            mat.SetFloat("_HoloViewTime", _time);

            _time += Time.deltaTime;
            await UniTask.Yield();
        }

        //終了処理
        mat.SetInt("_isHoloView", 0);
    }

    Color GetColor()
    {
        return TimeAxisManager.Instance.Axis switch
        {
            TimeAxisManager.axis.future => featureColor,
            TimeAxisManager.axis.past => pastColor,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}