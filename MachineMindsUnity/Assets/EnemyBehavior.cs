using System;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    private Rigidbody2D rb;

    public GameObject targetPlayer;
    public float enemyMoveSpeed = 1f;
    public GameObject enemyBullet;
    public float enemyFireRate = 5f;
    private float enemyFireTimer = 0f;
    public float distanceToPlayer = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created


    void OnCollsionEnter2D(Collision2D collision){
        Debug.Log("Enemy: " + collision.gameObject.name);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {   
        transform.up = targetPlayer.transform.position - transform.position;
        if (Vector2.Distance(transform.position, targetPlayer.transform.position) > distanceToPlayer){
            rb.linearVelocity = transform.up * enemyMoveSpeed;
        }else{
           rb.linearVelocity = transform.up * 0.000001f;
        }


        if(enemyFireTimer >= enemyFireRate){
            Instantiate(enemyBullet, transform.position + (transform.up * 1.1f), transform.rotation);
            enemyFireTimer = 0f;
        }
        else{
            enemyFireTimer += Time.deltaTime;
        }
    }
}
