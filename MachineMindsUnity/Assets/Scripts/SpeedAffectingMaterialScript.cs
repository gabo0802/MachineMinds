using UnityEngine;

public class SpeedAffectingMaterialScript : MonoBehaviour{
    public float speedAffect = 1f; // > 1f = speed boost; < 1f = speed reduction

    void OnTriggerEnter2D(Collider2D other){
        Debug.Log("changing speed of something");
       
        if(!other.gameObject.name.ToLower().Contains("bullet") && (other.gameObject.name.ToLower().Contains("player") || other.gameObject.name.ToLower().Contains("enemy"))){
        //if(other.gameObject.tag == "Player" || other.gameObject.tag == "Enemy"){
            other.transform.SendMessage("AffectSlowdownSpeed", speedAffect);
        }
    }

    void OnTriggerExit2D(Collider2D other){
        Debug.Log("reverting to original speed");
        
        if(!other.gameObject.name.ToLower().Contains("bullet") && (other.gameObject.name.ToLower().Contains("player") || other.gameObject.name.ToLower().Contains("enemy"))){
            //if(other.gameObject.tag == "Player" || other.gameObject.tag == "Enemy"){
            other.transform.SendMessage("AffectSlowdownSpeed", 1f);
        }
    }
}
