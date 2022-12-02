/*===============================================================================
Copyright (c) 2021 PTC Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other
countries.
===============================================================================*/

using UnityEngine;

public class ContentPositionerClient : MonoBehaviour
{
    [Header("Distance From Camera")]
    public float DistanceHoloLens1 = 1f; // adjust to preferred distance
    public float DistanceHoloLens2 = 1f; // adjust to preferred distance

    void Start()
    {
        var contentPositioner = FindObjectOfType<ContentPositioner>();

        if (contentPositioner)
        {
            contentPositioner.SetPerDeviceDistanceFromCamera(DistanceHoloLens1, DistanceHoloLens2);
            contentPositioner.ContentToAlign = gameObject;
            contentPositioner.CenterToCameraView();
        }
    }
}
