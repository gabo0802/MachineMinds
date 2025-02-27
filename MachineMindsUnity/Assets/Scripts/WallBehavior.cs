using UnityEngine;

public class WallBehavior : MonoBehaviour{
    public bool isDestroyable = false;
    public bool isExplodable = false;
    public bool slowsCharacter = false;
    public float slowDownRatio = 1f;

    void OnExplosionHit(){
        Debug.Log(gameObject.name + " got hit be explosion");

        if(isExplodable){
            Destroy(gameObject);
            AstarPath.active.Scan();           
        }
    }

    void OnBulletHit(GameObject bulletType){
        if(isDestroyable){
            Destroy(gameObject);
            Destroy(bulletType);
            AstarPath.active.Scan();           
        }
    }

    void OnTriggerEnter2D(Collider2D other){
        Debug.Log("Slowing down something");
        if(slowsCharacter){
            if(other.gameObject.name.ToLower().Contains("player") || other.gameObject.name.ToLower().Contains("enemy")){
            //if(other.gameObject.tag == "Player" || other.gameObject.tag == "Enemy"){
               other.transform.SendMessage("AffectSlowdownSpeed", slowDownRatio);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other){
        Debug.Log("Slowing down something");
        if(slowsCharacter){
            if(other.gameObject.name.ToLower().Contains("player") || other.gameObject.name.ToLower().Contains("enemy")){
            //if(other.gameObject.tag == "Player" || other.gameObject.tag == "Enemy"){
               other.transform.SendMessage("AffectSlowdownSpeed", slowDownRatio);
            }
        }
    }
}
