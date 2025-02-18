using System;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour{
    private Rigidbody2D rb;

    private GameObject targetPlayer;
    private GameObject levelManager;
    
    public float enemyMoveSpeed = 1f;

    public int maxEnemyHealth; //# bullets they can survive
    private int currentEnemyHealth;

    public GameObject enemyBullet;
    public float bulletShotSpawnOffset = 0.5f;

    public float enemyFireRate = 5f;
    private float enemyFireTimer = 0f;

    public float distanceToPlayer = 5f;

    public float pointsWorth;

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
        currentEnemyHealth = maxEnemyHealth;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update(){
        if(targetPlayer){  
            //Target Player:
            transform.up = targetPlayer.transform.position - transform.position;
            if (Vector2.Distance(transform.position, targetPlayer.transform.position) > distanceToPlayer){
                rb.linearVelocity = transform.up * enemyMoveSpeed;
            }else{
            rb.linearVelocity = transform.up * 0.000001f;
            }
        
            //Shoot Player:
            if(enemyFireTimer >= enemyFireRate){
                Instantiate(enemyBullet, transform.position + (transform.up * bulletShotSpawnOffset), transform.rotation);
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
