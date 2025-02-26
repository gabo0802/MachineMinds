using UnityEngine;

public class WallBehavior : MonoBehaviour{
    public bool isDestroyable = false;
    public bool isExplodable = false;
    public bool slowsCharacter = false;

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
            if(other.gameObject.tag == "Player" || other.gameObject.tag == "Enemy"){
               Debug.Log("Slowing down character");
            }
        }
    }
}
