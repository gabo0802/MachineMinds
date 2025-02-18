using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    private Rigidbody2D rb;
    public float bulletSpeed = 1f;
    public float bulletBounceAngle = 45f; //do we really want to have bouncy bullets?
    public float bulletLifeTime = 10f;
    private float bulletLifeTimer = 0f;
    public float bulletDetectRange = 0.25f;

    void OnBulletHit(GameObject bullet){
        //Destroy(bullet);
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
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, bulletDetectRange); //in theory should work, if not need to change where looking (might be transform.up)
        if(hit){       
            hit.transform.SendMessageUpwards("OnBulletHit", gameObject);
            transform.Rotate(new Vector3(0, 0, bulletBounceAngle));
        }

        //Object Lifetime:
        if(bulletLifeTimer >= bulletLifeTime){
            Destroy(gameObject);
        }else{
            bulletLifeTimer += Time.deltaTime;
        }

        
    }
}
