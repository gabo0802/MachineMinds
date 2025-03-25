using System;
using UnityEngine;
using Pathfinding;

public class EnemyBehavior : MonoBehaviour
{
    private const float difficultyScale = 1.10f;
    private Rigidbody2D rb;
    int layerMask;

    private AIPath path;

    private Vector2 patrolDestination;
    private Vector2 lastPosition;
    private float stuckCheckTimer = 0f;
    private float stuckCheckInterval = 0.5f;

    public GameObject cannonHead;
    private float turretRotationSpeedMultiplier = 1f;

    public GameObject tireThreads;
    public float tireThreadCreateInterval = 0.5f;
    private float tireThreadCreateTimer = 0f;

    public int maxEnemyHealth; //# bullets they can survive
    private int currentEnemyHealth;

    public float enemyMoveSpeed = 1f;
    private float enemyMoveSpeedMultiplier = 1f;

    public GameObject enemyBullet;
    public float enemyShootInterval = 5f;
    private float enemyShootTimer = 0f;
    private float bulletShotSpawnOffset;
    public bool shootIfCannotSeePlayer = false;

    public float pointsWorth = 100;
    private int currentDifficulty = 1;
    private GameObject levelManager;

    private GameObject currentAlivePlayer;
    private const string playerName = "player";

    private GameObject currentTarget;
    public bool isBoss = false;

    public void AffectSpeed(float newMultiplier)
    {
        enemyMoveSpeedMultiplier = newMultiplier;
    }

    public void SetGameObjects(GameObject[] parameters)
    {
        levelManager = parameters[0];
        currentAlivePlayer = parameters[1];
    }

    public void SetDifficultyLevel(int newDifficultyLevel)
    {
        currentDifficulty = newDifficultyLevel;
        maxEnemyHealth = (int)(maxEnemyHealth * Mathf.Pow(difficultyScale, currentDifficulty - 1));
        currentEnemyHealth = maxEnemyHealth;
        Debug.Log("Current Health Difficulty: " + currentEnemyHealth);
    }

    void OnExplosionHit()
    {
        Debug.Log(gameObject.name + " got hit be explosion");

        currentEnemyHealth -= 1; //could make explosions instant kills?

        if (currentEnemyHealth <= 0)
        {
            if (levelManager)
            {
                levelManager.transform.SendMessage("OnEnemyDeath", pointsWorth);
            }
            Destroy(gameObject);
        }
    }

    void OnBulletHit(GameObject bullet)
    {
        if (!currentTarget)
        {
            if (enemyMoveSpeed > 0f)
            {
                patrolDestination = (Vector2)currentAlivePlayer.transform.position;
            }
            else
            {
                currentTarget = currentAlivePlayer;
            }
        }

        if (bullet)
        {
            Destroy(bullet);
        }

        currentEnemyHealth -= 1;
        if (isBoss)
        {
            levelManager.transform.SendMessage("updateBossHealhBar", new int[] { currentEnemyHealth, maxEnemyHealth });
        }

        if (currentEnemyHealth <= 0)
        {
            if (levelManager)
            {
                levelManager.transform.SendMessage("OnEnemyDeath", pointsWorth);
            }
            Destroy(gameObject);
        }
    }

    private void PathFindingStuckFix(bool isPatrolling)
    {
        if (stuckCheckTimer <= 0)
        {
            Vector2 latestPosition = (Vector2)transform.position;
            stuckCheckTimer = stuckCheckInterval;

            if (Vector2.Distance(latestPosition, lastPosition) <= 0.01f)
            {
                if (isPatrolling)
                {
                    patrolDestination = new Vector2(UnityEngine.Random.Range(-21, -4), UnityEngine.Random.Range(-3, 4));
                }
                else
                {
                    Debug.Log("Stuck");
                    transform.Rotate(0, 0, 180f);
                    //rb.AddForce(-transform.up * 5000f);
                }
            }

            lastPosition = latestPosition;
        }

        stuckCheckTimer -= Time.deltaTime;
    }


    private float currentAngle;
    private float turnTimer;

    private void PatrolBehavior()
    {
        float rotationSpeed = 60f * turretRotationSpeedMultiplier;
        cannonHead.transform.localEulerAngles = new Vector3(0, 0, Mathf.PingPong(Time.time * rotationSpeed, 160) - 80); //(-80, 80)

        RaycastHit2D lookForPlayerRay = Physics2D.Raycast(cannonHead.transform.position + (cannonHead.transform.up * bulletShotSpawnOffset), cannonHead.transform.up, Mathf.Infinity, layerMask);
        RaycastHit2D lookForPlayerRay2 = Physics2D.Raycast(transform.position + (transform.up * bulletShotSpawnOffset), transform.up, Mathf.Infinity, layerMask);

        Debug.DrawLine(cannonHead.transform.position + (cannonHead.transform.up * bulletShotSpawnOffset), cannonHead.transform.position + (cannonHead.transform.up * 100f), Color.white);
        Debug.DrawLine(transform.position, transform.position + (transform.up * 100f), Color.blue);

        if (lookForPlayerRay && lookForPlayerRay.transform.gameObject.name.Equals(currentAlivePlayer.transform.gameObject.name))
        {
            currentTarget = currentAlivePlayer;
            cannonHead.transform.localEulerAngles = new Vector3(0, 0, 0);
        }
        else if (lookForPlayerRay2 && lookForPlayerRay2.transform.gameObject.name.Equals(currentAlivePlayer.transform.gameObject.name))
        {
            currentTarget = currentAlivePlayer;
            cannonHead.transform.localEulerAngles = new Vector3(0, 0, 0);
        }

        if (!currentTarget && enemyMoveSpeed > 0f)
        {
            path.maxSpeed = enemyMoveSpeed * enemyMoveSpeedMultiplier * Mathf.Pow(difficultyScale, currentDifficulty - 1);
            if (Vector2.Distance(transform.position, patrolDestination) <= 2f)
            {
                patrolDestination = new Vector2(UnityEngine.Random.Range(-21, -4), UnityEngine.Random.Range(-3, 4));
            }
            PathFindingStuckFix(true);
            path.destination = patrolDestination;
        }
    }

