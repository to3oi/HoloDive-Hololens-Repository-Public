/*===============================================================================
Copyright (c) 2021 PTC Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other
countries.
===============================================================================*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

/// <summary>
/// A custom handler which uses the VuMarkManager.
/// </summary>
public class VuMarkHandler : MonoBehaviour
{
    [Serializable]
    public class AugmentationObject
    {
        public string VuMarkID;
        public GameObject Augmentation;
    }
    
    // Define the number of persistent child objects of the VuMarkBehaviour. When
    // destroying the instance-specific augmentations, it will start after this value.
    // Persistent Children:
    // 1. Canvas -- displays info about the VuMark
    // 2. LineRenderer -- displays border outline around VuMark
    const int PERSISTENT_NUMBER_OF_CHILDREN = 2;
    
    Dictionary<string, Texture2D> mVuMarkInstanceTextures;
    Dictionary<string, GameObject> mVuMarkAugmentationObjects;
    VuMarkObserverStatusUI mNearestVuMarkScreenPanel;
    readonly Dictionary<VuMarkBehaviour, bool> mVuMarkBehaviours = new Dictionary<VuMarkBehaviour, bool>();
    VuMarkBehaviour mCurrentVuMark;
    Transform mVuforiaCamera;

    public AugmentationObject[] AugmentationObjects;

    void Start()
    {
        mVuMarkInstanceTextures = new Dictionary<string, Texture2D>();
        mVuMarkAugmentationObjects = new Dictionary<string, GameObject>();

        foreach (var augmentationObject in AugmentationObjects)
            mVuMarkAugmentationObjects.Add(augmentationObject.VuMarkID, augmentationObject.Augmentation);

        // Hide the initial VuMark Template when the scene starts.
        foreach (var vuMarkBehaviour in FindObjectsOfType<VuMarkBehaviour>())
            ToggleRenderers(vuMarkBehaviour.gameObject, false);

        mNearestVuMarkScreenPanel = FindObjectOfType<VuMarkObserverStatusUI>();

        VuforiaApplication.Instance.OnVuforiaStarted += VuforiaStarted;
    }

    void OnDestroy()
    {
        VuforiaApplication.Instance.OnVuforiaStarted -= VuforiaStarted;

        if (VuforiaBehaviour.Instance != null)
        {
            VuforiaBehaviour.Instance.SetMaximumSimultaneousTrackedImages(4); // Reset back to 4 when exiting
            VuforiaBehaviour.Instance.World.OnObserverCreated -= OnObserverCreated;
        }
    }

    void Update()
    {
        UpdateClosestTarget();
    }

    void VuforiaStarted()
    {
        VuforiaBehaviour.Instance.SetMaximumSimultaneousTrackedImages(10);
        mVuforiaCamera = VuforiaBehaviour.Instance.GetComponent<Camera>().transform;
        VuforiaBehaviour.Instance.World.OnObserverCreated += OnObserverCreated;
    }

    void OnObserverCreated(ObserverBehaviour behaviour)
    {
        if (!(behaviour is VuMarkBehaviour vuMarkBehaviour))
            return;

        if (mVuMarkBehaviours.ContainsKey(vuMarkBehaviour))
            return;
        
        mVuMarkBehaviours.Add(vuMarkBehaviour, false);

        vuMarkBehaviour.GetComponent<VuMarkObserverEventHandler>().OnVuMarkFound += OnVuMarkFound;
        vuMarkBehaviour.GetComponent<VuMarkObserverEventHandler>().OnVuMarkLost += OnVuMarkLost;
        vuMarkBehaviour.OnBehaviourDestroyed += OnVuMarkDestroyed;
    }

    void OnVuMarkDestroyed(ObserverBehaviour behaviour)
    {
        behaviour.GetComponent<VuMarkObserverEventHandler>().OnVuMarkFound -= OnVuMarkFound;
        behaviour.GetComponent<VuMarkObserverEventHandler>().OnVuMarkLost -= OnVuMarkLost;
        behaviour.OnBehaviourDestroyed -= OnVuMarkDestroyed;
        mVuMarkBehaviours.Remove((VuMarkBehaviour) behaviour);
                    
        if (behaviour == mCurrentVuMark && mNearestVuMarkScreenPanel != null)
        {
            mCurrentVuMark = null;
            mNearestVuMarkScreenPanel.ResetUI();
        }
    }

    /// <summary>
    ///  Register a callback which is invoked whenever a VuMark-result is newly detected which was not tracked
    ///  in the previous frame
    /// </summary>
    void OnVuMarkFound(VuMarkBehaviour vuMarkBehaviour)
    {
        mVuMarkBehaviours[vuMarkBehaviour] = true;

        if (RetrieveStoredTextureForVuMarkTarget(vuMarkBehaviour) == null)
            mVuMarkInstanceTextures.Add(GetVuMarkId(vuMarkBehaviour), GenerateTextureFromVuMarkInstanceImage(vuMarkBehaviour));
            
        Debug.Log("<color=cyan>VuMarkHandler.OnVuMarkFound(): </color>" + vuMarkBehaviour.TargetName);
        GenerateVuMarkBorderOutline(vuMarkBehaviour);
        ToggleRenderers(vuMarkBehaviour.gameObject, true);
        
        // Check for existence of previous augmentations and delete before instantiating new ones.
        DestroyChildAugmentationsOfTransform(vuMarkBehaviour.transform);
        
        SetVuMarkInfoForCanvas(vuMarkBehaviour);
        SetVuMarkAugmentation(vuMarkBehaviour);
        SetVuMarkOpticalSeeThroughConfig(vuMarkBehaviour);

        if (vuMarkBehaviour == mCurrentVuMark && mNearestVuMarkScreenPanel != null)
        {
            var vuMarkId = GetVuMarkId(mCurrentVuMark);
            var vuMarkDataType = GetVuMarkDataType(mCurrentVuMark);
            var vuMarkImage = GetVuMarkImage(mCurrentVuMark);
            var vuMarkDesc = GetNumericVuMarkDescription(mCurrentVuMark);
            mNearestVuMarkScreenPanel.Show(vuMarkId, vuMarkDataType, vuMarkDesc, vuMarkImage);
        }
    }

    void OnVuMarkLost(VuMarkBehaviour vuMarkBehaviour)
    {
        if (!mVuMarkBehaviours.TryGetValue(vuMarkBehaviour, out var tracked) || !tracked)
            return;

        mVuMarkBehaviours[vuMarkBehaviour] = false;
        
        Debug.Log("<color=cyan>VuMarkHandler.OnVuMarkLost(): </color>" + GetVuMarkId(vuMarkBehaviour));

        ToggleRenderers(vuMarkBehaviour.gameObject, false);
        DestroyChildAugmentationsOfTransform(vuMarkBehaviour.transform);
        
        if (vuMarkBehaviour == mCurrentVuMark && mNearestVuMarkScreenPanel != null)
            mNearestVuMarkScreenPanel.ResetUI();
    }

    string GetVuMarkDataType(VuMarkBehaviour vuMarkTarget)
    {
        switch (vuMarkTarget.InstanceId.DataType)
        {
            case InstanceIdType.BYTE:
                return "Bytes";
            case InstanceIdType.STRING:
                return "String";
            case InstanceIdType.NUMERIC:
                return "Numeric";
        }
        return string.Empty;
    }

    string GetVuMarkId(VuMarkBehaviour vuMarkTarget)
    {
        switch (vuMarkTarget.InstanceId.DataType)
        {
            case InstanceIdType.BYTE:
                return vuMarkTarget.InstanceId.HexStringValue;
            case InstanceIdType.STRING:
                return vuMarkTarget.InstanceId.StringValue;
            case InstanceIdType.NUMERIC:
                return vuMarkTarget.InstanceId.NumericValue.ToString();
        }
        return string.Empty;
    }

    Sprite GetVuMarkImage(VuMarkBehaviour vuMarkTarget)
    {
        var instanceImage = vuMarkTarget.InstanceImage;
        if (instanceImage == null)
        {
            Debug.Log("VuMark Instance Image is null.");
            return null;
        }

        // First we create a texture
        var texture = new Texture2D(instanceImage.Width, instanceImage.Height, TextureFormat.RGBA32, false)
        {
            wrapMode = TextureWrapMode.Clamp
        };
        instanceImage.CopyToTexture(texture);

        // Then we turn the texture into a Sprite
        if (texture.width == 0 || texture.height == 0)
            return null;
        var rect = new Rect(0, 0, texture.width, texture.height); 
        return Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
    }

    string GetNumericVuMarkDescription(VuMarkBehaviour vuMarkTarget)
    {
        if (int.TryParse(GetVuMarkId(vuMarkTarget), NumberStyles.Integer, CultureInfo.InvariantCulture, out var vuMarkIdNumeric))
        {
            // Change the description based on the VuMark ID
            switch (vuMarkIdNumeric % 4)
            {
                case 1:
                    return "Astronaut";
                case 2:
                    return "Drone";
                case 3:
                    return "Fissure";
                case 0:
                    return "Oxygen Tank";
                default:
                    return "Unknown";
            }
        }

        return string.Empty; // if VuMark DataType is byte or string
    }

    void SetVuMarkInfoForCanvas(VuMarkBehaviour vuMarkBehaviour)
    {
        var canvasText = vuMarkBehaviour.gameObject.GetComponentInChildren<Text>();
        var canvasImage = vuMarkBehaviour.gameObject.GetComponentsInChildren<UnityEngine.UI.Image>()[2];
        var vuMarkInstanceTexture = RetrieveStoredTextureForVuMarkTarget(vuMarkBehaviour);
        var rect = new Rect(0, 0, vuMarkInstanceTexture.width, vuMarkInstanceTexture.height);
        var vuMarkId = GetVuMarkId(vuMarkBehaviour);
        var vuMarkDesc = GetVuMarkDataType(vuMarkBehaviour);
        var vuMarkDataType = GetNumericVuMarkDescription(vuMarkBehaviour);

        canvasText.text =
            "<color=yellow>VuMark Instance Id: </color>" +
            "\n" + vuMarkId + " - " + vuMarkDesc +
            "\n\n<color=yellow>VuMark Type: </color>" +
            "\n" + vuMarkDataType;

        if (vuMarkInstanceTexture.width == 0 || vuMarkInstanceTexture.height == 0)
            canvasImage.sprite = null;
        else
            canvasImage.sprite = Sprite.Create(vuMarkInstanceTexture, rect, new Vector2(0.5f, 0.5f));
    }

    void SetVuMarkAugmentation(VuMarkBehaviour vuMarkBehaviour)
    {
        var sourceAugmentation = GetValueFromDictionary(mVuMarkAugmentationObjects, GetVuMarkId(vuMarkBehaviour));

        if (sourceAugmentation == null) 
            return;
        
        Instantiate(sourceAugmentation, vuMarkBehaviour.transform);
    }

    void SetVuMarkOpticalSeeThroughConfig(VuMarkBehaviour vuMarkBehaviour)
    {
        var meshRenderer = vuMarkBehaviour.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
            meshRenderer.enabled = false;
    }

    Texture2D RetrieveStoredTextureForVuMarkTarget(VuMarkBehaviour vuMarkTarget)
    {
        return GetValueFromDictionary(mVuMarkInstanceTextures, GetVuMarkId(vuMarkTarget));
    }

    Texture2D GenerateTextureFromVuMarkInstanceImage(VuMarkBehaviour vuMarkTarget)
    {
        Debug.Log("<color=cyan>SaveImageAsTexture() called.</color>");

        if (vuMarkTarget.InstanceImage == null)
        {
            Debug.Log("VuMark Instance Image is null.");
            return null;
        }

        var texture = new Texture2D(vuMarkTarget.InstanceImage.Width, vuMarkTarget.InstanceImage.Height, TextureFormat.RGBA32, false)
        {
            wrapMode = TextureWrapMode.Clamp
        };
        vuMarkTarget.InstanceImage.CopyToTexture(texture);
        return texture;
    }

    void GenerateVuMarkBorderOutline(VuMarkBehaviour vuMarkBehaviour)
    {
        var lineRendererAugmentation = vuMarkBehaviour.GetComponentInChildren<LineRenderer>();
        if (lineRendererAugmentation == null)
        {
           var vuMarkBorder = new GameObject("VuMarkBorder");
        vuMarkBorder.transform.SetParent(vuMarkBehaviour.transform);
        vuMarkBorder.transform.localPosition = Vector3.zero;
        vuMarkBorder.transform.localEulerAngles = Vector3.zero;
        vuMarkBorder.transform.localScale = new Vector3(1 / vuMarkBehaviour.transform.localScale.x,
                                                        1, 1 / vuMarkBehaviour.transform.localScale.z);
        lineRendererAugmentation = vuMarkBorder.AddComponent<LineRenderer>();
        lineRendererAugmentation.enabled = false;
        lineRendererAugmentation.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lineRendererAugmentation.receiveShadows = false;
        // This shader needs to be added in the Project's Graphics Settings,
        // unless it is already in use by a Material present in the project.
        lineRendererAugmentation.material.shader = Shader.Find("Unlit/Color");
        lineRendererAugmentation.material.color = Color.clear;
        lineRendererAugmentation.positionCount = 4;
        lineRendererAugmentation.loop = true;
        lineRendererAugmentation.useWorldSpace = false;
        var vuMarkSize = vuMarkBehaviour.GetSize();
        var curve = new AnimationCurve();
        curve.AddKey(0.0f, 1.0f);
        curve.AddKey(1.0f, 1.0f);
        lineRendererAugmentation.widthCurve = curve;
        lineRendererAugmentation.widthMultiplier = 0.003f;
        var vuMarkExtentsX = vuMarkSize.x * 0.5f + lineRendererAugmentation.widthMultiplier * 0.5f;
        var vuMarkExtentsZ = vuMarkSize.y * 0.5f + lineRendererAugmentation.widthMultiplier * 0.5f;
        lineRendererAugmentation.SetPositions(new []
                                   {
                                       new Vector3(-vuMarkExtentsX, 0.001f, vuMarkExtentsZ),
                                       new Vector3(vuMarkExtentsX, 0.001f, vuMarkExtentsZ),
                                       new Vector3(vuMarkExtentsX, 0.001f, -vuMarkExtentsZ),
                                       new Vector3(-vuMarkExtentsX, 0.001f, -vuMarkExtentsZ)
                                   });
        }
        
        var lineRendererComponent = vuMarkBehaviour.GetComponent<VuMarkObserverEventHandler>();
        if(lineRendererComponent != null) 
            lineRendererComponent.AssignLineRenderer(lineRendererAugmentation);
    }

    void DestroyChildAugmentationsOfTransform(Transform parent)
    {
        if (parent.childCount <= PERSISTENT_NUMBER_OF_CHILDREN) 
            return;
        
        for (var x = PERSISTENT_NUMBER_OF_CHILDREN; x < parent.childCount; x++)
            Destroy(parent.GetChild(x).gameObject);
    }

    T GetValueFromDictionary<T>(Dictionary<string, T> dictionary, string key)
    {
        if (dictionary.ContainsKey(key))
        {
            dictionary.TryGetValue(key, out var value);
            return value;
        }
        return default;
    }

    void ToggleRenderers(GameObject obj, bool enable)
    {
        var rendererComponents = obj.GetComponentsInChildren<Renderer>(true);
        var canvasComponents = obj.GetComponentsInChildren<Canvas>(true);

        foreach (var component in rendererComponents)
        {
            // Skip the LineRenderer
            if (!(component is LineRenderer))
                component.enabled = enable;
        }

        foreach (var component in canvasComponents)
            component.enabled = enable;
    }

    void UpdateClosestTarget()
    {
        if (!VuforiaApplication.Instance.IsRunning || mVuforiaCamera == null)
            return;
        
        var closestDistance = Mathf.Infinity;
        var closestVuMark = mCurrentVuMark;
        foreach (var vuMarkBehaviour in mVuMarkBehaviours.Where(vmb => vmb.Value).Select(vmb => vmb.Key))
        {
            var worldPosition = vuMarkBehaviour.transform.position;
            var camPosition = mVuforiaCamera.InverseTransformPoint(worldPosition);
            var distance = Vector3.Distance(Vector2.zero, camPosition);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestVuMark = vuMarkBehaviour;
            }
        }

        if (mCurrentVuMark != closestVuMark)
        {
            var vuMarkId = GetVuMarkId(closestVuMark);
            var vuMarkDataType = GetVuMarkDataType(closestVuMark);
            var vuMarkImage = GetVuMarkImage(closestVuMark);
            var vuMarkDesc = GetNumericVuMarkDescription(closestVuMark);
            mCurrentVuMark = closestVuMark;
            StartCoroutine(ShowPanelAfter(0.5f, vuMarkId, vuMarkDataType, vuMarkDesc, vuMarkImage));
        }
    }

    IEnumerator ShowPanelAfter(float seconds, string vuMarkId, string vuMarkDataType, string vuMarkDesc, Sprite vuMarkImage)
    {
        yield return new WaitForSeconds(seconds);

        if (mNearestVuMarkScreenPanel != null)
            mNearestVuMarkScreenPanel.Show(vuMarkId, vuMarkDataType, vuMarkDesc, vuMarkImage);
    }
}
