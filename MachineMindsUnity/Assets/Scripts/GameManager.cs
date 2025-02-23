using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//using UnityEditor; //only for testing, remove when building
using UnityEngine.UI;
using System.IO;

public class GameManager : MonoBehaviour{
    public Button loadGameButton;
    private string filePath = Application.dataPath + "/Resources/GameState.save";
    private int lastLevelNumber = 1;

     void LoadSave(){
        if (File.Exists(filePath)){
            string saveFileData = "";
            using (StreamReader saveFile = File.OpenText(filePath)){
                string currentLine;
                while ((currentLine = saveFile.ReadLine()) != null){
                    saveFileData += currentLine + "\n";
                }
            }

            string[] fileArray = saveFileData.Split('\n');
            lastLevelNumber = System.Int32.Parse(fileArray[0]);
        }
    }

    public void PlayGame(){
        SceneManager.LoadScene("Scenes/Levels/Level1", LoadSceneMode.Single);
    }

    public void LoadGame(){
        Debug.Log(lastLevelNumber);
        LoadSave();
        Debug.Log(lastLevelNumber);
        SceneManager.LoadScene("Scenes/Levels/Level" + lastLevelNumber, LoadSceneMode.Single);
    }

    public void QuitGame(){
        Application.Quit();
        //EditorApplication.ExitPlaymode(); //only for testing, remove when building
    }

    void Start(){
        loadGameButton.interactable = File.Exists(filePath);
    }
}