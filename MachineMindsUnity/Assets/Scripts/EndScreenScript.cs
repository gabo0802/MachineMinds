using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class EndScreenScript : MonoBehaviour
{
    public TMPro.TextMeshProUGUI finalPointsUI = null;
    private const string SAVE_KEY = "GameState";

    public AudioSource musicPlayer;

    private void volumeAdjustments(){
        if (musicPlayer && PlayerPrefs.HasKey("MusicVolume")){
            musicPlayer.volume = PlayerPrefs.GetFloat("MusicVolume");
        }
    }
    public void MainMenuButton()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    public void QuitGame()
    {
        if (Application.isEditor)
        {
            Debug.Log("Quitting game in editor");
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#endif
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

    void Start()
    {   
        volumeAdjustments();
        if (SaveSystem.FileExists(SAVE_KEY))
        {
            string saveFileData = SaveSystem.ReadAllText(SAVE_KEY);

            string[] fileArray = saveFileData.Split('\n');
            string totalPoints = fileArray[1];

            finalPointsUI.text = "Final Score: " + totalPoints;
            //test comment
        }
    }
}

