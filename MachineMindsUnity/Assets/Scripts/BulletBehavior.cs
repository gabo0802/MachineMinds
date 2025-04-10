using UnityEngine;
using Pathfinding;

public class BulletBehavior : MonoBehaviour
{
    private Rigidbody2D rb;

    public float bulletSpeed = 1f;
    public float bulletLifeTime = 10f;
    private float bulletLifeTimer = 0f;

    public int bounceCap = 2;
    public float bulletDetectRange = 0.25f;

    public bool isBouncy = true;

    public bool isExplody = false;
    private float worldScale = 2.0f;
    public float explosionRadius = 20f;
    public GameObject explosionObject;

    private AIPath pathFinder;
    public GameObject targetPlayer;

    public float bulletDiameter = 0f;

    public AudioSource bounceBounceSoundPlayer;

    public void SetTarget(GameObject newTarget)
    {
        targetPlayer = newTarget;
        pathFinder = GetComponent<AIPath>();
    }


    void OnBulletHit(GameObject bullet)
    {
        if (bullet)
        {
            Destroy(bullet);
        }

        if (isExplody)
        {
            Destroy(bullet);

            Collider2D[] allExplodedObjects = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
            
            GameObject currentExplosionObject = (GameObject)Instantiate(explosionObject, transform.position, transform.rotation);
            currentExplosionObject.SendMessageUpwards("setExplosionMaxRadius", explosionRadius * worldScale);

            foreach (Collider2D currentExplodedObject in allExplodedObjects)
            {
                currentExplodedObject.transform.SendMessageUpwards("OnExplosionHit");
            }
        }

        Destroy(gameObject);
    }

    void OnExplosionHit()
    {
        Debug.Log(gameObject.name + " got hit be explosion");
    }

    void BounceBullet(RaycastHit2D hit, Vector2 moveDirection)
    {
        transform.position = hit.point + hit.normal * 0.05f; // Move slightly away
        Vector2 newDirection = Vector2.Reflect(moveDirection, hit.normal);
        float angle = Mathf.Atan2(newDirection.y, newDirection.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        rb.linearVelocity = newDirection * bulletSpeed; // Apply new velocity
        if(bounceBounceSoundPlayer){
            bounceBounceSoundPlayer.Play();
        }
        bounceCap--;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pathFinder = GetComponent<AIPath>();

        if (bulletDiameter == 0f)
        {
            bulletDiameter = transform.localScale.x;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Use FixedUpdate for physics calculations
    void FixedUpdate()
    {
        Vector2 moveDirection = transform.up;
        float moveDistance = bulletSpeed * Time.fixedDeltaTime;

        // Raycast ahead to check if a collision will happen before moving
        int layerMask = ~LayerMask.GetMask("InteractableGround"); // Ignores "NoBounce" layer

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Bullets"), LayerMask.NameToLayer("InteractableGround"), true);

        float bulletOffset = 0.1f;

        RaycastHit2D hit = Physics2D.CircleCast(
            transform.position + (transform.up * (bulletDiameter + bulletOffset)),
            (bulletDiameter / 2),
            moveDirection,
            moveDistance,
            layerMask);

        Debug.DrawLine(transform.position + (transform.up * (bulletDiameter + bulletOffset)),
        transform.position + (transform.up * moveDistance), Color.green);


        if (hit)
        {
            hit.transform.SendMessageUpwards("OnBulletHit", gameObject);

            if (isExplody)
            {
                Collider2D[] allExplodedObjects = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
                GameObject currentExplosionObject = (GameObject)Instantiate(explosionObject, transform.position, transform.rotation);
                currentExplosionObject.SendMessageUpwards("setExplosionMaxRadius", explosionRadius * worldScale);

                Debug.DrawLine(transform.position - new Vector3(explosionRadius, 0, 0), transform.position + new Vector3(explosionRadius, 0, 0), Color.red, 2.5f);
                Debug.DrawLine(transform.position - new Vector3(0, explosionRadius, 0), transform.position + new Vector3(0, explosionRadius, 0), Color.red, 2.5f);


                foreach (Collider2D currentExplodedObject in allExplodedObjects)
                {
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
            if (!pathFinder)
            {
                rb.linearVelocity = moveDirection * bulletSpeed; // Normal movement
            }
            else
            {
                pathFinder.maxSpeed = bulletSpeed;
                pathFinder.destination = targetPlayer.transform.position;
            }
        }

        // Bullet Lifetime
        if (bulletLifeTimer >= bulletLifeTime || bounceCap <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            bulletLifeTimer += Time.deltaTime;
        }
    }


}
