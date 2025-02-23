using System;
using UnityEngine;
using Pathfinding;

public class EnemyBehavior : MonoBehaviour{
    public GameObject cannonHead;

    private Rigidbody2D rb;

    private AIPath path;
    private bool canShoot;
    private Vector2 lastPosition;
    private float stuckCheckTimer = 0f;
    private float stuckCheckInterval = 0.5f;

    private GameObject targetPlayer;
    private GameObject levelManager;
    
    public float enemyMoveSpeed = 1f;

    public int maxEnemyHealth; //# bullets they can survive
    private int currentEnemyHealth;

    public GameObject enemyBullet;
    public float bulletShotSpawnOffset = 0.5f;

    public float enemyFireRate = 5f;
    private float enemyFireTimer = 0f;

    //public float distanceToPlayer = 5f;

    public float pointsWorth;

    private const string playerName = "player";

    public void SetGameObjects(GameObject[] parameters){
        levelManager = parameters[0];
        targetPlayer = parameters[1];
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        bulletShotSpawnOffset = (transform.localScale.magnitude / 2) + 0.1f;
        currentEnemyHealth = maxEnemyHealth;
        rb = GetComponent<Rigidbody2D>();
        path = GetComponent<AIPath>();

        lastPosition = (Vector2) transform.position;
    }

    // Update is called once per frame
    void Update(){
        if(targetPlayer){
            if(cannonHead){
                cannonHead.transform.up = targetPlayer.transform.position - transform.position;
            }

            RaycastHit2D scanAhead;
            if(cannonHead){
                scanAhead = Physics2D.Raycast(cannonHead.transform.position + (transform.up * bulletShotSpawnOffset), cannonHead.transform.up, Mathf.Infinity);
            }else{
                scanAhead = Physics2D.Raycast(transform.position + (cannonHead.transform.up * bulletShotSpawnOffset), cannonHead.transform.up, Mathf.Infinity);
            }

            //Target Player:
            /*transform.up = targetPlayer.transform.position - transform.position;
            if (Vector2.Distance(transform.position, targetPlayer.transform.position) > distanceToPlayer){
                rb.linearVelocity = transform.up * enemyMoveSpeed;
            }else{
                rb.linearVelocity = transform.up * 0.000001f;
            }*/

            if (scanAhead && scanAhead.transform.gameObject.name.ToLower().Contains(playerName)){
                path.maxSpeed = 0.01f;
            }else{
                path.maxSpeed = enemyMoveSpeed;
                path.destination = targetPlayer.transform.position;

                if(stuckCheckTimer <= 0){
                    Vector2 latestPosition = (Vector2) transform.position;
                    stuckCheckTimer = stuckCheckInterval;

                    if(Vector2.Distance(latestPosition, lastPosition) <= 0.01f){
                        Debug.Log("Stuck");
                        transform.Rotate(0, 0, 180f);
                        //rb.AddForce(-transform.up * 5000f);
                    }

                    lastPosition = latestPosition;
                }
                stuckCheckTimer -= Time.deltaTime;
            }

            //Shoot Player:
            if(enemyFireTimer >= enemyFireRate){                
                Debug.DrawLine(transform.position + (transform.up * bulletShotSpawnOffset), transform.position + (transform.up * 100f), Color.white, enemyFireRate / 2);

                //Debug.Log(hit.transform.gameObject.name);
                if(scanAhead && scanAhead.transform.gameObject.name.ToLower().Contains(playerName)){
                    if(cannonHead){
                        Instantiate(enemyBullet, cannonHead.transform.position + (cannonHead.transform.up * bulletShotSpawnOffset), cannonHead.transform.rotation);
                    }else{
                        Instantiate(enemyBullet, transform.position + (transform.up * bulletShotSpawnOffset), transform.rotation);
                    }
                }
                
                enemyFireTimer = 0f;
            }else{
                enemyFireTimer += Time.deltaTime;
            }
        }else{
            //Celebrate?
            transform.Rotate(0, 0, 1f);
        }

        
    }
}
