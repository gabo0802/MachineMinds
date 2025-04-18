using UnityEngine;

/// <summary>
/// Manages the lifecycle and visual scaling of an explosion effect,
/// including timed destruction and sound volume adjustments.
/// </summary>
public class ExplosionScript : MonoBehaviour
{
    public float explosionTime = 0.1f;   // Duration before the explosion object destroys itself
    private float explosionTimer;
    private float explosionRadiusMax = 0f;
    public AudioSource soundEffectSoundPlayer;

    /// <summary>
    /// Applies saved volume settings to the explosion sound effect.
    /// </summary>
    private void volumeAdjustments()
    {
        if (soundEffectSoundPlayer && PlayerPrefs.HasKey("SoundEffectVolume"))
        {
            soundEffectSoundPlayer.volume = PlayerPrefs.GetFloat("SoundEffectVolume");
        }
    }

    /// <summary>
    /// Unity Start method (unused initialization placeholder).
    /// </summary>
    void Start()
    {
        // No initialization needed currently
    }

    /// <summary>
    /// Sets the target maximum radius for the explosion scaling logic.
    /// </summary>
    void setExplosionMaxRadius(float newRadius)
    {
        explosionRadiusMax = newRadius;
    }

    /// <summary>
    /// Unity Update: scales the explosion over time, plays sound adjustments,
    /// and destroys the GameObject when its duration expires.
    /// </summary>
    void Update()
    {
        volumeAdjustments();

        if (explosionTimer >= explosionTime)
        {
            Destroy(gameObject);
        }
        else
        {
            if (explosionRadiusMax > 0f)
            {
                float scale = (explosionTimer * explosionRadiusMax) / explosionTime;
                transform.localScale = new Vector3(scale, scale, scale);
            }
            explosionTimer += Time.deltaTime;
        }
    }
}
