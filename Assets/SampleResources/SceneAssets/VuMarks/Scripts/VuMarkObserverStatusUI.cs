/*===============================================================================
Copyright (c) 2021 PTC Inc. All Rights Reserved.

Confidential and Proprietary - Protected under copyright and other laws.
Vuforia is a trademark of PTC Inc., registered in the United States and other
countries.
===============================================================================*/

using UnityEngine;
using UnityEngine.UI;

public class VuMarkObserverStatusUI : MonoBehaviour
{
    public Image Image;
    public Text Info;

    private string defaultInfoText = "<color=yellow>VuMark Instance Id:</color>\nNone\n\n" +
                                     "<color=yellow>VuMark Type:</color>\nNone";
    
    public void Show(string vuMarkId, string vuMarkDataType, string vuMarkDesc, Sprite vuMarkImage)
    {
        Info.text = "<color=yellow>VuMark Instance Id: </color>\n" +
                      $"{vuMarkId} - {vuMarkDesc}\n\n" +
                      "<color=yellow>VuMark Type: </color>\n" +
                      $"{vuMarkDataType}";

        Image.sprite = vuMarkImage;
        Image.enabled = true;
    }

    public void ResetUI()
    {
        Info.text = defaultInfoText;
        Image.enabled = false;
    }
}
