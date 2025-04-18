using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Handles the end-of-game UI: displays final score, manages menu buttons,
/// volume settings, and saving to leaderboards.
/// </summary>
public class EndScreenScript : MonoBehaviour
{
    public TMPro.TextMeshProUGUI finalPointsUI = null;
    public GameObject leaderboardsPrefab;
    private const string SAVE_KEY = "GameState";

    public AudioSource musicPlayer;

    /// <summary>
    /// Adjusts the end screen music volume based on saved preferences.
    /// </summary>
    private void volumeAdjustments()
    {
        if (musicPlayer && PlayerPrefs.HasKey("MusicVolume"))
        {
            musicPlayer.volume = PlayerPrefs.GetFloat("MusicVolume") * 0.25f;
        }
    }

    /// <summary>
    /// Loads the main menu scene (index 0).
    /// </summary>
    public void MainMenuButton()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    /// <summary>
    /// Instantiates the leaderboards UI prefab.
    /// </summary>
    public void LeaderboardsButton()
    {
        Instantiate(leaderboardsPrefab);
    }

    /// <summary>
    /// Quits the game or stops playmode in the editor.
    /// Handles WebGL limitations and build exit.
    /// </summary>
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

    /// <summary>
    /// Unity Start: shows cursor, sets volume, reads saved score,
    /// updates UI, and records score to leaderboards.
    /// </summary>
    void Start()
    {   
        Cursor.visible = true;
        volumeAdjustments();
        if (SaveSystem.FileExists(SAVE_KEY))
        {
            string saveFileData = SaveSystem.ReadAllText(SAVE_KEY);
            string[] fileArray = saveFileData.Split('\n');
            string totalPoints = fileArray[1];

            finalPointsUI.text = "Final Score: " + totalPoints;

            // Save score to leaderboards
            float score = float.Parse(totalPoints);
            SaveSystem.SaveToLeaderboards(score);
        }
    }
}

