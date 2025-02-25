using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerControls : MonoBehaviour
{
    //Movement
    private Rigidbody2D rb;
    //public float playerRotateSpeed = 1f;
    public float playerMoveSpeed = 1f;
    public float playerMoveSpeedMultiplierMax = 2f;
    private float playerMoveSpeedMultiplier;
    public GameObject playerBody;
    
    private bool pressW = false;
    private bool pressA = false;
    private bool pressS = false;
    private bool pressD = false;

    //Shooting
    public bool isAlive = true;
    public TMPro.TextMeshProUGUI ammoUI = null;
    public int totalBullets = 10;
    private int currentBullets = 0;
    public GameObject cannonHead;
    public GameObject playerBullet;
    public float bulletShootDistance = 0.5f;

    private int currentDifficulty = 1;

    public void SetDifficultyLevel(int newDifficultyLevel){
        currentDifficulty = newDifficultyLevel;
        currentBullets = totalBullets * currentDifficulty;
    }

    public void UpPress(){
        currentBullets += 1;
        pressW = true;
        pressS = false;
    }

    public void DownPress(){
        currentBullets += 1;
        pressW = false;
        pressS = true;
    }

    public void LeftPress(){
        currentBullets += 1;
        pressA = true;
        pressD = false;
    }

    public void RightPress(){
        currentBullets += 1;
        pressA = false;
        pressD = true;
    }

    public void StopPress(){
        currentBullets += 1;
        pressW = false;
        pressA = false;
        pressS = false;
        pressD = false;
    }

    void OnExplosionHit(){
        Debug.Log(gameObject.name + " got hit be explosion");

        isAlive = false;
        Destroy(gameObject);
    }

    void OnBulletHit(GameObject bullet){
        //string bulletType = bullet.name;
        //Debug.Log("Player Bullet Hit" + bulletType);

        if(bullet){
            Destroy(bullet);
        }

        isAlive = false;
        Destroy(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        rb = GetComponent<Rigidbody2D>();

        if(currentBullets == 0){
            currentBullets = totalBullets * currentDifficulty;
        }   
    }

    // Update is called once per frame
    void Update(){
        if(isAlive){
            //Shooting:
            Vector2 mouseScreenPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            cannonHead.transform.up = mouseScreenPosition - (Vector2) cannonHead.transform.position;

            if(Input.GetKey(KeyCode.LeftShift)){
                playerMoveSpeedMultiplier = playerMoveSpeedMultiplierMax;
            }else{
                playerMoveSpeedMultiplier = 1f;
            }

            if((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && currentBullets > 0){
                currentBullets--;
                Instantiate(playerBullet, cannonHead.transform.position + (cannonHead.transform.up * bulletShootDistance), cannonHead.transform.rotation);
            }
            ammoUI.text = currentBullets + " / " + totalBullets;

            //Movement:
            Vector3 currentPlayerVelocity = Vector3.zero;
            string currentPlayerRotationString = "";

            if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || pressW){
                currentPlayerVelocity += transform.up * playerMoveSpeed * playerMoveSpeedMultiplier;
                currentPlayerRotationString += "W";
            }else if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) || pressS){
                currentPlayerVelocity -= transform.up * playerMoveSpeed * playerMoveSpeedMultiplier;
                currentPlayerRotationString += "S";
            }
            
            if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) || pressA){
                currentPlayerVelocity -= transform.right * playerMoveSpeed * playerMoveSpeedMultiplier;
                currentPlayerRotationString += "A";
            }else if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) || pressD){
                currentPlayerVelocity += transform.right * playerMoveSpeed * playerMoveSpeedMultiplier;
                currentPlayerRotationString += "D";
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
            //Old Movement (Tank Controls):
            
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
        
            //playerLifeTimer += Time.deltaTime;
        }
    }
}
