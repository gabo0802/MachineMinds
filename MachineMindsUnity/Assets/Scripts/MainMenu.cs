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

public class GameManager : MonoBehaviour
{
    public Button loadGameButton;
    public GameObject controlsMenuPrefab;
    public GameObject optionsMenuPrefab;

    private string filePath = "GameState";
    private int lastLevelNumber = 1;

    public AudioSource musicPlayer;

    private void volumeAdjustments(){
        if (musicPlayer && PlayerPrefs.HasKey("MusicVolume")){
            musicPlayer.volume = PlayerPrefs.GetFloat("MusicVolume");
        }
    }
    public void PlayGame()
    {
        string content = "3\n0\n0\n1\n0\nfalse";
        SaveSystem.WriteAllText(filePath, content);

        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    public void LoadGame()
    {
        string content = "3\n0\n0\n1\n0\ntrue";
        SaveSystem.WriteAllText(filePath, content);

        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

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
    public void LoadControls()
    {
        Instantiate(controlsMenuPrefab);
    }

    public void LoadOptions()
    {
        Instantiate(optionsMenuPrefab);
    }

    void Update(){
        volumeAdjustments();
    }

}