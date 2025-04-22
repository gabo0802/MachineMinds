using UnityEngine;

public class LeaderboardNameChooseScript : MonoBehaviour{
    public TMPro.TMP_InputField nameInput; 

    public void OnContinueButtonPress(){
        if(nameInput.text.Trim() != ""){
            PlayerPrefs.SetString("leaderboardName", nameInput.text.Trim().Substring(0, 2));
        }
        Destroy(gameObject);
    }
}
