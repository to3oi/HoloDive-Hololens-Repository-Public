/*===============================================================================
Copyright (c) 2021 PTC Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other
countries.
===============================================================================*/

using UnityEngine;

public class SetDrivenKey : MonoBehaviour
{
    public RectTransform WatchSizeChanges;
    public RectTransform ReadChangedSize;

    void Start()
    {
        ApplyRectTransformSizeToTransformScale(ref ReadChangedSize);
    }

    void Update()
    {
        if (WatchSizeChanges.hasChanged)
        {
            ApplyRectTransformSizeToTransformScale(ref ReadChangedSize);
            WatchSizeChanges.hasChanged = false;
        }
    }

    void ApplyRectTransformSizeToTransformScale(ref RectTransform rectTransform)
    {
        if (rectTransform != null)
            transform.localScale = new Vector3(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y);
    }
}
