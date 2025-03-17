using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PauseMenuScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = 0f;
    }

    public void onContinueButtonPress()
    {
        Time.timeScale = 1f;
        Destroy(gameObject);
    }

    public void MainMenuButtoon()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
