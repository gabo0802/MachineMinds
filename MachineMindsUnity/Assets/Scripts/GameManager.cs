using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//using UnityEditor; //only for testing, remove when building
using UnityEngine.UI;
using static WebGLSaveSystem;
using System.IO;

public class GameManager : MonoBehaviour
{
    public Button loadGameButton;
    private string filePath = "GameState";
    private int lastLevelNumber = 1;


    public void PlayGame()
    {
        string content = "3\n0\n0\n1\n0\nfalse";
        WebGLSaveSystem.WriteAllText(filePath, content);

        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    public void LoadGame()
    {
        string content = "3\n0\n0\n1\n0\ntrue";
        WebGLSaveSystem.WriteAllText(filePath, content);

        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    public void QuitGame()
    {
        Application.Quit();
        //EditorApplication.ExitPlaymode(); //only for testing, remove when building
    }
}