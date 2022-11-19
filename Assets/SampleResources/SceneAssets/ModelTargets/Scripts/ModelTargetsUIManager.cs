/*==============================================================================
Copyright (c) 2021 PTC Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other
countries.
==============================================================================*/

using System.Collections;
using UnityEngine;
using Vuforia;
using Image = UnityEngine.UI.Image;

public class ModelTargetsUIManager : MonoBehaviour
{
    [Header("Symbolic Icon Canvas Groups")]
    [SerializeField] CanvasGroup CanvasGroupAdvanced = null;

    [Tooltip("Cycle Multiple Targets")] 
    [SerializeField] bool CycleMultipleIcons = false;

    const float IMAGE_SWAP_FADE_RANGE_MAX = 0.001f;
    readonly Color mWhiteTransparent = new Color(1f, 1f, 1f, 0f);

    Image[] mImageSequence;
    Image[] mImagesAdvanced;
    bool mUIEnabled;
    bool mImageSequencePaused;
    int mImageSequenceIndex;
    float mClock;
    float mFadeMeter;

    // Start is called before the first frame update
    void Start()
    {
        AttachCanvasToCamera();
        InitSymbolicTargetIcons();
    }

    // Update is called once per frame
    void Update()
    {
        // Use the Symbolic Target Fade Cycle when running on Mobile, but not HoloLens
        if (CycleMultipleIcons)
            UpdateSymbolicTargetIconFadeCycle();
    }


    public void SetUI(ModelTargetsManager.ModelTargetMode modelTargetMode, bool enable)
    {
        switch (modelTargetMode)
        {
            case ModelTargetsManager.ModelTargetMode.MODE_STANDARD:
                CanvasGroupAdvanced.alpha = 0;
                break;
            case ModelTargetsManager.ModelTargetMode.MODE_ADVANCED:
                mImageSequence = mImagesAdvanced;
                CanvasGroupAdvanced.alpha = enable ? 1 : 0;
                break;
        }

        if (mUIEnabled == enable) 
            return;
        
        if (CycleMultipleIcons)
            // For Mobile, we'll use image sequence (Advanced) and fade cycling (Advanced, 360)
            ResetImageSequenceValues();

        mUIEnabled = enable;
    }

    void AttachCanvasToCamera()
    {
        // For HoloLens, attach our Symbolic Icons UI Canvas to the ARCamera
        var canvas = CanvasGroupAdvanced.GetComponentInParent<Canvas>().transform;
        canvas.SetParent(VuforiaBehaviour.Instance.transform);
        canvas.localPosition = new Vector3(0, 0, 1.25f);
        canvas.localRotation = Quaternion.identity;
    }

    void DetachCanvasFromCamera()
    {
        if (CanvasGroupAdvanced == null) 
            return;
        
        var canvas = CanvasGroupAdvanced.GetComponentInParent<Canvas>();
        if (canvas != null) 
            Destroy(canvas.gameObject);
    }

    void OnDestroy()
    {
        DetachCanvasFromCamera();
    }

    void InitSymbolicTargetIcons()
    {
        if (CanvasGroupAdvanced != null)
            mImagesAdvanced = CanvasGroupAdvanced.GetComponentsInChildren<Image>();

        if (CycleMultipleIcons)
        {
            // Set all the symbolic icons to be transparent at start.
            foreach (var image in mImagesAdvanced)
                image.color = mWhiteTransparent;
        }
    }

    void UpdateSymbolicTargetIconFadeCycle()
    {
        if (!mUIEnabled) 
            return;
        
        mFadeMeter = Mathf.InverseLerp(-1f, 1f, Mathf.Sin(mClock += Time.deltaTime * 2));
        mFadeMeter = Mathf.SmoothStep(0, 1, mFadeMeter);

        if (mImageSequence == null) 
            return;
        
        if (mImageSequence.Length > 1)
        {
            if (mFadeMeter < IMAGE_SWAP_FADE_RANGE_MAX && !mImageSequencePaused)
            {
                mImageSequence[mImageSequenceIndex].color = Color.clear;
                mImageSequenceIndex = (mImageSequenceIndex + 1) % mImageSequence.Length;
                mImageSequence[mImageSequenceIndex].color = Color.white;
                mImageSequencePaused = true;
                StartCoroutine(ClearImageSequencePause());
            }

            mImageSequence[mImageSequenceIndex].color = Color.Lerp(mWhiteTransparent, Color.white, mFadeMeter);
        }
        else
            mImageSequence[0].color = Color.Lerp(mWhiteTransparent, Color.white, mFadeMeter);
    }

    void ResetImageSequenceValues()
    {
        mClock = 0f;
        mImageSequenceIndex = 0;

        foreach (var image in mImageSequence)
            image.color = mWhiteTransparent;
    }

    IEnumerator ClearImageSequencePause()
    {
        // Wait until the fade meter exits the valid image swapping range before clearing sequence flag.
        // This enforces a maximum of one symbolic icon change per fade cycle.
        yield return new WaitUntil(() => mFadeMeter > IMAGE_SWAP_FADE_RANGE_MAX);
        mImageSequencePaused = false;
    }
}