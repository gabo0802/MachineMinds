using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class LevelManager : MonoBehaviour{

    public GameObject currentPlayer;
    public TMPro.TextMeshProUGUI pointsUI;
    public TMPro.TextMeshProUGUI countdownUI;
    private float playerLifeTimer = 0f;

    private bool wonLevel = false;
    public float playerCelebrateTime = 3f;
    private float currentWinTime = 0f;

    public float playerRespawnTime = 3f;
    private float currentDeadTime = 0f;

    public int currentEnemyTotal = 1;
    private int totalEnemiesKilled = 0;

    public float pointsPerEnemy = 1000;
    public float difficultyMultiplier = 2f;
    public int currentDifficulty = 0;
    private float totalPoints = 0f;

    void OnEnemyDeath(){
        totalEnemiesKilled += 1;
        currentEnemyTotal -= 1;
        totalPoints += (pointsPerEnemy * Mathf.Pow(difficultyMultiplier, currentDifficulty));
        
        pointsUI.text = totalPoints + " pts";

        if(currentEnemyTotal <= 0 && currentPlayer){
           wonLevel = true;
        }
    }

    void CreateSave_LevelEnd(int currentLevelNumber){
        string filePath = Application.dataPath + "/Resources/GameState.save";
        
        if(!File.Exists(filePath)){
            File.CreateText(filePath);
        }
    
        using (StreamWriter sw = File.CreateText(filePath)){
            sw.WriteLine(currentLevelNumber);
            sw.WriteLine(totalPoints);
            sw.WriteLine(totalEnemiesKilled);
            sw.WriteLine(currentDifficulty);        
            sw.WriteLine(playerLifeTimer);
        }
    }

    void LoadSave(){
        string filePath = Application.dataPath + "/Resources/GameState.save";

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
                    string currentScene = SceneManager.GetActiveScene().name;
                    
                    string[] tempArray = currentScene.Split("Level");
                    int sceneNumber = int.Parse(tempArray[tempArray.Length - 1]);
                    CreateSave_LevelEnd(sceneNumber);
                    
                    string newScene = "Scenes/Levels/Level" + (sceneNumber + 1);
                    SceneManager.LoadScene(newScene, LoadSceneMode.Single);
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
                CreateSave_LevelEnd(0);

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
