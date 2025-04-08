using UnityEngine;

public class ShieldScript : MonoBehaviour
{
    public AudioSource soundEffectSoundPlayer;

    private void volumeAdjustments()
    {
        if (soundEffectSoundPlayer && PlayerPrefs.HasKey("SoundEffectVolume"))
        {
            soundEffectSoundPlayer.volume = PlayerPrefs.GetFloat("SoundEffectVolume");
        }
    }

    void OnExplosionHit()
    {
        return;
    }

    void OnBulletHit(GameObject bullet)
    {
        Destroy(bullet);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.name.ToLower().Contains("bullet"))
        {
            Destroy(other.gameObject);
        }
    }

    void Update()
    {
        volumeAdjustments();
    }
}
