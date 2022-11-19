/*===============================================================================
Copyright (c) 2021 PTC Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other
countries.
===============================================================================*/

#if MRTK_ENABLED
using Microsoft.MixedReality.Toolkit;
#endif
using UnityEngine;

public class VoiceCommands : MonoBehaviour
{
    public void Quit()
    {
        Application.Quit();
    }

    public void Reset()
    {
        SampleUtil.ResetObjectTracker();
    }

    public void ShowHandMesh()
    {
#if MRTK_ENABLED
        var handTrackingProfile = CoreServices.InputSystem?.InputSystemProfile?.HandTrackingProfile;

        if (handTrackingProfile != null)
            handTrackingProfile.EnableHandMeshVisualization = true;
#endif
    }

    public void HideHandMesh()
    {
#if MRTK_ENABLED
        var handTrackingProfile = CoreServices.InputSystem?.InputSystemProfile?.HandTrackingProfile;

        if (handTrackingProfile != null)
            handTrackingProfile.EnableHandMeshVisualization = false;
#endif
    }
}