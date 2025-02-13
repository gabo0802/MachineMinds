using UnityEngine;

public class WallBehavior : MonoBehaviour{
    public bool isDestroyable = false;

    void OnBulletHit(string bulletType){
        if(isDestroyable){
            Destroy(gameObject);
        }
    }
}
