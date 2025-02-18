using UnityEngine;

public class WallBehavior : MonoBehaviour{
    public bool isDestroyable = false;
    public bool isExplodable = false;

    void OnExplosionHit(){
        Debug.Log(gameObject.name + " got hit be explosion");

        if(isExplodable){
            Destroy(gameObject);
        }
    }

    void OnBulletHit(GameObject bulletType){
        if(isDestroyable){
            Destroy(gameObject);
        }
    }
}
