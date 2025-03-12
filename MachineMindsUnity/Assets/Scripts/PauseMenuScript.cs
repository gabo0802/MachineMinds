using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PauseMenuScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        Time.timeScale = 0f;
    }

    public void onContinueButtonPress(){
        Time.timeScale = 1f;
        Destroy(gameObject);
    }

    public void onQuitButtonPress(){
        Time.timeScale = 1f;

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

    // Update is called once per frame
    void Update(){
        
    }
}
