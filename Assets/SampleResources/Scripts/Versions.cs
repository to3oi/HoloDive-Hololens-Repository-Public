/*===============================================================================
Copyright (c) 2021 PTC Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other
countries.
===============================================================================*/

using UnityEngine;
using Vuforia;

public class Versions : MonoBehaviour
{
    void Start()
    {
        var versions = $"Vuforia Version: {VuforiaApplication.GetVuforiaLibraryVersion()}\n" +
                       $"Unity Version: {Application.unityVersion}";

        SampleUtil.AssignStringToTextComponent(gameObject, versions);
    }
}
