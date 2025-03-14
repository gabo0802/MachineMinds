using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ControlsMenuScriptMenuScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
    }

    public void onBackButtonPress(){
        Destroy(gameObject);
    }


    // Update is called once per frame
    void Update(){
        
    }
}