using UnityEngine;

public class WallBehavior : MonoBehaviour{
    public bool isDestroyable = false;

    void OnBulletHit(GameObject bulletType){
        if(isDestroyable){
            Destroy(gameObject);
        }
    }
}
