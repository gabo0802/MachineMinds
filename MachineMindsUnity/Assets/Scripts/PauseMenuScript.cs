using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PauseMenuScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public GameObject controlsMenuPrefab;
    public GameObject optionsMenuPrefab;
        private GameObject additionalMenu;
    void Start()
    {
        Time.timeScale = 0f;
    }

    void Update()
    {
        KeyCode pauseKey = PlayerPrefs.HasKey("KeyPause") ? (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("KeyPause")): KeyCode.Escape;
        if (!additionalMenu && Input.GetKeyDown(pauseKey))
        {
            QuitPause();
        }
    }

    public void onContinueButtonPress()
    {
        if(!additionalMenu){
            QuitPause();
        }
    }

    public void onOptionsButtonPress()
    {
        additionalMenu = (GameObject) Instantiate(optionsMenuPrefab);
    }

    public void onControlsButtonPress()
    {
        additionalMenu = (GameObject) Instantiate(controlsMenuPrefab);
    }

    private void QuitPause()
    {
        Time.timeScale = 1f;
        Destroy(gameObject);
    }

    public void MainMenuButtoon()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }
}
