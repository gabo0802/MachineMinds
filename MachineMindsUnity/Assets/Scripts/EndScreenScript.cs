using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor; //only for testing, remove when building

public class EndScreenScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void MainMenuButton(){
        SceneManager.LoadScene("Scenes/MainMenu", LoadSceneMode.Single);
    }

    public void QuitGame(){
        Application.Quit();
        EditorApplication.ExitPlaymode(); //only for testing, remove when building
    }
}
