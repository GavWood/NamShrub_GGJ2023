using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BWGameState : MonoBehaviour
{
    int monsterIndex = 0;                       // Which hole the monster is in
    public static CapsuleCollider[] capsules;   // Where the holes are
    public static bool isDead = false;          // Are you dead

    AudioSource[] audioSources;
    [SerializeField]
    AudioClip death;
    public BWGameState instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // Get the wood beast
        GameObject woodBeast = BWTransform.FindAlways("WoodBeast").gameObject;

        // Get the targets
        capsules = woodBeast.GetComponentsInChildren<CapsuleCollider>();

        Debug.Log("Capsules: " + capsules.Length);

        audioSources = GetComponentsInChildren<AudioSource>();
        ResetGame();
    }

    // Start is called before the first frame update
    public void ResetGame()
    {
        isDead = false;
        monsterIndex = Random.Range(0, capsules.Length - 1);
    }

    // Update is called once per frame
    void Update()
    {
        // How far are we away from the bottom of the tube
        float dist = BWHand.distanceToNearestTarget;

        // Calculate death
        if (dist < BWHand.MinDistance)
        {
            if (monsterIndex == BWHand.nearestIndex)
            {
                if( isDead == false )
                {
                    isDead = true;
                    PlayDeathSound();
                }
            }
        }

        // Show the debug string
        string text = "Nearest distance: " + dist +
                      "\nMonster: " + monsterIndex +
                      "\nDeath " + isDead;
 
        BWDebug.instance.AddText(text);
    }

    public void PlayDeathSound()
    {
        audioSources[1].PlayOneShot(death);
    }
}
