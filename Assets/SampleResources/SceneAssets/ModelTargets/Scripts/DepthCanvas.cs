/*==============================================================================
Copyright (c) 2021 PTC Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other
countries.
==============================================================================*/

using UnityEngine;
using Vuforia;

[RequireComponent(typeof(RectTransform))]
public class DepthCanvas : MonoBehaviour
{
    Camera mVuforiaCamera;
    RectTransform mCanvasRectTransform;
    const float MAX_DISTANCE_FROM_CAMERA = 1.25f;
    const float BACKWARDS_OFFSET = 0.02f;

    void Start()
    {
        mVuforiaCamera = VuforiaBehaviour.Instance.GetComponent<Camera>();
        mCanvasRectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (!VuforiaApplication.Instance.IsRunning)
            return;

        var ray = mVuforiaCamera.ViewportPointToRay(0.5f * Vector2.one);

        if (Physics.Raycast(ray, out var hit, mVuforiaCamera.farClipPlane))
        {
            var point = hit.point - BACKWARDS_OFFSET * ray.direction.normalized;
            var depth = Vector3.Distance(ray.origin, point);

            if (mCanvasRectTransform != null)
            {
                depth = Mathf.Clamp(depth, mVuforiaCamera.nearClipPlane, MAX_DISTANCE_FROM_CAMERA);
                mCanvasRectTransform.anchoredPosition3D = new Vector3(0, 0, depth);
            }
        }
    }
}
