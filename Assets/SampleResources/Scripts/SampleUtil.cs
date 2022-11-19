/*===============================================================================
Copyright (c) 2021 PTC Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other
countries.
===============================================================================*/

using UnityEngine;
using Vuforia;

public class SampleUtil : MonoBehaviour
{
    public static bool AssignStringToTextComponent(GameObject textObj, string text)
    {
        if (textObj)
        {
            var canvasText = textObj.GetComponent<UnityEngine.UI.Text>();
            var textMesh = textObj.GetComponent<TextMesh>();

            if (canvasText)
            {
                canvasText.text = text;
                return true;
            }
            if (textMesh)
            {
                textMesh.text = text;
                return true;
            }
            return false;
        }

        Debug.LogWarning("Destination Text GameObject is Null.");
        return false;
    }

    // Used by the VoiceCommands script
    public static void ResetObjectTracker()
    {
        VuforiaBehaviour.Instance.DevicePoseBehaviour.Reset();
    }
}
