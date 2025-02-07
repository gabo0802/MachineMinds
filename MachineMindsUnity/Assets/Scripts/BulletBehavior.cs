using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    private Rigidbody2D rb;
    public float bulletSpeed = 1f;
    public float bulletLifeTime = 10f;
    private float bulletLifeTimer = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update(){
        rb.linearVelocity = transform.up * bulletSpeed; 

        

        if(bulletLifeTimer >= bulletLifeTime){
            Destroy(gameObject);
        }
        else{
            bulletLifeTimer += Time.deltaTime;
        }

    }
}
