using System;
using UnityEngine;
using Pathfinding;
using UnityEngine.Rendering;

/// <summary>
/// Governs enemy AI: patrol and targeting behaviors, health scaling,
/// shooting mechanics, and hit/explosion responses based on difficulty.
/// </summary>
public class EnemyBehavior : MonoBehaviour
{
    private const float difficultyScaleHealth = 1.15f;
    private const float difficultyScaleFireRate = 1.12f;

    private Rigidbody2D rb;
    private AIPath path;
    private int layerMask;

    private Vector2 patrolDestination;
    private Vector2 lastPosition;
    private float stuckCheckTimer = 0f;
    private float stuckCheckInterval = 0.5f;

    public GameObject cannonHead;
    public GameObject tireThreads;
    public float tireThreadCreateInterval = 0.5f;
    private float tireThreadCreateTimer = 0f;

    public int maxEnemyHealth;
    private int defaultEnemyHealth;
    private int currentEnemyHealth;

    public float enemyMoveSpeed = 1f;
    private float enemyMoveSpeedMultiplier = 1f;

    public GameObject enemyBullet;
    public float enemyShootInterval = 5f;
    private float enemyShootTimer = 0f;
    public float bulletShotSpawnOffset;
    public bool shootIfCannotSeePlayer = false;

    public float pointsWorth = 100;
    public int currentDifficulty = 1;
    private GameObject levelManager;
    private GameObject currentAlivePlayer;
    private const string playerName = "player";

    public GameObject currentTarget;
    public bool isBoss = false;

    public GameObject[] enemyHealthBarComponents;
    public AudioSource enemyShootSoundPlayer;
    public AudioSource enemyHitSoundPlayer;
    public GameObject enemyDeathObject;
    public GameObject enemyDetectPlayerEffectObject;
    public int stealthBonusDamage = 2;
    public int redirectBonusDamage = 4;

    /// <summary>
    /// Applies saved sound effect volume to shooting and hit audio sources.
    /// </summary>
    private void volumeAdjustments()
    {
        if (enemyShootSoundPlayer && PlayerPrefs.HasKey("SoundEffectVolume"))
            enemyShootSoundPlayer.volume = PlayerPrefs.GetFloat("SoundEffectVolume");

        if (enemyHitSoundPlayer && PlayerPrefs.HasKey("SoundEffectVolume"))
            enemyHitSoundPlayer.volume = PlayerPrefs.GetFloat("SoundEffectVolume");
    }

    /// <summary>
    /// Instantiates a detect-player effect when the enemy spots the player.
    /// </summary>
    private void onDetectPlayer()
    {
        if (enemyDetectPlayerEffectObject)
        {
            var detectObj = Instantiate(
                enemyDetectPlayerEffectObject,
                enemyHealthBarComponents[1].transform.position,
                enemyHealthBarComponents[1].transform.rotation);
            detectObj.transform.SetParent(enemyHealthBarComponents[1].transform);
        }
    }

    /// <summary>
    /// Modifies movement speed multiplier (e.g., for slow or boost effects).
    /// </summary>
    public void AffectSpeed(float newMultiplier)
    {
        enemyMoveSpeedMultiplier = newMultiplier;
    }

    /// <summary>
    /// Receives references for level manager and player to coordinate events.
    /// </summary>
    public void SetGameObjects(GameObject[] parameters)
    {
        levelManager = parameters[0];
        currentAlivePlayer = parameters[1];
    }

    /// <summary>
    /// Scales max health according to difficulty level.
    /// </summary>
    public void SetDifficultyLevel(int newDifficultyLevel)
    {
        currentDifficulty = newDifficultyLevel;
        if (maxEnemyHealth == defaultEnemyHealth)
        {
            maxEnemyHealth = (int)(maxEnemyHealth * MathF.Pow(difficultyScaleHealth, currentDifficulty - 1));
            currentEnemyHealth = maxEnemyHealth;
        }
        Debug.Log("Current Health Difficulty: " + currentEnemyHealth);
    }

