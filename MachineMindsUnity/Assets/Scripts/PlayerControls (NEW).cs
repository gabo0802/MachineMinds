using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerControlsNEW : MonoBehaviour
{
    public GameObject playerBody;
    public GameObject cannonHead;

    private Rigidbody2D rb;

    public float playerMoveSpeed = 1f;
    public float playerRotateSpeed = 1f; //if want to use tank controls
    private float playerMoveSpeedMultiplier = 1f;
    private float playerMoveSpeedBoostMultiplier = 1f;

    public GameObject playerBullet;
    public float bulletShootDistance = 0.5f;

    public bool useTankControlMovement = false;
    private bool pressW = false;
    private bool pressA = false;
    private bool pressS = false;
    private bool pressD = false;

    public void UpPress(){
        pressW = true;
        pressS = false;
    }

    public void DownPress(){
        pressW = false;
        pressS = true;
    }

    public void LeftPress(){
        pressA = true;
        pressD = false;
    }

    public void RightPress(){
        pressA = false;
        pressD = true;
    }

    public void StopPress(){
        pressW = false;
        pressA = false;
        pressS = false;
        pressD = false;
    }

    public void ShootBullet(){
        Instantiate(playerBullet, cannonHead.transform.position + (cannonHead.transform.up * bulletShootDistance), cannonHead.transform.rotation);
    }

    public void AffectSpeed(float newMultiplier){
        playerMoveSpeedMultiplier = newMultiplier;
    }

    public void AffectBoostSpeed(float newMultiplier){
        playerMoveSpeedBoostMultiplier = newMultiplier;
    }

    private void tankControlMovement(){
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || pressW){
            rb.linearVelocity = transform.up * playerMoveSpeed * playerMoveSpeedBoostMultiplier * playerMoveSpeedMultiplier;
        }else if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) || pressS){
            rb.linearVelocity = transform.up * -playerMoveSpeed * playerMoveSpeedBoostMultiplier * playerMoveSpeedMultiplier;
        }else{
            rb.linearVelocity = new Vector2(0, 0);
        }

        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) || pressA){
            transform.Rotate(0, 0, playerRotateSpeed);
        }
        else if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) || pressD){
             transform.Rotate(0, 0, -playerRotateSpeed);
        }
    }

    private void normalControlMovement(){
        //Movement:
        Vector3 currentPlayerVelocity = Vector3.zero;
        string currentPlayerRotationString = "";

        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || pressW){
            currentPlayerVelocity += transform.up;
            currentPlayerRotationString += "W";
        }else if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) || pressS){
            currentPlayerVelocity -= transform.up;
            currentPlayerRotationString += "S";
        }
            
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) || pressA){
            currentPlayerVelocity -= transform.right;
            currentPlayerRotationString += "A";
        }else if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) || pressD){
            currentPlayerVelocity += transform.right;
            currentPlayerRotationString += "D";
        }

        rb.linearVelocity = currentPlayerVelocity * playerMoveSpeed * playerMoveSpeedBoostMultiplier * playerMoveSpeedMultiplier;

        //Rotation:
        if(currentPlayerRotationString == "WA" || currentPlayerRotationString == "SD" ){
            playerBody.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 45));
        }else if(currentPlayerRotationString == "WD" || currentPlayerRotationString == "SA"){
            playerBody.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -45));
        }else if(currentPlayerRotationString == "W" || currentPlayerRotationString == "S"){
            playerBody.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }else if(currentPlayerRotationString == "D" || currentPlayerRotationString == "A"){
            playerBody.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
        }       
    }

    void OnExplosionHit(){
        //Debug.Log(gameObject.name + " got hit be explosion");

        Destroy(gameObject);
    }

    void OnBulletHit(GameObject bullet){
        //Debug.Log("Player Bullet Hit" + bulletType);

        if(bullet){
            Destroy(bullet);
        }

        Destroy(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update(){
        if(gameObject){
            //Move Cannon to Mouse Position
            Vector2 mouseScreenPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            cannonHead.transform.up = mouseScreenPosition - (Vector2) cannonHead.transform.position;

            //Player Movement:
            if(useTankControlMovement){
                tankControlMovement();
            }else{
                normalControlMovement();
            }
        }
    }
}
