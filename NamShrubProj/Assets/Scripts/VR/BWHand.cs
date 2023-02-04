using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BWHand : MonoBehaviour
{
    float lastTime = 0;

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

    private void Update()
    {
        if (BWVR.IsTrigger(handId))
        {
            poseMarker.SetActive(true);
            lastTime = Time.time;
        }

        float currentTime = Time.time;

        if( (currentTime - lastTime) > 5.0f )
        {
            poseMarker.SetActive(false);
        }
    }
}