    /// <summary>
    /// Handles being hit by an explosion: reduces health and triggers death logic.
    /// </summary>
    void OnExplosionHit()
    {
        currentEnemyHealth -= 1;
        if (currentEnemyHealth <= 0)
        {
            levelManager?.transform.SendMessage("OnEnemyDeath", pointsWorth);
            if (enemyDeathObject)
            {
                var deathObj = Instantiate(enemyDeathObject, transform.position, transform.rotation);
                deathObj.SendMessageUpwards("setExplosionMaxRadius", transform.localScale.magnitude * 2f);
            }
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Processes bullet hits: applies damage, plays sounds, updates health bar, and handles death.
    /// </summary>
    void OnBulletHit(GameObject bullet)
    {
        if (bullet)
        {
            if (!bullet.name.ToLower().Contains("player"))
                currentEnemyHealth -= redirectBonusDamage;
            Destroy(bullet);
        }

        if (!currentTarget)
        {
            patrolDestination = enemyMoveSpeed > 0f ?
                (Vector2)currentAlivePlayer.transform.position : patrolDestination;
            onDetectPlayer();
            currentEnemyHealth -= stealthBonusDamage;
            enemyHitSoundPlayer.pitch = 1.2f;
        }
        else
        {
            currentEnemyHealth -= 1;
            enemyHitSoundPlayer.pitch = 1f;
        }

        enemyHitSoundPlayer?.Play();
        UpdateHealthBar();

        if (currentEnemyHealth <= 0)
        {
            levelManager?.transform.SendMessage("OnEnemyDeath", pointsWorth);
            if (enemyDeathObject)
            {
                var deathObj = Instantiate(enemyDeathObject, transform.position, transform.rotation);
                deathObj.SendMessageUpwards("setExplosionMaxRadius", transform.localScale.magnitude * 2f);
            }
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Adjusts the visual health bar based on current health ratio.
    /// </summary>
    private void UpdateHealthBar()
    {
        if (isBoss)
        {
            levelManager.transform.SendMessage("updateBossHealhBar", new int[] { currentEnemyHealth, maxEnemyHealth });
        }
        else if (enemyHealthBarComponents.Length == 3)
        {
            float ratio = (float)currentEnemyHealth / maxEnemyHealth;
            var bar = enemyHealthBarComponents[0].transform;
            bar.localScale = new Vector3(1.5f * ratio, bar.localScale.y, bar.localScale.z);
            bar.localRotation = Quaternion.identity;
            enemyHealthBarComponents[1].transform.localRotation = Quaternion.identity;
            bar.localPosition = new Vector3((1.5f / (maxEnemyHealth * 2f)) * (currentEnemyHealth - maxEnemyHealth),
                                           bar.localPosition.y,
                                           bar.localPosition.z);
            enemyHealthBarComponents[0].GetComponent<SpriteRenderer>().color = Color.green;
            enemyHealthBarComponents[1].GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    /// <summary>
    /// Prevents AI pathfinding from getting stuck by rotating or choosing new patrol points.
    /// </summary>
    private void PathFindingStuckFix(bool isPatrolling)
    {
        if (stuckCheckTimer <= 0)
        {
            var latestPosition = (Vector2)transform.position;
            stuckCheckTimer = stuckCheckInterval;
            if (Vector2.Distance(latestPosition, lastPosition) <= 0.01f)
            {
                if (isPatrolling)
                    patrolDestination = new Vector2(UnityEngine.Random.Range(-21, -4), UnityEngine.Random.Range(-3, 4));
                else
                    transform.Rotate(0, 0, 180f);
            }
            lastPosition = latestPosition;
        }
        stuckCheckTimer -= Time.deltaTime;
    }

    /// <summary>
    /// Executes turret rotation and patrol movement, transitioning to player detection.
    /// </summary>
    private void PatrolBehavior()
    {
        float speed = 60f * turretRotationSpeedMultiplier;
        cannonHead.transform.localEulerAngles = new Vector3(0, 0, Mathf.PingPong(Time.time * speed, 160) - 80);
        var ray = Physics2D.Raycast(cannonHead.transform.position + cannonHead.transform.up * bulletShotSpawnOffset,
                                    cannonHead.transform.up, Mathf.Infinity, layerMask);
        if (ray && ray.transform.gameObject.name.Equals(currentAlivePlayer.name, StringComparison.OrdinalIgnoreCase))
        {
            currentTarget = currentAlivePlayer;
            onDetectPlayer();
            cannonHead.transform.localEulerAngles = Vector3.zero;
        }
        if (!currentTarget && enemyMoveSpeed > 0f)
        {
            path.maxSpeed = enemyMoveSpeed * enemyMoveSpeedMultiplier;
            if (Vector2.Distance(transform.position, patrolDestination) <= 2f)
                patrolDestination = new Vector2(UnityEngine.Random.Range(-21, -4), UnityEngine.Random.Range(-3, 4));
            PathFindingStuckFix(true);
            path.destination = patrolDestination;
        }
    }

    /// <summary>
    /// Rotates turret toward player, manages slowing/slippery mechanics, and handles shooting logic.
    /// </summary>
    private void TargetPlayerBehavior()
    {
        cannonHead.transform.up = currentTarget.transform.position - transform.position;
        cannonHead.transform.rotation = Quaternion.Euler(0, 0, cannonHead.transform.eulerAngles.z);
        var scan = Physics2D.Raycast(cannonHead.transform.position + cannonHead.transform.up * bulletShotSpawnOffset,
                                     cannonHead.transform.up, Mathf.Infinity, layerMask);
        var canSee = scan && scan.transform.gameObject.name.ToLower().Contains(playerName);
        path.maxSpeed = canSee ? 0.01f : enemyMoveSpeed * enemyMoveSpeedMultiplier;
        if (!canSee || shootIfCannotSeePlayer)
            path.destination = currentTarget.transform.position;
        PathFindingStuckFix(false);

        if (enemyShootTimer >= enemyShootInterval / MathF.Pow(difficultyScaleFireRate, currentDifficulty - 1))
        {
            if (canSee || shootIfCannotSeePlayer)
            {
                if (enemyBullet)
                {
                    var bullet = Instantiate(enemyBullet,
                                              cannonHead.transform.position + cannonHead.transform.up * bulletShotSpawnOffset,
                                              cannonHead.transform.rotation);
                    bullet.SendMessageUpwards("SetTarget", currentAlivePlayer);
                    enemyShootTimer = 0f;
                    enemyShootSoundPlayer.Play();
                }
            }
            else
            {
                enemyShootTimer = enemyShootInterval / (currentDifficulty * 2f);
            }
        }
        else
        {
            enemyShootTimer += Time.deltaTime;
        }
    }

    /// <summary>
    /// Unity Start: initializes health, pathfinder, rigidbody, and spawn offsets.
    /// </summary>
    void Start()
    {
        defaultEnemyHealth = maxEnemyHealth;
        layerMask = ~LayerMask.GetMask("InteractableGround");
        currentDifficulty = Math.Max(currentDifficulty, 1);
        maxEnemyHealth = Math.Max(maxEnemyHealth, 1);
        bulletShotSpawnOffset += (transform.localScale.magnitude / 2) + 0.1f;
        maxEnemyHealth = (int)(maxEnemyHealth * MathF.Pow(difficultyScaleHealth, currentDifficulty - 1));
        currentEnemyHealth = maxEnemyHealth;
        rb = GetComponent<Rigidbody2D>();
        path = GetComponent<AIPath>();
        lastPosition = transform.position;
        patrolDestination = transform.position;
    }

    /// <summary>
    /// Unity Update: handles volume, health bar rotation, trail spawning, and selects behavior mode.
    /// </summary>
    void Update()
    {
        volumeAdjustments();
        if (isBoss)
            levelManager?.transform.SendMessage("updateBossHealhBar", new int[] { currentEnemyHealth, maxEnemyHealth });

        if (tireThreads)
        {
            if (path.maxSpeed > 0.01f && tireThreadCreateTimer <= 0)
            {
                tireThreadCreateTimer = tireThreadCreateInterval;
                Instantiate(tireThreads, transform.position, transform.rotation);
            }
            else
            {
                tireThreadCreateTimer -= Time.deltaTime;
            }
        }

        if (!currentAlivePlayer)
        {
            path.maxSpeed = 0f;
            path.destination = transform.position;
            transform.Rotate(0, 0, 1f);
        }
        else if (!currentTarget)
        {
            PatrolBehavior();
        }
        else
        {
            TargetPlayerBehavior();
        }

        if (enemyHealthBarComponents.Length == 3)
            enemyHealthBarComponents[2].transform.rotation = Quaternion.Euler(0, 0, -transform.rotation.z);
    }
}
