using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerControls : MonoBehaviour
{
    public bool godMode = false;
    public GameObject playerBody;
    public GameObject cannonHead;
    int layerMask;

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
    public float wallHitGlitchDistance = 0.225f;

    public void UpPress()
    {
        pressW = true;
        pressS = false;
    }

    public void DownPress()
    {
        pressW = false;
        pressS = true;
    }

    public void LeftPress()
    {
        pressA = true;
        pressD = false;
    }

    public void RightPress()
    {
        pressA = false;
        pressD = true;
    }

    public void StopPress()
    {
        pressW = false;
        pressA = false;
        pressS = false;
        pressD = false;
    }

    public void ShootBullet()
    {
        bool shootWallGlitchPrevention = false;
        Vector3 originPoint = cannonHead.transform.position + (cannonHead.transform.up * bulletShootDistance);

        Collider2D[] possibleWallHitArray = Physics2D.OverlapCircleAll(originPoint, wallHitGlitchDistance, layerMask);
        foreach (Collider2D collider in possibleWallHitArray)
        {
            if (collider.name.ToLower().Contains("wall") && !collider.name.ToLower().Contains("break"))
            {
                shootWallGlitchPrevention = true;
            }
        }

        if (!shootWallGlitchPrevention)
        {
            Instantiate(playerBullet, cannonHead.transform.position + (cannonHead.transform.up * bulletShootDistance), cannonHead.transform.rotation);
        }
    }

    public void AffectSpeed(float newMultiplier)
    {
        playerMoveSpeedMultiplier = newMultiplier;
    }

    public void AffectBoostSpeed(float newMultiplier)
    {
        playerMoveSpeedBoostMultiplier = newMultiplier;
    }

    private const float slipperyIceSpeed = 5f;
    private const float slideMultiplier = 0.25f;

    private void tankControlMovement()
    {
        KeyCode upKey = PlayerPrefs.HasKey("KeyMovementUp") ? (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("KeyMovementUp")): KeyCode.Space;
        KeyCode downKey = PlayerPrefs.HasKey("KeyMovementDown") ? (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("KeyMovementDown")): KeyCode.Space;
        KeyCode leftKey = PlayerPrefs.HasKey("KeyMovementLeft") ? (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("KeyMovementLeft")): KeyCode.Space;
        KeyCode rightKey = PlayerPrefs.HasKey("KeyMovementRight") ? (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("KeyMovementRight")): KeyCode.Space;

        if (Input.GetKey(upKey) || Input.GetKey(KeyCode.UpArrow) || pressW)
        {
            if(playerMoveSpeedMultiplier == slipperyIceSpeed){
                rb.AddForce(transform.up * playerMoveSpeed * playerMoveSpeedBoostMultiplier * slideMultiplier); //slippery ice
            }else{
                rb.linearVelocity = transform.up * playerMoveSpeed * playerMoveSpeedBoostMultiplier * playerMoveSpeedMultiplier;
            }
        }
        else if (Input.GetKey(downKey) || Input.GetKey(KeyCode.DownArrow) || pressS)
        {   
            if(playerMoveSpeedMultiplier == slipperyIceSpeed){
                rb.AddForce(transform.up * -playerMoveSpeed * playerMoveSpeedBoostMultiplier * slideMultiplier); //slippery ice
            }else{
                rb.linearVelocity = transform.up * -playerMoveSpeed * playerMoveSpeedBoostMultiplier * playerMoveSpeedMultiplier;
            }
        }   
        else
        {   
            if(playerMoveSpeedMultiplier != slipperyIceSpeed){
                rb.linearVelocity = new Vector2(0, 0);
            }
        }

        if (Input.GetKey(leftKey) || Input.GetKey(KeyCode.LeftArrow) || pressA)
        {
            transform.Rotate(0, 0, playerRotateSpeed);
        }
        else if (Input.GetKey(rightKey) || Input.GetKey(KeyCode.RightArrow) || pressD)
        {
            transform.Rotate(0, 0, -playerRotateSpeed);
        }
    }

    private void normalControlMovement()
    {   
        KeyCode upKey = PlayerPrefs.HasKey("KeyMovementUp") ? (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("KeyMovementUp")): KeyCode.W;
        KeyCode downKey = PlayerPrefs.HasKey("KeyMovementDown") ? (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("KeyMovementDown")): KeyCode.S;
        KeyCode leftKey = PlayerPrefs.HasKey("KeyMovementLeft") ? (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("KeyMovementLeft")): KeyCode.A;
        KeyCode rightKey = PlayerPrefs.HasKey("KeyMovementRight") ? (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("KeyMovementRight")): KeyCode.D;

        //Movement:
        Vector3 currentPlayerVelocity = Vector3.zero;
        string currentPlayerRotationString = "";

        if (Input.GetKey(upKey) || Input.GetKey(KeyCode.UpArrow) || pressW)
        {
            currentPlayerVelocity += transform.up;
            currentPlayerRotationString += "W";
        }
        else if (Input.GetKey(downKey) || Input.GetKey(KeyCode.DownArrow) || pressS)
        {
            currentPlayerVelocity -= transform.up;
            currentPlayerRotationString += "S";
        }

        if (Input.GetKey(leftKey) || Input.GetKey(KeyCode.LeftArrow) || pressA)
        {
            currentPlayerVelocity -= transform.right;
            currentPlayerRotationString += "A";
        }
        else if (Input.GetKey(rightKey) || Input.GetKey(KeyCode.RightArrow) || pressD)
        {
            currentPlayerVelocity += transform.right;
            currentPlayerRotationString += "D";
        }
        // Speed change on editor for screenshots.
#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.Q)) {
            if (Time.timeScale == 0.1f) {
                Time.timeScale = 1f;
            } else {
                Time.timeScale = 0.1f;
            }
        }
#endif

        if(playerMoveSpeedMultiplier == slipperyIceSpeed){
            rb.AddForce(currentPlayerVelocity * playerMoveSpeed * playerMoveSpeedBoostMultiplier * slideMultiplier); //slippery ice
        }else{
            rb.linearVelocity = currentPlayerVelocity * playerMoveSpeed * playerMoveSpeedBoostMultiplier * playerMoveSpeedMultiplier;
        }

        //Rotation:
        if (currentPlayerRotationString == "WA" || currentPlayerRotationString == "SD")
        {
            playerBody.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 45));
        }
        else if (currentPlayerRotationString == "WD" || currentPlayerRotationString == "SA")
        {
            playerBody.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -45));
        }
        else if (currentPlayerRotationString == "W" || currentPlayerRotationString == "S")
        {
            playerBody.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
        else if (currentPlayerRotationString == "D" || currentPlayerRotationString == "A")
        {
            playerBody.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
        }
    }

    void OnExplosionHit()
    {
        //Debug.Log(gameObject.name + " got hit be explosion");

        if(!godMode){
            Destroy(gameObject);
        }
    }

    void OnBulletHit(GameObject bullet)
    {
        //Debug.Log("Player Bullet Hit" + bulletType);

        if (bullet)
        {
            Destroy(bullet);
        }

        if(!godMode){
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(PlayerPrefs.GetInt("FirstLevelDeaths") > 5){
            godMode = true;
        }
        layerMask = ~LayerMask.GetMask("InteractableGround"); // Ignores "NoBounce" layer
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject && Time.timeScale != 0f)
        {
            //Move Cannon to Mouse Position
            Vector2 mouseScreenPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            cannonHead.transform.up = mouseScreenPosition - (Vector2)cannonHead.transform.position;

            //Player Movement:
            if (useTankControlMovement)
            {
                tankControlMovement();
            }
            else
            {
                normalControlMovement();
            }
        }
    }
}
