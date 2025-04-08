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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        for(int i = 0; i < keyChangeInputs.Length; i++){
            if(PlayerPrefs.HasKey(playerPrefsKeyMatch[i])){
                keyChangeInputs[i].text = PlayerPrefs.GetString(playerPrefsKeyMatch[i]);
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
                keyChangeInputs[currentKeyIndex].text = keyCodeString;
                currentKeyIndex = -1;
            }catch(System.Exception){
                keyChangeInputs[currentKeyIndex].text = "err";
            }
        }
    }

    void Update(){
        KeyCode pauseKey = PlayerPrefs.HasKey("KeyPause") ? (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("KeyPause")): KeyCode.Escape;
        if (Input.GetKeyDown(pauseKey))
        {
            Destroy(gameObject);
        }
    }
}