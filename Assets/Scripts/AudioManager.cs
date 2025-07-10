using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource sfxSource;

    [Header("Player Sounds")]
    public AudioClip jumpClip;
    public AudioClip landClip;
    public AudioClip footstepClip;

    [Header("Enemy Sounds")]
    public AudioClip alertClip;
    public AudioClip chaseClip;
    public AudioClip patrolStepClip;

    private void Awake()
    {
        if (!sfxSource)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }
    }

    /// <summary>
    /// Play a one-shot sound with optional volume control
    /// </summary>
    public void PlaySound(AudioClip clip, float volume = 1f)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip, volume);
        }
    }

    // --- Utility Wrappers ---

    public void PlayJump() => PlaySound(jumpClip);
    public void PlayLand() => PlaySound(landClip);
    public void PlayFootstep() => PlaySound(footstepClip);

    public void PlayEnemyAlert() => PlaySound(alertClip);
    public void PlayEnemyChase() => PlaySound(chaseClip);
    public void PlayPatrolStep() => PlaySound(patrolStepClip);
}
