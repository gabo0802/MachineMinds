using System;
using UnityEngine;
using Pathfinding;

public class EnemyBehavior : MonoBehaviour{
    public GameObject cannonHead;
    
    public GameObject tireThreads;
    private float tireThreadRevealTimer = 0f;
    public float tireThreadRevealInterval = 0.5f;

    private Rigidbody2D rb;
    int layerMask;

    private AIPath path;
    private bool canShoot;
    private Vector2 lastPosition;
    private float stuckCheckTimer = 0f;
    private float stuckCheckInterval = 0.5f;

    private GameObject targetPlayer;
    private GameObject currentActualTarget;
    private GameObject levelManager;
    private Vector2 patrolDestination;
    
    public float enemyMoveSpeed = 1f;
    private float enemyMoveSpeedSlowMultiplier = 1f;

    public int maxEnemyHealth; //# bullets they can survive
    private int currentEnemyHealth;

    public GameObject enemyBullet;
    public float bulletShotSpawnOffset = 0.5f;
    public bool shootIfCannotSeePlayer = false;

    public float enemyFireRate = 5f;
    private float enemyFireTimer = 0f;

    //public float distanceToPlayer = 5f;

    public float pointsWorth;

    private const string playerName = "player";

    private int currentDifficulty = 0;

    public void AffectSlowdownSpeed(float newMultiplier){
        enemyMoveSpeedSlowMultiplier = newMultiplier;
    }

    public void SetGameObjects(GameObject[] parameters){
        levelManager = parameters[0];
        targetPlayer = parameters[1];
    }

    public void SetDifficultyLevel(int newDifficultyLevel){
        currentDifficulty = newDifficultyLevel;
    }

    void OnExplosionHit(){
        Debug.Log(gameObject.name + " got hit be explosion");

        currentEnemyHealth -= 1; //could make explosions instant kills?

        if(currentEnemyHealth <= 0){
            if(levelManager){
                levelManager.transform.SendMessageUpwards("OnEnemyDeath", pointsWorth);
            }
            Destroy(gameObject);
        }
    }

    void OnBulletHit(GameObject bullet){
        //string bulletType = bullet.name;
        //Debug.Log("Enemy Bullet Hit" + bulletType);
        if(!currentActualTarget && cannonHead){
            cannonHead.transform.up = bullet.transform.position - transform.position;
        }

        if(bullet){
            Destroy(bullet);
        }

        currentEnemyHealth -= 1;

        if(currentEnemyHealth <= 0){
            if(levelManager){
                levelManager.transform.SendMessageUpwards("OnEnemyDeath", pointsWorth);
            }
            Destroy(gameObject);
        }
    }

    private void PathFindingStuckFix(bool isPatrolling){
        if(stuckCheckTimer <= 0){
            Vector2 latestPosition = (Vector2) transform.position;
            stuckCheckTimer = stuckCheckInterval;

            if(Vector2.Distance(latestPosition, lastPosition) <= 0.01f){
                if(isPatrolling){
                    patrolDestination = new Vector2(UnityEngine.Random.Range(-21, -4), UnityEngine.Random.Range(-3, 4));
                }else{
                    Debug.Log("Stuck");
                    transform.Rotate(0, 0, 180f);
                    //rb.AddForce(-transform.up * 5000f);
                }
            }

            lastPosition = latestPosition;
        }

        stuckCheckTimer -= Time.deltaTime;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        layerMask = ~LayerMask.GetMask("InteractableGround"); // Ignores "NoBounce" layer

        if(currentDifficulty < 1){
            currentDifficulty = 1;
        }

        if(maxEnemyHealth < 1){
            maxEnemyHealth = 1;
        }

        bulletShotSpawnOffset = (transform.localScale.magnitude / 2) + 0.1f;
        currentEnemyHealth = maxEnemyHealth * currentDifficulty;
        rb = GetComponent<Rigidbody2D>();
        path = GetComponent<AIPath>();

        lastPosition = (Vector2) transform.position;
        patrolDestination = (Vector2) transform.position;

    }
    

