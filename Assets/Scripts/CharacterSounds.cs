using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSounds : MonoBehaviour
{
    public AK.Wwise.Event Footsteps;
    public AK.Wwise.Event Chopping;
    public AK.Wwise.Event Sliding;
    public AK.Wwise.Event Whetstone;
    public AK.Wwise.Event Inspect;
    public AK.Wwise.Event PigWalk;
    // Start is called before the first frame update

    public void PlayFootsteps()
    {
        Footsteps.Post(gameObject);
    }

    public void PlayChopping()
    {
        Chopping.Post(gameObject);
    }

    public void PlaySliding()
    {
        Sliding.Post(gameObject);
    }

    public void PlayWhetstone()
    {
        Whetstone.Post(gameObject);
    }

    public void PlayInspect()
    {
        Inspect.Post(gameObject);
    }

    public void PlayPigWalk()
    {
        PigWalk.Post(gameObject);
    }

}
