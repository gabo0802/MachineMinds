using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    private Rigidbody2D rb;
    public float bulletSpeed = 1f;
    public float bulletBounceAngle = 45f; //do we really want to have bouncy bullets?
    public float bulletLifeTime = 10f;
    private float bulletLifeTimer = 0f;

    void OnBulletHit(string bulletType){
      
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update(){
        //Constantly move
        rb.linearVelocity = transform.up * bulletSpeed; 

        //Object Hit Detection:
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, 0.25f); //in theory should work, if not need to change where looking (might be transform.up)
        if(hit){       
            hit.transform.SendMessageUpwards("OnBulletHit", gameObject.name);
            Destroy(gameObject); 
        }

        //Object Lifetime:
        if(bulletLifeTimer >= bulletLifeTime){
            Destroy(gameObject);
        }else{
            bulletLifeTimer += Time.deltaTime;
        }

        
    }
}
