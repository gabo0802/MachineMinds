using UnityEngine;

/// <summary>
/// Controls a visual flying missile effect: scales, rotates, and moves toward a target,
/// spawns reticle, and triggers an explosion on arrival.
/// </summary>
public class FlyingMissileEffectScript : MonoBehaviour
{
    public Vector3 targetPosition = Vector3.zero;
    private float totalDistanceToTarget;
    private float currentDistanceFromTarget;

    public GameObject missileObject;
    public GameObject targetReticle;
    private GameObject currentReticle;
    public GameObject explosionObject;
    public float explosionRadius = 1f;

    private float worldScale = 2.0f;
    private float angleChangeScale = 10f;
    private float sizeChangeScale = 0.02f;
    private float speedScale = 0.05f;

    /// <summary>
    /// Sets the missile's target position, instantiates reticle,
    /// and initializes scaling factors based on distance.
    /// </summary>
    void setTargetPosition(Vector3 newPosition)
    {
        targetPosition = newPosition;
        currentReticle = Instantiate(targetReticle, targetPosition, Quaternion.identity);
        totalDistanceToTarget = Vector3.Distance(transform.position, targetPosition);
        transform.up = targetPosition - transform.position;
        angleChangeScale /= totalDistanceToTarget;
        sizeChangeScale /= totalDistanceToTarget;
    }

    /// <summary>
    /// Adjusts the speed scaling factor for missile movement.
    /// </summary>
    void setSpeedScale(float newSpeed)
    {
        speedScale = newSpeed;
    }

    /// <summary>
    /// Adjusts the angle change speed, factoring in distance.
    /// </summary>
    void setAngleChangeSpeed(float newSpeed)
    {
        angleChangeScale = newSpeed / totalDistanceToTarget;
    }

    /// <summary>
    /// Unity Start (unused placeholder for potential initialization).
    /// </summary>
    void Start()
    {
        // Initialization handled in setTargetPosition
    }

    /// <summary>
    /// Called when missile reaches its path end: spawns explosion,
    /// notifies nearby objects, and cleans up reticle and missile.
    /// </summary>
    void EndOfPath()
    {
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        GameObject explosion = Instantiate(explosionObject, transform.position, transform.rotation);
        explosion.transform.localScale = new Vector3(explosionRadius * worldScale,
                                                    explosionRadius * worldScale,
                                                    1);
        foreach (Collider2D col in hitObjects)
        {
            col.transform.SendMessageUpwards("OnExplosionHit");
        }
    }

    /// <summary>
    /// Receives explosion hit messages (no action required here).
    /// </summary>
    void OnExplosionHit() { }

    /// <summary>
    /// Updates missile scaling, rotation, movement each frame,
    /// and triggers end-of-path logic when target reached.
    /// </summary>
    void Update()
    {
        currentDistanceFromTarget = Vector3.Distance(transform.position, targetPosition);

        float sizeDelta = sizeChangeScale * Time.deltaTime * 500f;
        float speedDelta = speedScale * Time.deltaTime * 500f;
        float angleDelta = angleChangeScale * Time.deltaTime * 500f;

        if (currentDistanceFromTarget > totalDistanceToTarget / 2)
        {
            missileObject.transform.localScale += Vector3.one * sizeDelta;
            transform.position += transform.up * speedDelta;
        }
        else if (currentDistanceFromTarget > 0.1f)
        {
            missileObject.transform.Rotate(angleDelta, 0, 0);
            missileObject.transform.localScale -= Vector3.one * sizeDelta;
            transform.position += transform.up * speedDelta;
        }
        else
        {
            EndOfPath();
            Destroy(currentReticle);
            Destroy(gameObject);
        }
    }
}
