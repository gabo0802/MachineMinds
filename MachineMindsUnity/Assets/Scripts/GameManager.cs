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
    private string filePath = Application.dataPath + "/Resources/GameState.save";
    private int lastLevelNumber = 1;

    void LoadSave()
    {
        if (WebGLSaveSystem.IsWebGLPlatform())
        {
            if (WebGLSaveSystem.FileExists(filePath))
            {
                string saveFileData = WebGLSaveSystem.ReadAllText(filePath);
                string[] fileArray = saveFileData.Split('\n');
                lastLevelNumber = System.Int32.Parse(fileArray[0]);
            }
        }
        else
        {
            if (File.Exists(filePath))
            {
                string saveFileData = "";
                using (StreamReader saveFile = File.OpenText(filePath))
                {
                    string currentLine;
                    while ((currentLine = saveFile.ReadLine()) != null)
                    {
                        saveFileData += currentLine + "\n";
                    }
                }

                string[] fileArray = saveFileData.Split('\n');
                lastLevelNumber = System.Int32.Parse(fileArray[0]);
            }
        }
    }

    public void PlayGame()
    {
        if (WebGLSaveSystem.IsWebGLPlatform())
        {
            string content = "3\n0\n0\n1\n0\nfalse";
            WebGLSaveSystem.WriteAllText(filePath, content);
        }
        else
        {
            using (StreamWriter sw = File.CreateText(filePath))
            {
                sw.WriteLine(3); //lives
                sw.WriteLine(0); //points
                sw.WriteLine(0); //enemies killed
                sw.WriteLine(1); //difficulty
                sw.WriteLine(0); //time alive
                sw.WriteLine(false); //training mode?
            }
        }

        SceneManager.LoadScene("Scenes/Levels/Level1", LoadSceneMode.Single);
    }

    public void LoadGame()
    {
        /*Debug.Log(lastLevelNumber);
        LoadSave();
        Debug.Log(lastLevelNumber);
        SceneManager.LoadScene("Scenes/Levels/Level" + lastLevelNumber, LoadSceneMode.Single);*/

        if (WebGLSaveSystem.IsWebGLPlatform())
        {
            string content = "3\n0\n0\n1\n0\ntrue";
            WebGLSaveSystem.WriteAllText(filePath, content);
        }
        else
        {
            using (StreamWriter sw = File.CreateText(filePath))
            {
                sw.WriteLine(3);
                sw.WriteLine(0);
                sw.WriteLine(0);
                sw.WriteLine(1);
                sw.WriteLine(0);
                sw.WriteLine(true);
            }
        }

        SceneManager.LoadScene("Scenes/Levels/Level1", LoadSceneMode.Single);
    }

    public void QuitGame()
    {
        Application.Quit();
        //EditorApplication.ExitPlaymode(); //only for testing, remove when building
    }
}