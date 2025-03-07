using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//using UnityEditor; //only for testing, remove when building

public class EndScreenScript : MonoBehaviour{
    public TMPro.TextMeshProUGUI finalPointsUI = null;
    private const string SAVE_KEY = "GameState";

    public void MainMenuButton(){
        SceneManager.LoadScene("Scenes/MainMenu", LoadSceneMode.Single);
    }

    public void QuitGame(){
        Application.Quit();
        //EditorApplication.ExitPlaymode(); //only for testing, remove when building
    }

    void Start(){
        if (WebGLSaveSystem.FileExists(SAVE_KEY)){
            string saveFileData = WebGLSaveSystem.ReadAllText(SAVE_KEY);
            
            string[] fileArray = saveFileData.Split('\n');
            string totalPoints = fileArray[1];
            
            finalPointsUI.text = "Final Score: " + totalPoints;
            //test comment
        }
    }
}
