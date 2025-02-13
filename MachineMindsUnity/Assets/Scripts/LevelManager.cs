using UnityEngine;
using UnityEngine.SceneManagement;

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
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){

    }

    // Update is called once per frame
    void Update(){
        if(currentPlayer){
            countdownUI.text = "";
            playerLifeTimer += Time.deltaTime;

            if(wonLevel){
                if(currentWinTime > playerCelebrateTime){
                    countdownUI.text = "Loading...";
                    string currentScene = SceneManager.GetActiveScene().name;
                    
                    string[] tempArray = currentScene.Split("Level");
                    int sceneNumber = int.Parse(tempArray[tempArray.Length - 1]);
                    
                    string newScene = "Scenes/Levels/Level" + (sceneNumber + 1);
                    SceneManager.LoadScene(newScene, LoadSceneMode.Single);
                }else{
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
                countdownUI.text = "Loading...";
                SceneManager.LoadScene("Scenes/Levels/Level1", LoadSceneMode.Single);
            }else{
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
