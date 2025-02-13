using UnityEngine;

public class WallBehavior : MonoBehaviour{
    public bool isDestroyable = false;

    void OnBulletHit(string bulletType){
        if(isDestroyable){
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        
    }

    // Update is called once per frame
    void Update(){
        
    }
}
