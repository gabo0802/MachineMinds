using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ControlMenuScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        Time.timeScale = 0f;
    }

    public void onBackButtonPress(){
        Time.timeScale = 1f;
        Destroy(gameObject);
    }


    // Update is called once per frame
    void Update(){
        
    }
}