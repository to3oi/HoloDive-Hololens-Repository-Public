using UnityEngine;
using TMPro;

/// <summary>
/// マスターPCで使用するキー入力デバイスの全体管理をするマネージャー
/// </summary>
public class MasterPCKeyPadManager : MonoBehaviour
{
    /// <summary>
    /// 正解のパスワード
    /// </summary>
    [SerializeField] private string password = "0603";

    /// <summary>
    /// 入力されたテキストを表示するエリア
    /// </summary>
    [SerializeField] private TextMeshProUGUI outPutArea;

    /// <summary>
    /// 入力されたテキスト
    /// </summary>
    private string inputString = "";

    /// <summary>
    /// 入力したパスワードが正解済みの場合true
    /// </summary>
    private bool isSuccess = false;

    [SerializeField] private MasterPCSuccessViewChange _masterPC;

    private void Start()
    {
        outPutArea.text = "";
    }

    /// <summary>
    /// 外部のUnityEventから任意の数字を入力する関数
    /// </summary>
    /// <param name="_keyPad">整数の数字1文字</param>
    public void PushKeyPad(int _keyPad)
    {
        if (isSuccess)
        {
            return;
        }

        //入力された数字をKeyPadEnumに沿って処理
        switch ((KeyPadEnum)_keyPad)
        {
            //入力をクリア
            case KeyPadEnum.X:
            {
                inputString = "";
            }
                break;
            //入力を確定
            case KeyPadEnum.V:
            {
                //入力したものが正解のパスワードとあっているときの処理
                if (inputString == password)
                {
                    outPutArea.text = "success";
                    _masterPC.UpdateMonitor();
                    isSuccess = true;
                    return;
                }
                else
                {   //間違っていたら入力をクリア
                    inputString = "";
                }
            }
                break;
            //数字の処理
            case KeyPadEnum.Zero:
            case KeyPadEnum.One:
            case KeyPadEnum.Two:
            case KeyPadEnum.Three:
            case KeyPadEnum.Four:
            case KeyPadEnum.Five:
            case KeyPadEnum.Six:
            case KeyPadEnum.Seven:
            case KeyPadEnum.Eight:
            case KeyPadEnum.Nine:
            {
                if (3 < inputString.Length)
                {
                    return;
                }
                //数字を文字列に変換しpush
                var i = (int)_keyPad;
                inputString += i.ToString();
            }
                break;
        }

        UpdateText();
    }

    /// <summary>
    /// 入力された文字列を更新
    /// </summary>
    private void UpdateText()
    {
        outPutArea.text = inputString;
    }
}