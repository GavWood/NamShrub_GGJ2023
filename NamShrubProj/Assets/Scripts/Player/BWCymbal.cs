using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BWCymbal : MonoBehaviour
{
    [SerializeField]
    AudioClip symbal;
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (BWKeyboard.Is_Space_Pressed() )
        {
            audioSource.PlayOneShot(symbal);
        }
    }
}
