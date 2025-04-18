using UnityEngine;

/// <summary>
/// Manages the Options menu, including screen resolution, fullscreen toggle,
/// and volume settings for music and sound effects.
/// </summary>
public class OptionsMenuScript : MonoBehaviour
{
    public UnityEngine.UI.Slider musicVolumeSlider;
    public UnityEngine.UI.Slider soundEffectVolumeSlider;
    public TMPro.TMP_Dropdown resolutionChanging;
    public UnityEngine.UI.Toggle isFullScreen;

    /// <summary>
    /// Initializes UI elements with current resolution, fullscreen state,
    /// and saved volume preferences.
    /// </summary>
    void Start()
    {   
        resolutionChanging.options[0].text = Screen.currentResolution.width + " x " + Screen.currentResolution.height;
        
        if (PlayerPrefs.HasKey("WindowFullScreen"))
            isFullScreen.isOn = PlayerPrefs.GetInt("WindowFullScreen") == 1;

        musicVolumeSlider.value = PlayerPrefs.HasKey("MusicVolume")
            ? PlayerPrefs.GetFloat("MusicVolume")
            : 1f;

        soundEffectVolumeSlider.value = PlayerPrefs.HasKey("SoundEffectVolume")
            ? PlayerPrefs.GetFloat("SoundEffectVolume")
            : 1f;
    }

    /// <summary>
    /// Closes the Options menu when the Exit button is pressed.
    /// </summary>
    public void onExitButtonPress()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Saves the adjusted music volume to PlayerPrefs.
    /// </summary>
    public void onMusicVolumeAdjustment()
    {
        PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
    }

    /// <summary>
    /// Saves the adjusted sound effect volume to PlayerPrefs.
    /// </summary>
    public void onSoundEffectVolumeAdjust()
    {
        PlayerPrefs.SetFloat("SoundEffectVolume", soundEffectVolumeSlider.value);
    }

    /// <summary>
    /// Updates screen resolution and fullscreen state based on user selection,
    /// and saves preferences.
    /// </summary>
    public void onResolutionAdjustment()
    {
        string resolutionString = resolutionChanging.options[resolutionChanging.value].text;
        int width = int.Parse(resolutionString.Split(" x ")[0]);
        int height = int.Parse(resolutionString.Split(" x ")[1]);

        PlayerPrefs.SetInt("WindowFullScreen", isFullScreen.isOn ? 1 : 0);
        Screen.SetResolution(width, height, isFullScreen.isOn);
    }

    /// <summary>
    /// Listens for the pause key to close the Options menu.
    /// </summary>
    void Update()
    {
        KeyCode pauseKey = PlayerPrefs.HasKey("KeyPause")
            ? (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("KeyPause"))
            : KeyCode.Escape;

        if (Input.GetKeyDown(pauseKey))
            Destroy(gameObject);
    }
}
