using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using TMPro;

/// <summary>
/// 外部からのメソッド呼び出しでUIの表示のみを行う
/// (テキストのアニメーションまでは、やってもいいかも) 
/// Singletone化する
/// </summary>
public class UIManager : SingletonMonoBehaviour<UIManager>
{
    [SerializeField] private TextMeshProUGUI UIArea = null;

    //private List<TextAnimationInformation> taInfoList = new List<TextAnimationInformation>();
    private ReactiveCollection<TextAnimationInformation>
        taInfoList = new ReactiveCollection<TextAnimationInformation>();

    [SerializeField] private float textSpeed = 0.1f;
    private bool isAnimation = false;

    [SerializeField] private bool isText = false;

    private void Awake()
    {
        taInfoList
            .ObserveAdd()
            .Where(_ => (taInfoList.Count != 0 && isAnimation == false))
            .Subscribe(value =>
            {
                isAnimation = true;
                TextAnimation().Forget();
            }).AddTo(this);
    }


    /// <summary>
    /// 受け取った Text を保持
    /// </summary>
    /// <param name="text"> 表示するテキスト </param>
    /// <param name="time"> 表示する秒数 </param>
    /// <param name="type"> アニメーションの種類 </param>
    public void StockText(string text, float time, TextAnimationType type = TextAnimationType.None)
    {
        TextAnimationInformation taInfo = new TextAnimationInformation
        {
            text = text,
            time = time,
            type = type
        };

        Debug.Log(taInfo.text);
        taInfoList.Add(taInfo);
    }

    // 1.テキストを表示
    // 2.流れるように表示
    // 3.アニメーション

    /// <summary>
    /// Text の表示
    /// </summary>
    async UniTask TextAnimation()
    {
        // 要変更
        while (taInfoList.Count != 0)
        {
            var v = taInfoList[0];
            UIArea.text = "";
            int i = 0;
            for (float f = 0.0f; f < v.time; f += Time.deltaTime)
            {
                if (i == Mathf.FloorToInt(f / textSpeed))
                {
                    Debug.Log(v.text);
                    if (v.text.Length < i)
                    {
                        await UniTask.Yield();
                        continue;
                    }

                    UIArea.text = v.text.Substring(0, i);
                    i++;
                }

                await UniTask.Yield();
            }

            taInfoList.Remove(v);
        }

        isAnimation = false;
    }
}

/// <summary>
/// StockText の引数を保持するクラス
/// </summary>
public class TextAnimationInformation
{
    public string text { get; set; }
    public float time { get; set; }
    public TextAnimationType type { get; set; }
}