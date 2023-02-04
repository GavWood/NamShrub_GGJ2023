using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BWHand : MonoBehaviour
{
    float lastTriggerTime = 0;
    float lastShortHapticTime = 0;
    float HidePoseMarkerTime = 5.0f;
    float HapticTime = 0.01f;
    float LowAmplitude = 0.1f;
    static float MinDistance = 0.1f;
    public static float distanceToNearestTarget = 0;
    public static int nearestIndex = 0;

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

    void SendShortHaptics()
    {
        float currentTime = Time.time;

        // Send haptic every HapticTime, then sleep HapticTime, then send again
        if ((currentTime - lastShortHapticTime) > HapticTime * 2)
        {
            lastShortHapticTime = Time.time;

            // dist 0   = amp 0.5
            // dist 0.5 = amp 0 
            float amp = Mathf.Lerp(LowAmplitude, 0, distanceToNearestTarget);

            // Get very close and your dead
            // Or the buzz turns off
            if (distanceToNearestTarget < MinDistance)
            {
                amp = 0;
            }
            BWVR.SendHaptic(handId, amp, HapticTime);
        }
    }

    private void BuildSuspenseWithHaptics()
    {
        // The hands position
        Vector3 position = transform.position;

        distanceToNearestTarget = 1000.0f;

        for (int i = 0; i < BWGameState.capsules.Length; i++)
        {
            CapsuleCollider c = BWGameState.capsules[i].GetComponent<CapsuleCollider>();

            Vector3 bottomSpot = c.transform.Find("Target").position;

            float dist = Vector3.Distance(bottomSpot, position);

            if( dist < distanceToNearestTarget)
            {
                nearestIndex = i;
                distanceToNearestTarget = dist;
            }
        }
    }

    private void Update()
    {
        ShowAndHideTrigger(); // for menu
        SendShortHaptics();   // to heighten tension
        BuildSuspenseWithHaptics();
    }
}
