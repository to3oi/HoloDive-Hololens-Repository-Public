/*===============================================================================
Copyright (c) 2021 PTC Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other
countries.
===============================================================================*/

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vuforia;

public class VuforiaStateInfo : MonoBehaviour
{
    public GameObject TextObject;

    const string ACTIVE_TARGETS_TITLE = "<b>Active Targets: </b>";

    readonly Dictionary<string, string> mTargetsStatus = new Dictionary<string, string>();

    public enum TargetType
    {
        IMAGE_TARGET,
        MODEL_TARGET
    }

    public TargetType desiredTargetType;

    void Start()
    {
        VuforiaApplication.Instance.OnVuforiaStarted += OnVuforiaStarted;
    }

    void OnDestroy()
    {
        VuforiaApplication.Instance.OnVuforiaStarted -= OnVuforiaStarted;
    }

    void OnVuforiaStarted()
    {
        var observerBehaviours = VuforiaBehaviour.Instance.World.GetObserverBehaviours();

        foreach (var observerBehaviour in observerBehaviours.Where(MatchesDesiredType))
        {
            mTargetsStatus.Add(observerBehaviour.TargetName, GetStatusString(observerBehaviour.TargetStatus));
            observerBehaviour.OnTargetStatusChanged += OnTargetStatusChanged;
            observerBehaviour.OnBehaviourDestroyed += OnBehaviourDestroyed;
        }

        VuforiaBehaviour.Instance.World.OnObserverCreated += OnObserverCreated;
        UpdateText();
    }

    bool MatchesDesiredType(ObserverBehaviour observerBehaviour)
    {
        return
            desiredTargetType == TargetType.IMAGE_TARGET && observerBehaviour is ImageTargetBehaviour
            ||
            desiredTargetType == TargetType.MODEL_TARGET && observerBehaviour is ModelTargetBehaviour;
    }

    void OnObserverCreated(ObserverBehaviour observerBehaviour)
    {
        if (!MatchesDesiredType(observerBehaviour))
            return;
        
        if (mTargetsStatus.ContainsKey(observerBehaviour.TargetName))
            return;

        mTargetsStatus.Add(observerBehaviour.TargetName, GetStatusString(observerBehaviour.TargetStatus));
        observerBehaviour.OnTargetStatusChanged += OnTargetStatusChanged;
        observerBehaviour.OnBehaviourDestroyed += OnBehaviourDestroyed;
    }

    void OnBehaviourDestroyed(ObserverBehaviour observerBehaviour)
    {
        observerBehaviour.OnTargetStatusChanged -= OnTargetStatusChanged;
        observerBehaviour.OnBehaviourDestroyed -= OnBehaviourDestroyed;
        mTargetsStatus.Remove(observerBehaviour.TargetName);
    }

    private void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus targetStatus)
    {
        mTargetsStatus[behaviour.TargetName] = GetStatusString(targetStatus);
        UpdateText();
    }

    void UpdateText()
    {
        var targetStatusInfo = GetTargetsStatusInfo();

        var completeInfo = ACTIVE_TARGETS_TITLE;

        if (targetStatusInfo.Length > 0)
            completeInfo += $"\n{targetStatusInfo}";

        SampleUtil.AssignStringToTextComponent(TextObject ? TextObject : gameObject, completeInfo);
    }

    string GetStatusString(TargetStatus targetStatus)
    {
        return $"{targetStatus.Status} -- {targetStatus.StatusInfo}";
    }
    
    string GetTargetsStatusInfo()
    {
        var targetsAsMultiLineString = "";
        
        foreach (var targetStatus in mTargetsStatus)
            targetsAsMultiLineString += "\n" + targetStatus.Key + ": " + targetStatus.Value;

        return targetsAsMultiLineString;
    }
}
