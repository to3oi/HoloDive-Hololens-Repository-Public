using UnityEngine;
using UniRx;
using DG.Tweening;

/// <summary>
/// PasscodeCubeの生成と破棄の演出と破壊時にMasterPCCancellationDeviceManagerへ通知するクラス
/// </summary>
public class PasscodeCube : MonoBehaviour
{
    /// <summary>
    /// 自身のオブジェクトの色を指定
    /// </summary>
    [SerializeField] private PasscodeCubeColorEnum _passcodeCubeColorEnum;

    [SerializeField] private GameObject ParentObject;

    /// <summary>
    /// 破棄するときのパーティクル
    /// </summary>
    [SerializeField] private ParticleSystem _destroyParticle;

    /// <summary>
    /// オブジェクトがアクティブかどうか
    /// </summary>
    private bool isActive = true;

    //デフォルトの値を保持
    private Vector3 defoultScale;
    private Vector3 defoultPosition;

    private void Start()
    {
        defoultScale = transform.localScale;
        defoultPosition = transform.position;

        ParentObject.transform.DOMoveY(defoultPosition.y + 0.05f, 1f).SetDelay(Random.value)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
        
        transform.DOLocalRotate(new Vector3(360,300,0), 12f,RotateMode.FastBeyond360)
            .SetDelay(Random.value)
            .SetLoops(-1, LoopType.Incremental );

        //PasscodeCubeの再生性を受け取ったときにこのCubeが破棄されていたら再生成する
        MasterPCCancellationDeviceManager.Instance.RegenerationPasscode
            .Delay(System.TimeSpan.FromSeconds(1.0f))
            .Subscribe(_ =>
            {
                if (!isActive)
                {
                    Generate();
                }
            }).AddTo(this);
    }

    /// <summary>
    /// 自身を生成するアニメーションを再生する
    /// </summary>
    private void Generate()
    {
        //破棄と同時に呼ばれてしまうので一秒遅らせる
        isActive = true;
        transform.DOScale(defoultScale, 0.5f).SetEase(Ease.OutBack);
    }

    /// <summary>
    /// 自身を破棄するアニメーションを再生する
    /// </summary>
    private void Discarding()
    {
        _destroyParticle.Play();
        isActive = false;
        transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.OutBack);
    }

    private void OnTriggerEnter(Collider other)
    {
        //自身に指が触れたらMasterPCCancellationDeviceManagerに通知し自身を破棄する
        if (isActive)
        {
            MasterPCCancellationDeviceManager.Instance.RegistrationPasscode.OnNext(_passcodeCubeColorEnum);
            Discarding();
        }
    }
}