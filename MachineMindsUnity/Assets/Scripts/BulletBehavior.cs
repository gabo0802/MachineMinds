using UnityEngine;

public class BulletBehavior : MonoBehaviour{
    private Rigidbody2D rb;
    public float bulletSpeed = 1f;
    public float bulletBounceAngle = 45f; //do we really want to have bouncy bullets?
    public float bulletLifeTime = 10f;
    private float bulletLifeTimer = 0f;
    public float bulletDetectRange = 0.25f;
    
    public bool isBouncy = true;
    
    public bool isExplody = false;
    public float explosionRadius = 20f;

    void OnBulletHit(GameObject bullet){
        /*if(bullet){
            Destroy(bullet);
        }*/
    }

    void OnExplosionHit(){
        Debug.Log(gameObject.name + " got hit be explosion");
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
        RaycastHit2D hit = Physics2D.Raycast(transform.position + ((bulletDetectRange / 2) * transform.up), transform.up, (bulletDetectRange / 2)); //in theory should work, if not need to change where looking (might be transform.up)
        if(hit){       
            hit.transform.SendMessageUpwards("OnBulletHit", gameObject);
            
            if(isExplody){
                Collider2D[] allExplodedObjects = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

                foreach(Collider2D currentExplodedObject in allExplodedObjects){
                    currentExplodedObject.transform.SendMessageUpwards("OnExplosionHit");
                }
            }

            if(isBouncy){
                transform.Rotate(new Vector3(0, 0, bulletBounceAngle));
            }else{
                Destroy(gameObject);
            }
        }

        //Object Lifetime:
        if(bulletLifeTimer >= bulletLifeTime){
            Destroy(gameObject);
        }else{
            bulletLifeTimer += Time.deltaTime;
        }

        
    }
}
