using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BWGameState : MonoBehaviour
{
    int monsterIndex = 0;                       // Which hole the monster is in
    public static Transform[] targets;   // Where the holes are
    public static bool isDead = false;          // Are you dead

    AudioSource[] audioSources;

    public static BWGameState instance;
    BWHand[] hands;

    [SerializeField]
    AudioClip death;

    [SerializeField]
    public float height = 0.7f;

    public float KillDistance = 0.1f;

    private void Awake()
    {
        instance = this;
        hands = GetComponentsInChildren<BWHand>();
    }

    private void Start()
    {
        // Get the wood beast
        GameObject woodBeast = BWTransform.FindAlways("WoodBeast").gameObject;

        // Get the targets
        targets = woodBeast.GetComponentsInChildren<Transform>();

        Debug.Log("Capsules: " + targets.Length);

        audioSources = GetComponentsInChildren<AudioSource>();
        ResetGame();
    }

    // Start is called before the first frame update
    public void ResetGame()
    {
        isDead = false;
        monsterIndex = Random.Range(0, targets.Length - 1);
    }

    // Update is called once per frame
    void Update()
    {
        string text = "";

        // How far are we away from the bottom of the tube
        for (int i = 0; i < 2; i++)
        {
            Vector3 handPos = hands[i].transform.position;

            Vector3 monsterPos = targets[monsterIndex].position;

            float distance = Vector3.Distance(monsterPos, handPos);

            if( distance < KillDistance )
            {
                if (isDead == false)
                {
                    isDead = true;
                    PlayDeathSound();

                    BWVR.SendHaptic((BWHand.BWControllerType)i, 1.0f, 5.0f);
                }
            }

            float handHeight = handPos.y;

            // Show the debug string
            text += "\nHeight " + string.Format("Height: {0:#.00} cm", handHeight + " out of " + height);
            text += "\nDist " + distance;
        }

        text += "\nDeath " + isDead;
        text += "\nMonster: " + monsterIndex;

        BWDebug.instance.AddText(text);
    }

    public void PlayDeathSound()
    {
        audioSources[0].PlayOneShot(death);
    }
}
