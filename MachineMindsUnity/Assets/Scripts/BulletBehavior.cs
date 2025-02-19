using UnityEngine;

public class BulletBehavior : MonoBehaviour{
    private Rigidbody2D rb;

    // -Gabe: Maybe nerf or allow the GameObject set this value, we could have different enemy types
    // that have different bullet speeds.
    public float bulletSpeed = 1f; 
    public float bulletLifeTime = 10f; // -Gabe: Still think that this should be a bounce count instead of lifetime.
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

    void BounceBullet(RaycastHit2D hit, Vector2 moveDirection) {
        transform.position = hit.point + hit.normal * 0.05f; // Move slightly away
        Vector2 newDirection = Vector2.Reflect(moveDirection, hit.normal);
        float angle = Mathf.Atan2(newDirection.y, newDirection.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        rb.linearVelocity = newDirection * bulletSpeed; // Apply new velocity
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update() {

    }  
    
    // Use FixedUpdate for physics calculations
    void FixedUpdate()
    {
        Vector2 moveDirection = transform.up;
        float moveDistance = bulletSpeed * Time.fixedDeltaTime;

        // Raycast ahead to check if a collision will happen before moving
        RaycastHit2D hit = Physics2D.Raycast(transform.position, moveDirection, moveDistance);
        if (hit)
        {
            hit.transform.SendMessageUpwards("OnBulletHit", gameObject);
            
            if(isExplody){
                Collider2D[] allExplodedObjects = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

                foreach(Collider2D currentExplodedObject in allExplodedObjects){
                    currentExplodedObject.transform.SendMessageUpwards("OnExplosionHit");
                }
            }
            if (isBouncy)
            {
                BounceBullet(hit, moveDirection);
            }
            else
            {
                Destroy(gameObject);
            }
            
        }
        else
        {
            rb.linearVelocity = moveDirection * bulletSpeed; // Normal movement
        }

        // Bullet Lifetime
        if (bulletLifeTimer >= bulletLifeTime)
        {
            Destroy(gameObject);
        }
        else
        {
            bulletLifeTimer += Time.deltaTime;
        }
    }


}
