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

    public void MainMenuButton()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    public void QuitGame()
    {
        if (Application.isEditor)
        {
            Debug.Log("Quitting game in editor");
            EditorApplication.ExitPlaymode();
        }
        else
        {
            Debug.Log("Quitting game in build");
            Application.Quit();
        }
    }

    void Start()
    {
        if (WebGLSaveSystem.FileExists(SAVE_KEY))
        {
            string saveFileData = WebGLSaveSystem.ReadAllText(SAVE_KEY);

            string[] fileArray = saveFileData.Split('\n');
            string totalPoints = fileArray[1];

            finalPointsUI.text = "Final Score: " + totalPoints;
            //test comment
        }
    }
}

