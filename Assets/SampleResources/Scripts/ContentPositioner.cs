/*===============================================================================
Copyright (c) 2021 PTC Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other
countries.
===============================================================================*/

using UnityEngine;
using Vuforia;

public class ContentPositioner : MonoBehaviour
{
    public GameObject ContentToAlign;

    const float DEFAULT_DISTANCE_HOLO_LENS1 = 1.5f; // default distance
    const float DEFAULT_DISTANCE_HOLO_LENS2 = 0.75f; // default distance
    const float HEIGHT_OFFSET = 0.1f;
    const float ALIGNMENT_SPEED = 5f;
    float mDistanceFromCamera;
    bool mRealignContent;
    Vector3 mDestinationPosition;
    Quaternion mDestinationRotation;
    Transform mCamera;

    void Start()
    {
        mCamera = VuforiaBehaviour.Instance.transform;

        SetPerDeviceDistanceFromCamera(DEFAULT_DISTANCE_HOLO_LENS2, DEFAULT_DISTANCE_HOLO_LENS1);
    }

    void Update()
    {
        UpdateContentAlignment();
    }

    public void SetPerDeviceDistanceFromCamera(float distanceHoloLens1, float distanceHoloLens2)
    {
        // Device Model for HL1 = HoloLens (Microsoft Corporation)
        // Device Model for HL2 = HoloLens 2 (Microsoft Corporation)
        mDistanceFromCamera = SystemInfo.deviceModel.Contains("HoloLens 2") ? distanceHoloLens2 : distanceHoloLens1;
    }

    public void CenterToCameraView()
    {
        var camForwardFlatY = new Vector3(mCamera.forward.x, 0, mCamera.forward.z);
        mDestinationRotation = Quaternion.LookRotation(camForwardFlatY, Vector3.up);
        mDestinationPosition = mCamera.position + camForwardFlatY * mDistanceFromCamera;
        mDestinationPosition = new Vector3(mDestinationPosition.x, mCamera.position.y - HEIGHT_OFFSET, mDestinationPosition.z);

        mRealignContent = true;
    }

    void UpdateContentAlignment()
    {
        if (!mRealignContent || ContentToAlign == null) 
            return;
        
        var newRotation = Quaternion.Slerp(ContentToAlign.transform.rotation, mDestinationRotation, Time.deltaTime * ALIGNMENT_SPEED);
        var newPosition = Vector3.Lerp(ContentToAlign.transform.position, mDestinationPosition, Time.deltaTime * ALIGNMENT_SPEED);

        ContentToAlign.transform.rotation = newRotation;
        ContentToAlign.transform.position = newPosition;

        if (Mathf.Abs(Vector3.Distance(newPosition, mDestinationPosition)) < 0.01f)
        {
            Debug.Log("Content Alignment Complete");
            mRealignContent = false;
        }
    }
}
