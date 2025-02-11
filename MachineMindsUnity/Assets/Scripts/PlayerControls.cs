using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    //Movement
    private Rigidbody2D rb;
    //public float playerRotateSpeed = 1f;
    public float playerMoveSpeed = 1f;
    public GameObject playerBody;

    //Shooting
    private bool isAlive = true;
    public int totalBullets = 10;
    private int currentBullets;
    public GameObject cannonHead;
    public GameObject playerBullet;

    //Point Calculation
    public float pointsPerEnemy = 1000;
    public float difficultyMultiplier = 2f;

    //Possibly Useful Statistics
    private float playerLifeTimer = 0f;
    private int totalEnemiesKilled = 0;
    public int currentDifficulty = 0;
    private float totalPoints = 0f;

    void OnBulletHit(string bulletType){
        Debug.Log("Player Bullet Hit" + bulletType);

        if(bulletType.ToLower().Contains("enemy")){
            //restart level or go to main menu
            isAlive = false;
            Destroy(gameObject);
        }else if(bulletType.ToLower().Contains("player")){
            //could make it so bullets can also hurt yourself
        }
    }

    void OnEnemyDeath(){
        totalEnemiesKilled += 1;
        totalPoints += (pointsPerEnemy * Mathf.Pow(difficultyMultiplier, currentDifficulty));
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        rb = GetComponent<Rigidbody2D>();
        currentBullets = totalBullets;
    }

    // Update is called once per frame
    void Update(){
        if(isAlive){
            //Shooting:
            Vector2 mouseScreenPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            cannonHead.transform.up = mouseScreenPosition - (Vector2) cannonHead.transform.position;

            if((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && currentBullets > 0){
                currentBullets--;
                Instantiate(playerBullet, cannonHead.transform.position + (cannonHead.transform.up * 1.1f), cannonHead.transform.rotation);
            }

            //Movement:
            Vector3 currentPlayerVelocity = Vector3.zero;
            string currentPlayerRotationString = "";

            if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)){
                currentPlayerVelocity += transform.up * playerMoveSpeed;
                currentPlayerRotation += "W";
            }else if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)){
                currentPlayerVelocity -= transform.up * playerMoveSpeed;
                currentPlayerRotation += "S";
            }
            
            if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)){
                currentPlayerVelocity -= transform.right * playerMoveSpeed;
                currentPlayerRotation += "A";
            }else if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)){
                currentPlayerVelocity += transform.right * playerMoveSpeed;
                currentPlayerRotation += "D";
            }

            rb.linearVelocity = currentPlayerVelocity;

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


            /*
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
            */
        
            playerLifeTimer += Time.deltaTime;
        }
    }
}
