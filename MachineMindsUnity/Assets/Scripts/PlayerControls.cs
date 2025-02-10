using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    private Rigidbody2D rb;
    public float playerRotateSpeed = 1f;
    public float playerMoveSpeed = 1f;
    
    public int totalBullets = 10;
    private int currentBullets;
    public GameObject playerBullet;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        rb = GetComponent<Rigidbody2D>();
        currentBullets = totalBullets;
    }

    void OnBulletHit(String bulletType){
        Debug.Log("Player Bullet Hit" + bulletType)
    }

    // Update is called once per frame
    void Update(){
        //Shooting:
        if(Input.GetKeyDown(KeyCode.Space) && currentBullets > 0){
            currentBullets--;
            Instantiate(playerBullet, transform.position + (transform.up * 1.1f), transform.rotation);
        }
        
        //Movement:
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)){
            rb.linearVelocity = transform.up * playerMoveSpeed;
        }else if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)){
           rb.linearVelocity = transform.up * -playerMoveSpeed;
        }else{
            rb.linearVelocity = new Vector2(0, 0);
        }

        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)){
            transform.Rotate(0, 0, playerRotateSpeed);
        }
        else if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)){
            transform.Rotate(0, 0, -playerRotateSpeed);
        }
       
    }
}
