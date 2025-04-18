using UnityEngine;

public class LazerBeamScript : MonoBehaviour
{
    public AudioSource soundEffectSoundPlayer;

    private void volumeAdjustments(){
        if (soundEffectSoundPlayer && PlayerPrefs.HasKey("SoundEffectVolume")){
            soundEffectSoundPlayer.volume = PlayerPrefs.GetFloat("SoundEffectVolume");
        }
    } 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
    }

    // Update is called once per frame
    void Update(){
        volumeAdjustments();
    }

    void OnTriggerEnter2D(Collider2D other){
        if(!other.transform.name.ToLower().Contains("boss") && !other.transform.name.ToLower().Contains("wall") ){
            GameObject tempGameObject = new GameObject();
            other.transform.SendMessage("OnBulletHit", tempGameObject);
        }
    }

}