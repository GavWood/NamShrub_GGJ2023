using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BWHand : MonoBehaviour
{
    float lastTriggerTime = 0;
    float HidePoseMarkerTime = 0;

    public enum BWControllerType
    {
        LeftHand,
        RightHand,
    };

    // For copying the correct pose
    [SerializeField]
    BWControllerType handId;

    GameObject poseMarker;

    void Start()
    {
        if (BWVR.IsVR() == false)
        {
            gameObject.SetActive(false);
            return;
        }

        poseMarker = transform.Find("PoseMarker").gameObject;
        poseMarker.SetActive(false);
    }

    void ShowAndHideTrigger()
    {
        // Show the trigger
        if (BWVR.IsTrigger(handId))
        {
            poseMarker.SetActive(true);
            lastTriggerTime = Time.time;
        }

        float currentTime = Time.time;

        if ((currentTime - lastTriggerTime) > HidePoseMarkerTime)
        {
            poseMarker.SetActive(false);
        }
    }

    private void Update()
    {
        ShowAndHideTrigger(); // for menu
    }
}
