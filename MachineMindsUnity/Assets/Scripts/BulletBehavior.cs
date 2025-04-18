using UnityEngine;
using Pathfinding;

/// <summary>
/// Handles bullet movement, collision responses (bouncing or exploding),
/// lifetime management, and optional homing behavior via AIPath.
/// </summary>
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

    /// <summary>
    /// Adjusts sound effect volume based on player preferences.
    /// </summary>
    private void volumeAdjustments()
    {
        if (PlayerPrefs.HasKey("SoundEffectVolume"))
        {
            bounceBounceSoundPlayer.volume = PlayerPrefs.GetFloat("SoundEffectVolume");
        }
    }

    /// <summary>
    /// Assigns the target player for homing and caches the AIPath component.
    /// </summary>
    public void SetTarget(GameObject newTarget)
    {
        targetPlayer = newTarget;
        pathFinder = GetComponent<AIPath>();
    }

    /// <summary>
    /// Handles collision logic when bullet hits another bullet or object.
    /// If explosive, creates explosion effect and notifies nearby objects.
    /// Then destroys this bullet.
    /// </summary>
    void OnBulletHit(GameObject bullet)
    {
        if (bullet) Destroy(bullet);

        if (isExplody)
        {
            Collider2D[] allExplodedObjects = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
            GameObject expl = Instantiate(explosionObject, transform.position, transform.rotation);
            expl.SendMessageUpwards("setExplosionMaxRadius", explosionRadius * worldScale);
            foreach (var col in allExplodedObjects)
            {
                col.transform.SendMessageUpwards("OnExplosionHit");
            }
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// Receives explosion messages from other objects and logs the hit.
    /// </summary>
    void OnExplosionHit()
    {
        Debug.Log(gameObject.name + " got hit by explosion");
    }

    /// <summary>
    /// Calculates bounce reflection, repositions bullet, applies new velocity,
    /// plays bounce sound, and decrements bounce count.
    /// </summary>
    void BounceBullet(RaycastHit2D hit, Vector2 moveDirection)
    {
        transform.position = hit.point + hit.normal * 0.05f;
        Vector2 newDir = Vector2.Reflect(moveDirection, hit.normal);
        float angle = Mathf.Atan2(newDir.y, newDir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        rb.velocity = newDir * bulletSpeed;
        if (bounceBounceSoundPlayer)
        {
            volumeAdjustments();
            bounceBounceSoundPlayer.Play();
        }
        bounceCap--;
    }

    /// <summary>
    /// Initializes components and default bullet diameter.
    /// </summary>
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pathFinder = GetComponent<AIPath>();

        if (bulletDiameter == 0f)
            bulletDiameter = transform.localScale.x;
    }

    /// <summary>
    /// Empty Update stub (no per-frame logic here).
    /// </summary>
    void Update() { }

    /// <summary>
    /// Physics update: moves or homing, detects collisions via CircleCast,
    /// handles bounce or destroy logic, and tracks lifetime expiry.
    /// </summary>
    void FixedUpdate()
    {
        Vector2 moveDir = transform.up;
        float dist = bulletSpeed * Time.fixedDeltaTime;
        int mask = ~LayerMask.GetMask("InteractableGround");
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Bullets"), LayerMask.NameToLayer("InteractableGround"), true);

        RaycastHit2D hit = Physics2D.CircleCast(
            transform.position + transform.up * (bulletDiameter + 0.1f),
            bulletDiameter / 2,
            moveDir,
            dist,
            mask);

        if (hit)
        {
            hit.transform.SendMessageUpwards("OnBulletHit", gameObject);
            if (isExplody)
            {
                // Explosion repeat logic for detection radius
                Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
                GameObject expl = Instantiate(explosionObject, transform.position, transform.rotation);
                expl.SendMessageUpwards("setExplosionMaxRadius", explosionRadius * worldScale);
                foreach (var col in cols)
                    col.transform.SendMessageUpwards("OnExplosionHit");
            }

            if (isBouncy)
                BounceBullet(hit, moveDir);
            else
                Destroy(gameObject);
        }
        else
        {
            if (!pathFinder)
                rb.velocity = moveDir * bulletSpeed;
            else
            {
                pathFinder.maxSpeed = bulletSpeed;
                pathFinder.destination = targetPlayer.transform.position;
            }
        }

        // Destroy bullet if over lifetime or out of bounces
        if (bulletLifeTimer >= bulletLifeTime || bounceCap <= 0)
            Destroy(gameObject);
        else
            bulletLifeTimer += Time.deltaTime;
    }
}
