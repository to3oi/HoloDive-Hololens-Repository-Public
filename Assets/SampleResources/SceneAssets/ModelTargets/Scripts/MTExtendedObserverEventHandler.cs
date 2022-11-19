/*==============================================================================
Copyright (c) 2021 PTC Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other
countries.
==============================================================================*/

using UnityEngine;
using Vuforia;

/// <summary>
/// This class watches for TargetStatus of Model Target Behaviours and draws a
/// colored bounding box to indicate when Model Target is being Extended Tracked.
/// </summary>
public class MTExtendedObserverEventHandler : DefaultObserverEventHandler
{
    [SerializeField] MeshRenderer BoundingBox = null;

    protected override void OnTrackingFound()
    {
        base.OnTrackingFound();

        var modelTarget = mObserverBehaviour as ModelTargetBehaviour;
        if (modelTarget != null)
            DrawOrientedBoundingBox3D(modelTarget.GetBoundingBox());
    }

    void LateUpdate()
    {
        // Currently only one Model Target can be tracked at a given time.
        // As this event handler may be applied to multiple ModelTargetBehaviours in a scene,
        // we confirm that the bounding box is a child of this target  before enabling/disabling its renderer.
        if (transform.Equals(BoundingBox.transform.parent))
            BoundingBox.enabled = mObserverBehaviour.TargetStatus.Status == Status.EXTENDED_TRACKED;
    }


    void DrawOrientedBoundingBox3D(Bounds bbox3d)
    {
        if (BoundingBox == null) 
            return;
        
        // Calculate local position and scale from Model Target bounding box.
        var bboxLocalPosition = new Vector3(bbox3d.center.x, bbox3d.center.y, bbox3d.center.z);
        var bboxLocalScale = new Vector3(bbox3d.extents.x * 2, bbox3d.extents.y * 2, bbox3d.extents.z * 2);

        // Assign values to augmentation bounding box.
        BoundingBox.transform.SetParent(mObserverBehaviour.transform);
        BoundingBox.transform.localEulerAngles = Vector3.zero;
        BoundingBox.transform.position = Vector3.zero;
        BoundingBox.transform.localPosition = bboxLocalPosition;
        BoundingBox.transform.localScale = bboxLocalScale;
    }
}
