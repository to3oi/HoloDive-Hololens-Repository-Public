using UnityEngine;
using TMPro;

public class MasterPCKeyPadManager : MonoBehaviour
{
    [SerializeField] private string password = "0603";

    [SerializeField] private TextMeshProUGUI outPutArea;
    private string inputString = "";

    private bool isSuccess = false;

    [SerializeField] private MainMonitorSuccessViewChange _mainMonitor;
    private void Start()
    {
        outPutArea.text = "";
    }

    public void PushKeyPad(int _keyPad)
    {
        if (isSuccess)
        {
            return;
        }

        switch ((KeyPadEnum)_keyPad)
        {
            case KeyPadEnum.X:
            {
                inputString = "";
            }
                break;
            case KeyPadEnum.V:
            {
                if (inputString == password)
                {
                    outPutArea.text = "success";
                    _mainMonitor.UpdateMonitor();
                    isSuccess = true;
                    return;
                }
                else
                {
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
            default:
            {
                if (3 < inputString.Length)
                {
                    return;
                }

                int i = (int)_keyPad;
                inputString += i.ToString();
            }
                break;
        }

        UpdateText();
    }

    private void UpdateText()
    {   
        outPutArea.text = inputString;
    }
}