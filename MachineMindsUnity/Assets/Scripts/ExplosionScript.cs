using UnityEngine;

public class ExplosionScript : MonoBehaviour{
    public float explosionTime = 0.1f;
    private float explosionTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        
    }

    // Update is called once per frame
    void Update(){
        if(explosionTimer >= explosionTime){
            Destroy(gameObject);
        }else{
            explosionTimer += Time.deltaTime;
        }
    }
}
