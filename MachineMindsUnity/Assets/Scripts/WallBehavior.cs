using UnityEngine;

public class WallBehavior : MonoBehaviour
{
    public bool isDestroyable = false;
    public bool isExplodable = false;
    public GameObject wallExplosionObject;
    public AudioSource destroyWallSoundPlayer;
    void OnExplosionHit()
    {
        Debug.Log(gameObject.name + " got hit be explosion");

        if (isExplodable)
        {
            Destroy(gameObject);
            AstarPath.active.Scan();
        }
    }

    void OnBulletHit(GameObject bulletType)
    {
        if (isDestroyable)
        {
            bulletType.SendMessage("OnDestroyableWallHit");
            if(destroyWallSoundPlayer){
                if (PlayerPrefs.HasKey("SoundEffectVolume")){
                    destroyWallSoundPlayer.volume = PlayerPrefs.GetFloat("SoundEffectVolume");
                }
                destroyWallSoundPlayer.Play();
                Destroy(gameObject, 0.25f);
            }else{
                Destroy(gameObject);
            }
            Destroy(bulletType);
            AstarPath.active.Scan();
        }
    }
}
