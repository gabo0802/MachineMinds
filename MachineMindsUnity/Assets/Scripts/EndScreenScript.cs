using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
//using UnityEditor; //only for testing, remove when building

public class EndScreenScript : MonoBehaviour{
    public TMPro.TextMeshProUGUI finalPointsUI = null;
    private string saveFilePath = Application.dataPath + "/Resources/GameState.save";

    public void MainMenuButton(){
        SceneManager.LoadScene("Scenes/MainMenu", LoadSceneMode.Single);
    }

    public void QuitGame(){
        Application.Quit();
        //EditorApplication.ExitPlaymode(); //only for testing, remove when building
    }

    void Start(){
        if (File.Exists(saveFilePath)){
            string saveFileData = "";
            using (StreamReader saveFile = File.OpenText(saveFilePath)){
                string currentLine;
                while ((currentLine = saveFile.ReadLine()) != null){
                    saveFileData += currentLine + "\n";
                }
            }

            string[] fileArray = saveFileData.Split('\n');
            string totalPoints = fileArray[1];

            finalPointsUI.text = "Final Score: " + totalPoints;
            //test comment
         }
    }
}
