using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour{
    private string filePath = Application.dataPath + "/Resources/GameState.save";
    private const string playerName = "player";
    private const string enemyName = "enemy";

    private GameObject currentPlayer;
    private TMPro.TextMeshProUGUI pointsUI;
    private TMPro.TextMeshProUGUI countdownUI;
    private float playerLifeTimer = 0f;

    private bool wonLevel = false;
    public float playerCelebrateTime = 3f;
    private float currentWinTime = 0f;

    public float playerRespawnTime = 3f;
    private float currentDeadTime = 0f;

    private int currentEnemyTotal = 0;
    private int totalEnemiesKilled = 0;

    public float difficultyMultiplier = 2f;
    public int currentDifficulty = 0;
    private float totalPoints = 0f;

    void OnEnemyDeath(int enemyPointWorth){
        totalEnemiesKilled += 1;
        currentEnemyTotal -= 1;
        totalPoints += (enemyPointWorth * Mathf.Pow(difficultyMultiplier, currentDifficulty));
        
        pointsUI.text = totalPoints + " pts";

        if(currentEnemyTotal <= 0 && currentPlayer){
           wonLevel = true;
        }
    }

    void CreateSave_LevelEnd(int currentLevelNumber){            
        using (StreamWriter sw = File.CreateText(filePath)){
            sw.WriteLine(currentLevelNumber);
            sw.WriteLine(totalPoints);
            sw.WriteLine(totalEnemiesKilled);
            sw.WriteLine(currentDifficulty);        
            sw.WriteLine(playerLifeTimer);
        }
    }

    void LoadSave(){
        if (File.Exists(filePath)){
            string saveFileData = "";
            using (StreamReader saveFile = File.OpenText(filePath)){
                string currentLine;
                while ((currentLine = saveFile.ReadLine()) != null){
                    saveFileData += currentLine + "\n";
                }
            }

            string[] fileArray = saveFileData.Split('\n');
            //currentLevelNumber = System.Int32.Parse(fileArray[0]);
            totalPoints = System.Single.Parse(fileArray[1]);
            totalEnemiesKilled = System.Int32.Parse(fileArray[2]);
            currentDifficulty = System.Int32.Parse(fileArray[3]);
            playerLifeTimer = System.Single.Parse(fileArray[4]);
        }
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        //Get enemy total + get player object
        Collider2D[] allHitObjectsInScence = Physics2D.OverlapCircleAll(new Vector2(0, 0), Mathf.Infinity);
        List<GameObject> allEnemies = new List<GameObject>();

        foreach(Collider2D currentHitObject in allHitObjectsInScence){
            string currentObjectName = currentHitObject.transform.gameObject.name;

            if(currentObjectName.ToLower().Contains(enemyName)){
                allEnemies.Add(currentHitObject.transform.gameObject);
                currentEnemyTotal += 1;
            }else if(currentObjectName.ToLower().Contains(playerName)){
                currentPlayer = currentHitObject.transform.gameObject;
            }
        }

        foreach(GameObject enemyObject in allEnemies){
            enemyObject.SendMessageUpwards("SetGameObjects", new GameObject[]{gameObject, currentPlayer});
        }

        pointsUI = currentPlayer.transform.GetChild(2).GetChild(1).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
        countdownUI = transform.GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();

        //Save point total across levels
        LoadSave();
        pointsUI.text = totalPoints + " pts"; 
    }

    // Update is called once per frame
    void Update(){
        if(currentPlayer){
            countdownUI.text = "";
            playerLifeTimer += Time.deltaTime;

            if(wonLevel){
                if(currentWinTime > playerCelebrateTime){
                    countdownUI.text = "Loading...";

                    //Load Next Level:
                    CreateSave_LevelEnd(SceneManager.GetActiveScene().buildIndex + 1);          
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Single);
                }else{
                    //Celebrate Time!!!!:
                    currentWinTime += Time.deltaTime;

                    if(currentWinTime < 1f){
                        countdownUI.text = "You Won! :)";
                    }else{
                        countdownUI.text = (playerCelebrateTime - currentWinTime) + " s";
                    }
                }
            }
        }else{
            countdownUI.text = (playerRespawnTime - currentDeadTime) + " s";
            if(currentDeadTime > playerRespawnTime){
                //Loading:
                countdownUI.text = "Loading...";
                
                /*
                //Send to AI?:
                sendToAI()
                */

                //Reset Save File:
                playerLifeTimer = 0f;
                totalPoints = 0f;
                totalEnemiesKilled = 0;
                CreateSave_LevelEnd(1);

                //Go Back to First Level:
                SceneManager.LoadScene("Scenes/Levels/Level1", LoadSceneMode.Single);
            }else{
                //Enemy Dancing Time:
                currentDeadTime += Time.deltaTime;

                if(currentDeadTime < 1f){
                    countdownUI.text = "You Lost. :(";
                }else{
                    countdownUI.text = (playerRespawnTime - currentDeadTime) + " s";
                }
            }
        }
    }
}
