using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigSounds : MonoBehaviour
{

    public AK.Wwise.Event Footsteps;
    public AK.Wwise.Event Pushing;


    public void PlayFootsteps()
    {
        Footsteps.Post(gameObject);
    }

    public void PlayPushing()
    {
        Pushing.Post(gameObject);
    }
}
