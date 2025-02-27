using System;
using UnityEngine;
using Pathfinding;

public class EnemyBehaviorNew : MonoBehaviour{
    private Rigidbody2D rb;
    int layerMask;

    private AIPath path;
    private Vector2 patrolDestination;
    private Vector2 lastPosition;

    public GameObject cannonHead;
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
    private GameObject currentTarget;

    public void AffectSpeed(float newMultiplier){
        enemyMoveSpeedMultiplier = newMultiplier;
    }

    public void SetGameObjects(GameObject[] parameters){
        levelManager = parameters[0];
        currentAlivePlayer = parameters[1];
    }

    public void SetDifficultyLevel(int newDifficultyLevel){
        currentDifficulty = newDifficultyLevel;
    }

    void OnExplosionHit(){
        Debug.Log(gameObject.name + " got hit be explosion");

        currentEnemyHealth -= 1; //could make explosions instant kills?

        if(currentEnemyHealth <= 0){
            if(levelManager){
                levelManager.transform.SendMessage("OnEnemyDeath", pointsWorth);
            }
            Destroy(gameObject);
        }
    }

    void OnBulletHit(GameObject bullet){
        if(!currentTarget){
            patrolDestination = (Vector2) currentAlivePlayer.transform.position;
        }

        if(bullet){
            Destroy(bullet);
        }

        currentEnemyHealth -= 1;

        if(currentEnemyHealth <= 0){
            if(levelManager){
                levelManager.transform.SendMessage("OnEnemyDeath", pointsWorth);
            }
            Destroy(gameObject);
        }
    }

    private void PathFindingStuckFix(bool isPatrolling){
        
    }

    private void PatrolBehavior(){

    }

    private void TargetPlayerBehavior(){
        
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
        if(!currentAlivePlayer){
            //Player is Dead
            path.maxSpeed = 0f;
            path.destination = transform.position;
            transform.Rotate(0, 0, 1f);

        }else if (!currentTarget){
            //Patrol Behavior
            path.destination = patrolDestination;
            path.maxSpeed = enemyMoveSpeed * enemyMoveSpeedMultiplier;

            PatrolBehavior();
        }else{
            //Target Player Behavior
            path.destination = currentTarget.transform.position;
            path.maxSpeed = enemyMoveSpeed * enemyMoveSpeedMultiplier;

            TargetPlayerBehavior();
        }
    }
           
}
