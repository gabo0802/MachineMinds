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
    
    private int levelStartEnemies = 0;
    private float levelStartPoints = 0f;
    private float playerLifeTimerStart = 0f;

    public int totalPlayerLives = 3;
    private int currentPlayerLives;

    public int numberLevelsCheckpoint = 5;
    private int currentLevelNumber;

    void OnEnemyDeath(int enemyPointWorth){
        totalEnemiesKilled += 1;
        currentEnemyTotal -= 1;
        totalPoints += (enemyPointWorth * Mathf.Pow(difficultyMultiplier, currentDifficulty));
        
        pointsUI.text = totalPoints + " pts";

        if(currentEnemyTotal <= 0 && currentPlayer){
           wonLevel = true;
        }
    }

    void ResetSaveFile(){
        playerLifeTimer = 0f;
        currentPlayerLives = totalPlayerLives;
        totalPoints = 0f;
        totalEnemiesKilled = 0;
        CreateSave_LevelEnd(1);
    }

    void CreateSave_LevelRetry(int currentLevelNumber){            
        using (StreamWriter sw = File.CreateText(filePath)){
            sw.WriteLine(currentPlayerLives);
            sw.WriteLine(currentLevelNumber);
            sw.WriteLine(levelStartPoints);
            sw.WriteLine(levelStartEnemies);
            sw.WriteLine(currentDifficulty);        
            sw.WriteLine(playerLifeTimerStart);
        }
    }

    void CreateSave_LevelEnd(int currentLevelNumber){            
        using (StreamWriter sw = File.CreateText(filePath)){
            sw.WriteLine(currentPlayerLives);
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
            currentPlayerLives = System.Int32.Parse(fileArray[0]);
            //currentLevelNumber = System.Int32.Parse(fileArray[1]);
            totalPoints = System.Single.Parse(fileArray[2]);
            totalEnemiesKilled = System.Int32.Parse(fileArray[3]);
            currentDifficulty = System.Int32.Parse(fileArray[4]);
            playerLifeTimer = System.Single.Parse(fileArray[5]);

            playerLifeTimerStart = playerLifeTimer;
            levelStartEnemies = totalEnemiesKilled;
            levelStartPoints = totalPoints;
        }else{
            ResetSaveFile();
        }
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        currentLevelNumber = SceneManager.GetActiveScene().buildIndex;
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
                    if(currentLevelNumber % 5 == 0 && currentPlayerLives >= totalPlayerLives){
                        currentPlayerLives++;
                    }
                    
                    countdownUI.text = currentPlayerLives + " / " + totalPlayerLives;

                    //Load Next Level:
                    CreateSave_LevelEnd(currentLevelNumber + 1);          
                    SceneManager.LoadScene(currentLevelNumber + 1, LoadSceneMode.Single);
                }else{
                    //Celebrate Time!!!!:
                    currentWinTime += Time.deltaTime;

                    if(currentWinTime < 1f){
                        countdownUI.text = "You Won! :)";
                    }else if (currentWinTime < playerCelebrateTime * 0.5f && currentLevelNumber % 5 == 0 && currentPlayerLives >= totalPlayerLives){
                        countdownUI.text = "Lives Increased";
                    }else{
                        countdownUI.text = Mathf.Round(playerCelebrateTime - currentWinTime) + "";
                    }
                }
            }
        }else{
            countdownUI.text = Mathf.Round(playerRespawnTime - currentDeadTime) + "";
            if(currentDeadTime > playerRespawnTime){
                //Loading:
                countdownUI.text = "Loading...";
                
                /*
                //Send to AI?:
                sendToAI()
                */

                currentPlayerLives -= 1;

                if(currentPlayerLives > 0){
                    countdownUI.text = currentPlayerLives + " / " + totalPlayerLives;
                    CreateSave_LevelEnd(currentLevelNumber);
                    SceneManager.LoadScene(currentLevelNumber, LoadSceneMode.Single);
                }else{
                    //Reset Save File:
                    ResetSaveFile();

                    //Go Back to First Level:
                    SceneManager.LoadScene(currentLevelNumber / numberLevelsCheckpoint, LoadSceneMode.Single);
                }
            }else{
                //Enemy Dancing Time:
                currentDeadTime += Time.deltaTime;

                if(currentDeadTime < 1f){
                    countdownUI.text = "You Lost. :(";
                }else{
                    countdownUI.text = Mathf.Round(playerRespawnTime - currentDeadTime) + "";
                }
            }
        }
    }
}
