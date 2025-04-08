using UnityEngine;
using UnityEngine.SceneManagement;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ControlsMenuScriptMenuScript : MonoBehaviour
{
    /*
        KeyMovementUp
        KeyMovementDown
        KeyMovementLeft
        KeyMovementRight
        KeyBoost
        KeyShoot
        KeyPause
    */

    private int currentKeyIndex = -1;
    public TMPro.TextMeshProUGUI[] keyChangeInputs;
    public string[] playerPrefsKeyMatch;
    public string[] defaultKeyCode;
    public GameObject confirmationWindowObject;
    private GameObject currentConfirmationWindowObject;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        for(int i = 0; i < keyChangeInputs.Length; i++){
            if(PlayerPrefs.HasKey(playerPrefsKeyMatch[i])){
                string keyCodeString = PlayerPrefs.GetString(playerPrefsKeyMatch[i]);
                if(i != 4 && i != 5 && keyCodeString.Length > 3){
                    keyCodeString = keyCodeString.Substring(0, 3);
                }
                keyChangeInputs[i].text = keyCodeString;
            }else{
                keyChangeInputs[i].text = defaultKeyCode[i];
            }
        }
    }

    public void onBackButtonPress(){
        Destroy(gameObject);
    }

    public void onKeyChangePress(int keyIndex){
        keyChangeInputs[keyIndex].text = "...";
        currentKeyIndex = keyIndex;
    }

    // Update is called once per frame

    void OnGUI(){
        Event e = Event.current;
        if (currentKeyIndex != -1 && e.isKey){   
             try{
                string keyCodeString = Event.current.keyCode + "";
                PlayerPrefs.SetString(playerPrefsKeyMatch[currentKeyIndex], keyCodeString);
                if(currentKeyIndex != 4 && currentKeyIndex != 5 && keyCodeString.Length > 3){
                    keyCodeString = keyCodeString.Substring(0, 3);
                }
                keyChangeInputs[currentKeyIndex].text = keyCodeString;
                currentConfirmationWindowObject = (GameObject) Instantiate(confirmationWindowObject);
                currentKeyIndex = -1;
            }catch(System.Exception){
                keyChangeInputs[currentKeyIndex].text = "err";
            }
        }
    }

    void Update(){
        KeyCode pauseKey = PlayerPrefs.HasKey("KeyPause") ? (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("KeyPause")): KeyCode.Escape;
        if (!currentConfirmationWindowObject && Input.GetKeyDown(pauseKey))
        {
            Destroy(gameObject);
        }
    }
}