    private void TargetPlayerBehavior()
    {
        cannonHead.transform.up = currentTarget.transform.position - transform.position;
        cannonHead.transform.rotation = Quaternion.Euler(new Vector3(0, 0, cannonHead.transform.eulerAngles.z));

        RaycastHit2D scanAhead = Physics2D.Raycast(cannonHead.transform.position + (cannonHead.transform.up * bulletShotSpawnOffset), cannonHead.transform.up, Mathf.Infinity, layerMask);

        if (scanAhead && scanAhead.transform.gameObject.name.ToLower().Contains(playerName))
        {
            if (enemyMoveSpeed > 0f)
            {
                path.maxSpeed = 0.01f;
            }
        }
        else
        {
            if (enemyMoveSpeed > 0f)
            {
                path.maxSpeed = enemyMoveSpeed * enemyMoveSpeedMultiplier * Mathf.Pow(difficultyScale, currentDifficulty - 1);
                path.destination = currentTarget.transform.position;
            }

            PathFindingStuckFix(false);
        }

        if (enemyShootTimer >= enemyShootInterval / Mathf.Pow(difficultyScale, currentDifficulty - 1))
        {
            //Debug.Log(hit.transform.gameObject.name);
            if (shootIfCannotSeePlayer || (scanAhead && scanAhead.transform.gameObject.name.ToLower().Contains(playerName)))
            {
                GameObject currentBullet = currentBullet = (GameObject)Instantiate(enemyBullet, cannonHead.transform.position + (cannonHead.transform.up * bulletShotSpawnOffset), cannonHead.transform.rotation);
                currentBullet.SendMessageUpwards("SetTarget", currentAlivePlayer);

                Debug.DrawLine(cannonHead.transform.position + (cannonHead.transform.up * bulletShotSpawnOffset),
                cannonHead.transform.position + (cannonHead.transform.up * bulletShotSpawnOffset) + (cannonHead.transform.up * scanAhead.distance),
                Color.green, enemyShootInterval / 2);
                enemyShootTimer = 0f;
            }
            else if (scanAhead)
            {
                Debug.Log(scanAhead.transform.gameObject.name);
                Debug.DrawLine(cannonHead.transform.position + (cannonHead.transform.up * bulletShotSpawnOffset),
                cannonHead.transform.position + (cannonHead.transform.up * bulletShotSpawnOffset) + (cannonHead.transform.up * scanAhead.distance),
                Color.red, enemyShootInterval / 2);
                enemyShootTimer = enemyShootInterval / (currentDifficulty * 2f);
            }
            else
            {
                Debug.DrawLine(cannonHead.transform.position + (cannonHead.transform.up * bulletShotSpawnOffset),
                cannonHead.transform.position + (cannonHead.transform.up * bulletShotSpawnOffset) + (cannonHead.transform.up * 100f),
                Color.white, enemyShootInterval / 2);
                enemyShootTimer = enemyShootInterval / (currentDifficulty * 2f);
            }
        }
        else
        {
            enemyShootTimer += Time.deltaTime;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        layerMask = ~LayerMask.GetMask("InteractableGround"); // Ignores "NoBounce" layer

        if (currentDifficulty < 1)
        {
            currentDifficulty = 1;
        }

        if (maxEnemyHealth < 1)
        {
            maxEnemyHealth = 1;
        }

        bulletShotSpawnOffset = (transform.localScale.magnitude / 2) + 0.1f;
        maxEnemyHealth = (int)(maxEnemyHealth * Mathf.Pow(difficultyScale, currentDifficulty - 1));
        currentEnemyHealth = maxEnemyHealth;
        Debug.Log("Current Health: " + currentEnemyHealth);
        rb = GetComponent<Rigidbody2D>();
        path = GetComponent<AIPath>();

        lastPosition = (Vector2)transform.position;
        patrolDestination = (Vector2)transform.position;
    }


    // Update is called once per frame
    void Update()
    {
        if (isBoss)
        {
            if (levelManager)
            {
                levelManager.transform.SendMessage("updateBossHealhBar", new int[] { currentEnemyHealth, maxEnemyHealth });
            }
        }

        if (tireThreads)
        {
            if (path.maxSpeed > 0.01f && tireThreadCreateTimer < 0)
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
            //Player is Dead
            path.maxSpeed = 0f;
            path.destination = transform.position;
            transform.Rotate(0, 0, 1f);

        }
        else if (!currentTarget)
        {
            //Patrol Behavior
            PatrolBehavior();
        }
        else
        {
            //Target Player Behavior
            TargetPlayerBehavior();
        }
    }

}
