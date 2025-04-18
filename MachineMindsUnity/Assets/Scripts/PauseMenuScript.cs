using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Handles the in-game pause menu: pausing/unpausing, and loading sub-menus.
/// </summary>
public class PauseMenuScript : MonoBehaviour
{
    public GameObject controlsMenuPrefab;
    public GameObject optionsMenuPrefab;
    private GameObject additionalMenu;

    /// <summary>
    /// Pauses the game and shows the cursor when the pause menu is created.
    /// </summary>
    void Start()
    {
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Listens for the pause key to resume if no additional menu is open.
    /// </summary>
    void Update()
    {
        KeyCode pauseKey = PlayerPrefs.HasKey("KeyPause")
            ? (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("KeyPause"))
            : KeyCode.Escape;

        if (!additionalMenu && Input.GetKeyDown(pauseKey))
        {
            QuitPause();
        }
    }

    /// <summary>
    /// Continues the game if no sub-menu is open.
    /// </summary>
    public void onContinueButtonPress()
    {
        if (!additionalMenu)
            QuitPause();
    }

    /// <summary>
    /// Opens the Options sub-menu.
    /// </summary>
    public void onOptionsButtonPress()
    {
        additionalMenu = Instantiate(optionsMenuPrefab);
    }

    /// <summary>
    /// Opens the Controls sub-menu.
    /// </summary>
    public void onControlsButtonPress()
    {
        additionalMenu = Instantiate(controlsMenuPrefab);
    }

    /// <summary>
    /// Resumes the game and hides the pause menu.
    /// </summary>
    private void QuitPause()
    {
        Cursor.visible = false;
        Time.timeScale = 1f;
        Destroy(gameObject);
    }

    /// <summary>
    /// Returns to the main menu scene from the pause menu.
    /// </summary>
    public void MainMenuButtoon()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }
}
