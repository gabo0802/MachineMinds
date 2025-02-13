using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor; //only for testing, remove when building

public class GameManager : MonoBehaviour
{
    public void PlayGame(){
        SceneManager.LoadScene("Scenes/Levels/Level1", LoadSceneMode.Single);
    }

    public void QuitGame(){
        Application.Quit();
        EditorApplication.ExitPlaymode(); //only for testing, remove when building
    }
}