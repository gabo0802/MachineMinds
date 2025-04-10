using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TestGameManager
{
    private GameManager gameManager;
    private GameObject gameManagerObject;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        // Create a new GameObject with GameManager
        gameManagerObject = new GameObject("GameManager");
        gameManager = gameManagerObject.AddComponent<GameManager>();

        // Setup required components
        gameManager.loadGameButton = new GameObject("LoadButton").AddComponent<Button>();
        gameManager.controlsMenuPrefab = new GameObject("ControlsMenu");
        gameManager.optionsMenuPrefab = new GameObject("OptionsMenu");
        gameManager.musicPlayer = gameManagerObject.AddComponent<AudioSource>();

        // Make the GameManager persist between scenes
        Object.DontDestroyOnLoad(gameManagerObject);

        yield return null;
    }

    [UnityTearDown]
    public IEnumerator Teardown()
    {
        // Load an empty scene first to clean up
        yield return SceneManager.LoadSceneAsync(0);

        if (gameManagerObject != null)
        {
            Object.Destroy(gameManagerObject);
        }
        PlayerPrefs.DeleteAll();
        yield return null;
    }

    [UnityTest]
    public IEnumerator PlayGame_LoadsFirstLevel()
    {
        int initialSceneIndex = SceneManager.GetActiveScene().buildIndex;
        gameManager.PlayGame();

        yield return new WaitForSeconds(0.1f); // Wait for scene load

        Assert.AreEqual(1, SceneManager.GetActiveScene().buildIndex);
        Assert.IsNotNull(gameManagerObject); // Verify GameManager still exists
    }

    [UnityTest]
    public IEnumerator LoadGame_LoadsFirstLevelAndSetsGameState()
    {
        // Test the game state
        gameManager.LoadGame();

        // Check game state
        string content = SaveSystem.ReadAllText("GameState");
        Assert.AreEqual("3\n0\n0\n1\n0\ntrue", content);

        // Wait for scene load
        yield return new WaitForSeconds(0.1f);

        // Check scene load
        Assert.AreEqual(1, SceneManager.GetActiveScene().buildIndex);
        Assert.IsNotNull(gameManagerObject); // Verify GameManager still exists
    }

    [UnityTest]
    public IEnumerator VolumeAdjustments_UpdatesMusicVolumeOverTime()
    {
        float testVolume = 0.5f;
        PlayerPrefs.SetFloat("MusicVolume", testVolume);

        gameManager.musicPlayer.volume = 1.0f;
        yield return new WaitForSeconds(0.1f);

        Assert.AreEqual(testVolume * 0.5f, gameManager.musicPlayer.volume);
    }

    [UnityTest]
    public IEnumerator LoadControls_MenuPersistsAfterFrame()
    {
        gameManager.LoadControls();
        yield return null;

        GameObject controlsMenu = GameObject.Find("ControlsMenu(Clone)");
        Assert.NotNull(controlsMenu);
    }

    [UnityTest]
    public IEnumerator LoadOptions_MenuPersistsAfterFrame()
    {
        gameManager.LoadOptions();
        yield return null;

        GameObject optionsMenu = GameObject.Find("OptionsMenu(Clone)");
        Assert.NotNull(optionsMenu);
    }
}
