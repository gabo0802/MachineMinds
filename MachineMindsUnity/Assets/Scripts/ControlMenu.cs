using UnityEngine;
using UnityEngine.SceneManagement;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Manages the controls settings menu: displays current key bindings,
/// allows remapping keys, resetting to defaults, and handles confirmation dialogs.
/// </summary>
public class ControlsMenuScriptMenuScript : MonoBehaviour
{
    private int currentKeyIndex = -1;
    public TMPro.TextMeshProUGUI[] keyChangeInputs;
    public string[] playerPrefsKeyMatch;
    public string[] defaultKeyCode;
    public GameObject confirmationWindowObject;
    private GameObject currentConfirmationWindowObject;

    /// <summary>
    /// Initializes UI fields with saved keybindings or defaults on start.
    /// </summary>
    void Start()
    {
        for (int i = 0; i < keyChangeInputs.Length; i++)
        {
            if (PlayerPrefs.HasKey(playerPrefsKeyMatch[i]))
            {
                string keyCodeString = PlayerPrefs.GetString(playerPrefsKeyMatch[i]);
                if (i != 4 && i != 5 && keyCodeString.Length > 3)
                {
                    keyCodeString = keyCodeString.Substring(0, 3);
                }
                keyChangeInputs[i].text = keyCodeString;
            }
            else
            {
                keyChangeInputs[i].text = defaultKeyCode[i];
            }
        }
    }

    /// <summary>
    /// Closes the controls menu when the back button is pressed.
    /// </summary>
    public void onBackButtonPress()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Marks a key input field as active for remapping.
    /// </summary>
    public void onKeyChangePress(int keyIndex)
    {
        keyChangeInputs[keyIndex].text = "...";
        currentKeyIndex = keyIndex;
    }

    /// <summary>
    /// Resets all keybindings to their default values and updates the UI.
    /// </summary>
    public void resetAllKeys()
    {
        for (int i = 0; i < keyChangeInputs.Length; i++)
        {
            if (PlayerPrefs.HasKey(playerPrefsKeyMatch[i]))
                PlayerPrefs.DeleteKey(playerPrefsKeyMatch[i]);

            keyChangeInputs[i].text = defaultKeyCode[i];
        }
    }

    /// <summary>
    /// Captures and saves a new keybinding when a key is pressed after initiating remap.
    /// Displays a confirmation dialog upon success.
    /// </summary>
    void OnGUI()
    {
        Event e = Event.current;
        if (!currentConfirmationWindowObject && currentKeyIndex != -1 && e.isKey)
        {
            try
            {
                string keyCodeString = e.keyCode.ToString();
                PlayerPrefs.SetString(playerPrefsKeyMatch[currentKeyIndex], keyCodeString);
                if (currentKeyIndex != 4 && currentKeyIndex != 5 && keyCodeString.Length > 3)
                    keyCodeString = keyCodeString.Substring(0, 3);

                keyChangeInputs[currentKeyIndex].text = keyCodeString;
                currentKeyIndex = -1;
                currentConfirmationWindowObject = Instantiate(confirmationWindowObject);
            }
            catch (Exception)
            {
                keyChangeInputs[currentKeyIndex].text = "err";
            }
        }
    }

    /// <summary>
    /// Listens for the pause key to close the menu if no confirmation is open.
    /// </summary>
    void Update()
    {
        KeyCode pauseKey = PlayerPrefs.HasKey("KeyPause")
            ? (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("KeyPause"))
            : KeyCode.Escape;

        if (!currentConfirmationWindowObject && Input.GetKeyDown(pauseKey))
        {
            Destroy(gameObject);
        }
    }
}