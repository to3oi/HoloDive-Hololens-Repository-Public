using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;

/// <summary>
/// 生成したコンテンツの方向に回転する矢印を生成する
/// カメラがコンテンツを司会の中心に入れた際にこの矢印を破棄する
/// </summary>
public class SpinArrowDirectionContent : MonoBehaviour
{
    /// <summary>
    /// 矢印の前方向のベクトル
    /// </summary>
    private Vector3 forward = Vector3.left;

    /// <summary>
    /// 矢印の大きさのデフォルト値
    /// </summary>
    private Vector3 scale = new Vector3(0.025f, 0.025f, 0.025f);

    private GameObject contentGameObject;

    private void Update()
    {
        //contentGameObjectの方向に自身を向ける

        //contentへの向きベクトル
        var dir = contentGameObject.transform.position - transform.position;
        //contentの方向への回転
        var lookAtRotation = Quaternion.LookRotation(dir, Vector3.up);

        //回転を補正
        var offsetRotation = Quaternion.FromToRotation(forward, Vector3.forward);
        transform.rotation = lookAtRotation * offsetRotation;
    }

    /// <summary>
    /// 矢印の処理を開始させる
    /// </summary>
    /// <param name="tempContentGameObject">矢印を向けるcontent自身のオブジェクト</param>
    public async void StartArrowProcess(GameObject tempContentGameObject)
    {
        contentGameObject = tempContentGameObject;

        //ArrowManagerのインスタンスを保持
        var arrowManager = ArrowManager.Instance;

        //矢印の表示が終わるまで待機
        await ShowArrow();

        //視界内にcontentGameObjectが存在するまで待機
        await UniTask.WaitUntil(() => arrowManager.IsVisibleContent(contentGameObject));

        //矢印の非表示アニメーションが終わるまで待機
        await HideArrow();

        //処理が終わったときに自身を削除する
        Destroy(gameObject);
    }

    /// <summary>
    /// 矢印を表示する
    /// アニメーション
    /// </summary>
    private async UniTask ShowArrow()
    {
        await transform.DOScale(scale, 0.5f).SetEase(Ease.OutBack);
    }

    /// <summary>
    /// 矢印を非表示にする
    /// アニメーション
    /// </summary>
    private async UniTask HideArrow()
    {
        await transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.OutBack);
    }
}