using UnityEngine;

/// <summary>
/// Controls laser beam behavior: applies volume settings and handles collision triggers to damage targets.
/// </summary>
public class LazerBeamScript : MonoBehaviour
{
    public AudioSource soundEffectSoundPlayer;

    /// <summary>
    /// Applies saved sound effect volume to the laser beam audio source.
    /// </summary>
    private void volumeAdjustments()
    {
        if (soundEffectSoundPlayer && PlayerPrefs.HasKey("SoundEffectVolume"))
        {
            soundEffectSoundPlayer.volume = PlayerPrefs.GetFloat("SoundEffectVolume");
        }
    }

    /// <summary>
    /// Unity Start method (no initialization required).
    /// </summary>
    void Start()
    {
        // No setup needed
    }

    /// <summary>
    /// Unity Update: adjusts volume each frame.
    /// </summary>
    void Update()
    {
        volumeAdjustments();
    }

    /// <summary>
    /// Handles trigger collisions: sends an OnBulletHit message to any non-boss, non-wall object.
    /// </summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        string nameLower = other.transform.name.ToLower();
        if (!nameLower.Contains("boss") && !nameLower.Contains("wall"))
        {
            GameObject tempGameObject = new GameObject();
            other.transform.SendMessage("OnBulletHit", tempGameObject);
        }
    }
}