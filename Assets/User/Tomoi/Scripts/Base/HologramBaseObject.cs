using UnityEngine;
using UniRx;

[RequireComponent(typeof(DefaultObserverEventHandler))]
public class HologramBaseObject : MonoBehaviour
{
    /// <summary>
    /// 画像を認識しているかどうか
    /// </summary>
    public bool isViewing { get; private set; }

    /// <summary>
    /// vuforiaのEventHandler
    /// </summary>
    private DefaultObserverEventHandler handler = null;

    /// <summary>
    /// 画像認識で表示するオブジェクトのオフセットを指定するオブジェクト
    /// </summary>
    [SerializeField, Header("画像認識で表示するオブジェクトのオフセットを指定するオブジェクト")]
    private GameObject positionOffsetObject = null;

    /// <summary>
    /// 時間軸が過去のときに使用するオブジェクト
    /// </summary>
    [SerializeField, Header("時間軸が過去のときに使用するオブジェクト")]
    private GameObject pastObject = null;

    /// <summary>
    /// 時間軸が未来のときに使用するオブジェクト
    /// </summary>
    [SerializeField, Header("時間軸が未来のときに使用するオブジェクト")]
    private GameObject futureObject = null;

    /// <summary>
    /// trueのとき画像認識した際にAlwaysShowObjectに指定したオブジェクトを常に表示し続ける
    /// </summary>
    [SerializeField, Header("trueのとき画像認識した際にAlwaysShowObjectに指定したオブジェクトを常に表示し続ける")]
    private bool isAlwaysShow = false;

    /// <summary>
    /// isAlwaysShowがtrueのときに表示するオブジェクトを指定する
    /// </summary>
    [SerializeField, Header("isAlwaysShowがtrueのときに表示するオブジェクトを指定する")]
    private GameObject AlwaysShowObject;
    [Header("---")]
    [Header("AlwaysShowObjectを生成する位置を指定")] [SerializeField, Header("Forward")]
    private float AlwaysShowObjectForwardOffset = 0.6f;
    [SerializeField, Header("Down")] private float AlwaysShowObjectDownOffset = 0.6f;
    
    [Space(10)]
    
    
    [SerializeField, Header("isAlwaysShowと自身がtrueのときAlwaysShowObjectの方向に向き続ける矢印を生成する")]
    private bool isShowArrow = false;

    /// <summary>
    /// 新しく生成する
    /// </summary>
    private Transform rootTransform;

    /// <summary>
    /// 画像認識用の画像を一度でも読み込んでいるかどうか
    /// </summary>
    private bool viewedImage = false;

    /// <summary>
    /// 最初の画像認識かどうか
    /// </summary>
    private bool isFarstLook = false;

    void Start()
    {
        //初期化
        ObjectSetActive(futureObject, false);
        ObjectSetActive(pastObject, false);
        if (isAlwaysShow)
        {
            AlwaysShowObject.SetActive(false);
        }

        //移動と回転のルートオブジェクトを作成
        rootTransform = new GameObject(gameObject.name + "_RootObject").GetComponent<Transform>();
        //オフセット以下のオブジェクトをすべて新しく生成したルートの子要素に入れる
        //親オブジェクトからの相対座標を保持し、新しく生成したルートに移動した際にもその相対座標を使用する
        var position = positionOffsetObject.transform.localPosition;
        var rotation = positionOffsetObject.transform.localRotation;
        positionOffsetObject.transform.parent = rootTransform;
        positionOffsetObject.transform.localPosition = position;
        positionOffsetObject.transform.localRotation = rotation;

        //イベントの登録
        handler = GetComponent<DefaultObserverEventHandler>();
        handler.OnTargetFound.AddListener(Viewing);
        handler.OnTargetLost.AddListener(Invisible);

        //時間軸の変更を購読
        TimeAxisManager.Instance.TimeAxisObserver.Subscribe(_ => { ShowObject(); }).AddTo(this);
    }


    private void Update()
    {
        //画像認識中のみ座標を更新
        if (isViewing)
        {
            rootTransform.position = transform.position;
            rootTransform.rotation = transform.rotation;
        }
    }

    /// <summary>
    /// オブジェクトを表示する
    /// </summary>
    public void Viewing()
    {
        viewedImage = true;
        isViewing = true;

        //初めて画像認識するときのみ実行
        if (isAlwaysShow && !isFarstLook)
        {
            isFarstLook = true;
            AlwaysShowObject.SetActive(true);
            AlwaysShowObject.transform.parent = null;

            #region AlwaysShowObjectの座標をプレイヤーの前の位置に更新する

            //暫定的にカメラの回転を制限したオブジェクトを生成し、前方向のベクトルを取得する
            //TODO:計算のみで実装
            var cameraTransform = Camera.main.transform;

            //上下の回転を制限
            //プレイヤーの方向を向くよう回転を修正
            var q = cameraTransform.rotation.eulerAngles;
            q.x = 0;
            q.z = 0;

            var tempGameObject = new GameObject("tempGameObject")
            {
                transform =
                {
                    rotation = Quaternion.Euler(q)
                }
            };

            var p = cameraTransform.position +
                    //前方向に座標を移動
                    tempGameObject.transform.forward * AlwaysShowObjectForwardOffset +
                    //カメラのY座標から一定値下にずらす
                    -Vector3.up * AlwaysShowObjectDownOffset;

            //座標を代入
            AlwaysShowObject.transform.position = p;

            #endregion

            //(isShowArrow == true)時に矢印を表示する
            if (isShowArrow)
            {
                ArrowManager.Instance.SetupArrow(AlwaysShowObject);
            }

            ShowObject();
        }
    }

    /// <summary>
    /// オブジェクトを非表示にする
    /// </summary>
    public void Invisible()
    {
        isViewing = false;
    }

    /// <summary>
    /// オブジェクトの表示を更新する
    /// </summary>
    private void ShowObject()
    {
        //画像を一度も読み込んでいない場合return
        if (!viewedImage)
        {
            return;
        }

        if (TimeAxisManager.Instance.Axis == TimeAxisManager.axis.future)
        {
            ObjectSetActive(futureObject, true);
            ObjectSetActive(pastObject, false);
        }
        else
        {
            ObjectSetActive(futureObject, false);
            ObjectSetActive(pastObject, true);
        }
    }

    /// <summary>
    /// オブジェクトの表示状態を引数 isActive にする
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="isActive"></param>
    private static void ObjectSetActive(GameObject gameObject, bool isActive)
    {
        if (gameObject)
        {
            gameObject.SetActive(isActive);
        }
    }
}