using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.UI;
using static SaveSystem;
using System.IO;

/// <summary>
/// Manages core game flow: play, load, quit, and menu workflows.
/// </summary>
public class GameManager : MonoBehaviour
{
    public Button loadGameButton;
    public GameObject controlsMenuPrefab;
    public GameObject optionsMenuPrefab;

    private string filePath = "GameState";
    private int lastLevelNumber = 1;

    public AudioSource musicPlayer;

    /// <summary>
    /// Adjusts the background music volume based on saved preferences.
    /// </summary>
    public void volumeAdjustments()
    {  // Changed to public
        if (musicPlayer && PlayerPrefs.HasKey("MusicVolume"))
        {
            musicPlayer.volume = PlayerPrefs.GetFloat("MusicVolume") * 0.5f;
        }
    }

    /// <summary>
    /// Starts a new game by initializing save data and loading the first level.
    /// </summary>
    public void PlayGame()
    {
        string content = "3\n0\n0\n1\n0\nfalse";
        SaveSystem.WriteAllText(filePath, content);
        PlayerPrefs.SetInt("CheckpointLevelDeaths", 0);
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    /// <summary>
    /// Loads a saved game by applying saved state and loading the first level.
    /// </summary>
    public void LoadGame()
    {
        string content = "3\n0\n0\n1\n0\ntrue";
        SaveSystem.WriteAllText(filePath, content);

        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    /// <summary>
    /// Exits play mode or application depending on the platform.
    /// </summary>
    public void QuitGame()
    {
        if (Application.isEditor)
        {
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#endif
            return;
        }
        else if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            Debug.Log("Quitting game in WebGL is not supported.");
        }
        else
        {
            Debug.Log("Quitting game in build");
            Application.Quit();
        }
    }

    /// <summary>
    /// Displays the controls configuration menu.
    /// </summary>
    public void LoadControls()
    {
        Instantiate(controlsMenuPrefab);
    }

    /// <summary>
    /// Displays the options/settings menu.
    /// </summary>
    public void LoadOptions()
    {
        Instantiate(optionsMenuPrefab);
    }

    /// <summary>
    /// Called every frame to keep music volume updated.
    /// </summary>
    void Update()
    {
        volumeAdjustments();
    }
}
