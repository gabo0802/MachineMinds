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
    private string filePath = "GameState";
    private int lastLevelNumber = 1;


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
            Debug.Log("Quitting game in editor");
            // EditorApplication.ExitPlaymode();
        }
        else
        {
            Debug.Log("Quitting game in build");
            Application.Quit();
        }
    }
}