
using UnityEngine;

public class OptionsMenuScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public UnityEngine.UI.Slider musicVolumeSlider;
    public UnityEngine.UI.Slider soundEffectVolumeSlider;

    void Start()
    {
        if (PlayerPrefs.HasKey("MusicVolume")){
            musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        }else{
            musicVolumeSlider.value = 1f;
        }

        if (PlayerPrefs.HasKey("SoundEffectVolume")){
            soundEffectVolumeSlider.value = PlayerPrefs.GetFloat("SoundEffectVolume");
        }else{
            soundEffectVolumeSlider.value = 1f;
        }
    }

    public void onExitButtonPress(){
        Destroy(gameObject);
    }

    public void onMusicVolumeAdjustment(){
        PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
    }

    public void onSoundEffectVolumeAdjust(){
        PlayerPrefs.SetFloat("SoundEffectVolume", soundEffectVolumeSlider.value);
    }
}
