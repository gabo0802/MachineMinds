
using UnityEngine;

public class OptionsMenuScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public UnityEngine.UI.Slider musicVolumeSlider;
    public UnityEngine.UI.Slider soundEffectVolumeSlider;
    public TMPro.TMP_Dropdown resolutionChanging;
    public UnityEngine.UI.Toggle isFullScreen;

    void Start()
    {   
        resolutionChanging.options[0].text = Screen.currentResolution.width + " x " + Screen.currentResolution.height;
        
        if (PlayerPrefs.HasKey("WindowFullScreen")){
            isFullScreen.isOn = PlayerPrefs.GetInt("WindowFullScreen") == 1 ? true : false;
        }

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

    public void onResolutionAdjustment(){
        string resolutionString = resolutionChanging.options[resolutionChanging.value].text;
        int width = System.Int32.Parse(resolutionString.Split(" x ")[0]);
        int height = System.Int32.Parse(resolutionString.Split(" x ")[1]);

        int isFullScreenInt = isFullScreen.isOn ? 1 : 0;
        PlayerPrefs.SetInt("WindowFullScreen", isFullScreenInt);

        Screen.SetResolution(width, height, isFullScreen.isOn);
    }

    void Update(){
        KeyCode pauseKey = PlayerPrefs.HasKey("KeyPause") ? (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("KeyPause")): KeyCode.Escape;
        if (Input.GetKeyDown(pauseKey))
        {
            Destroy(gameObject);
        }
    }
}
