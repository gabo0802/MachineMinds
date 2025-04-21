using UnityEngine;

public class ExplosionScript : MonoBehaviour{
    public float explosionTime = 0.1f;
    private float explosionTimer;

    private float explosionRadiusMax = 0f;

    public AudioSource soundEffectSoundPlayer;

    private void volumeAdjustments(){
        if (soundEffectSoundPlayer && PlayerPrefs.HasKey("SoundEffectVolume")){
            soundEffectSoundPlayer.volume = PlayerPrefs.GetFloat("SoundEffectVolume");
        }
    } 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
    }

    void setExplosionMaxRadius(float newRadius){
        explosionRadiusMax = newRadius;
    }

    // Update is called once per frame
    void Update(){
        volumeAdjustments();

        if(explosionTimer >= explosionTime){
            Destroy(gameObject);
        }else{
            if(explosionRadiusMax > 0f){
                float currentScale = (explosionTimer * explosionRadiusMax) / explosionTime;
                gameObject.transform.localScale = new Vector3(currentScale, currentScale, currentScale);
            }

            explosionTimer += Time.deltaTime;
        }
    }
}