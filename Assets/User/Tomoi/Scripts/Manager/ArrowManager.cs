using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// 画像認識時に生成したcontentを視界に入れていないときにその方向を示す矢印を生成するマネージャー
/// </summary>
public class ArrowManager : SingletonMonoBehaviour<ArrowManager>
{
    /// <summary>
    /// 矢印のPrefab
    /// </summary>
    [SerializeField] private GameObject ArrowPrefab;

    /// <summary>
    /// 視界の角度
    /// </summary>
    private float _visibilityAngle = 15f;

    /// <summary>
    /// 視界の最大距離
    /// </summary>
    private float _maxDistance = float.PositiveInfinity;

    /// <summary>
    /// 矢印を生成し、引数のcontentを視界に入れるまで表示し続ける
    /// </summary>
    /// <param name="contentGameObject">矢印を向けるコンテンツ自身のオブジェクト</param>
    public void SetupArrow(GameObject contentGameObject)
    {
        //初めからオブジェクトが視界内にあるなら矢印を表示しない
        //視界内にcontentが存在しないならcontentGameObjectの方向を指す矢印を生成する
        if (!IsVisibleContent(contentGameObject))
        {
            //Prefabの生成
            var playerTransform = Camera.main.transform;
            GameObject g = Instantiate(ArrowPrefab, playerTransform.position + playerTransform.forward * 0.5f,
                quaternion.identity, playerTransform);
            //アニメーションさせるためにスケールを0で初期化
            g.transform.localScale = Vector3.zero;

            //Componentの取得
            SpinArrowDirectionContent spinArrowDirectionContent = g.GetComponent<SpinArrowDirectionContent>();

            //contentを渡す
            spinArrowDirectionContent.StartArrowProcess(contentGameObject);
        }
    }

    /// <summary>
    /// 視界内にcontentGameObjectが存在するかどうか
    /// </summary>
    /// <param name="contentGameObject">矢印を向けるコンテンツ自身のオブジェクト</param>
    /// <returns></returns>
    public bool IsVisibleContent(GameObject contentGameObject)
    {
        //一時的に変数に保持
        var playerTarget = Camera.main.transform;
        var contentTransform = contentGameObject.transform;

        // プレイヤーの位置
        var playerPos = playerTarget.position;
        // contentの位置
        var contentPos = contentTransform.position;

        // 自身の向き（正規化されたベクトル）
        var playerDirection = playerTarget.forward;

        // contentまでの向きと距離計算
        var contentDirection = contentPos - playerPos;
        var contentDistance = contentDirection.magnitude;

        var cosHalf = Mathf.Cos(_visibilityAngle / 2 * Mathf.Deg2Rad);

        // 内積を計算
        var innerProduct = Vector3.Dot(playerDirection, contentDirection.normalized);

        // 視界判定
        return innerProduct > cosHalf && contentDistance < _maxDistance;
    }
}