    // Update is called once per frame
    void Update(){
        if(tireThreads){
            if(path.maxSpeed > 0.01f && tireThreadRevealTimer < 0){
                tireThreadRevealTimer = tireThreadRevealInterval;
                Instantiate(tireThreads, transform.position, transform.rotation);
            }else{
                tireThreadRevealTimer -= Time.deltaTime;
            }
        }
        if(currentActualTarget){
            if(cannonHead){
                cannonHead.transform.up = currentActualTarget.transform.position - transform.position;
            }

            RaycastHit2D scanAhead;
            if(cannonHead){
                scanAhead = Physics2D.Raycast(cannonHead.transform.position + (transform.up * bulletShotSpawnOffset), cannonHead.transform.up, Mathf.Infinity, layerMask);
            }else{
                scanAhead = Physics2D.Raycast(transform.position + (cannonHead.transform.up * bulletShotSpawnOffset), transform.up, Mathf.Infinity, layerMask);
            }

            //Target Player:
            /*transform.up = currentActualTarget.transform.position - transform.position;
            if (Vector2.Distance(transform.position, currentActualTarget.transform.position) > distanceToPlayer){
                rb.linearVelocity = transform.up * enemyMoveSpeed;
            }else{
                rb.linearVelocity = transform.up * 0.000001f;
            }*/

            if (scanAhead && scanAhead.transform.gameObject.name.ToLower().Contains(playerName)){
                path.maxSpeed = 0.01f;
            }else{
                path.maxSpeed = enemyMoveSpeed * currentDifficulty * enemyMoveSpeedSlowMultiplier;
                path.destination = currentActualTarget.transform.position;

                PathFindingStuckFix(false);
            }

            //Shoot Player:
            if(enemyFireTimer >= enemyFireRate / currentDifficulty){                
                //Debug.Log(hit.transform.gameObject.name);
                if(shootIfCannotSeePlayer || (scanAhead && scanAhead.transform.gameObject.name.ToLower().Contains(playerName))){
                    GameObject currentBullet;
                    if(cannonHead){
                        currentBullet = (GameObject) Instantiate(enemyBullet, cannonHead.transform.position + (cannonHead.transform.up * bulletShotSpawnOffset), cannonHead.transform.rotation);
                    }else{
                        currentBullet = (GameObject) Instantiate(enemyBullet, transform.position + (transform.up * bulletShotSpawnOffset), transform.rotation);
                    }
                    currentBullet.SendMessageUpwards("SetTarget", targetPlayer);
                     
                    Debug.DrawLine(cannonHead.transform.position + (cannonHead.transform.up * bulletShotSpawnOffset), cannonHead.transform.position + (cannonHead.transform.up * scanAhead.distance), Color.red, enemyFireRate / 2);
                }else{
                    Debug.DrawLine(cannonHead.transform.position + (cannonHead.transform.up * bulletShotSpawnOffset), cannonHead.transform.position + (cannonHead.transform.up * 100f), Color.white, enemyFireRate / 2);
                }
                
                enemyFireTimer = 0f;
            }else{
                enemyFireTimer += Time.deltaTime;
            }
        }else if(!targetPlayer){
            //Celebrate?
            transform.Rotate(0, 0, 1f);
        }else{
            RaycastHit2D lookForPlayerRay;

            for(int i = 0; i < 4; i++){          
                if(i == 0){
                    lookForPlayerRay = Physics2D.Raycast(cannonHead.transform.position + (transform.up * bulletShotSpawnOffset), cannonHead.transform.up, Mathf.Infinity, layerMask);
                    Debug.DrawLine(cannonHead.transform.position + (cannonHead.transform.up * bulletShotSpawnOffset), cannonHead.transform.position + (cannonHead.transform.up * 100f), Color.white);
                    cannonHead.transform.Rotate(new Vector3(0, 0, 1f));
                }else if(i == 2){
                    lookForPlayerRay = Physics2D.Raycast(transform.position + (cannonHead.transform.up * bulletShotSpawnOffset), transform.up, Mathf.Infinity, layerMask);
                    Debug.DrawLine(transform.position + (transform.up * bulletShotSpawnOffset), transform.position + (transform.up * 100f), Color.blue);
                }else if(i == 3){
                    lookForPlayerRay = Physics2D.Raycast(cannonHead.transform.position - (transform.up * bulletShotSpawnOffset), -cannonHead.transform.up, Mathf.Infinity, layerMask);
                    Debug.DrawLine(cannonHead.transform.position - (cannonHead.transform.up * bulletShotSpawnOffset), cannonHead.transform.position - (cannonHead.transform.up * 100f), Color.white);
                }else{
                    lookForPlayerRay = Physics2D.Raycast(transform.position - (cannonHead.transform.up * bulletShotSpawnOffset), -transform.up, Mathf.Infinity, layerMask);
                    Debug.DrawLine(transform.position - (transform.up * bulletShotSpawnOffset), transform.position - (transform.up * 100f), Color.blue);
                }

                if(lookForPlayerRay && lookForPlayerRay.transform.gameObject.name.Equals(targetPlayer.transform.gameObject.name)){
                    currentActualTarget = targetPlayer;
                }
            }
            
            if(!currentActualTarget){
                path.maxSpeed = enemyMoveSpeed * currentDifficulty * enemyMoveSpeedSlowMultiplier;
                if(Vector2.Distance(transform.position, patrolDestination) <= 2f){
                    patrolDestination = new Vector2(UnityEngine.Random.Range(-21, -4), UnityEngine.Random.Range(-3, 4));
                }
                PathFindingStuckFix(true);
                path.destination = patrolDestination;
            }
        }

        
    }
}
