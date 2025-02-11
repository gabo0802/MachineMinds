using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour{

    public GameObject currentPlayer;
    private float playerLifeTimer = 0f;

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

        if(currentEnemyTotal <= 0 && currentPlayer){
            string currentScene = SceneManager.GetActiveScene().name;
            
            string[] tempArray = currentScene.Split("Level");
            int sceneNumber = int.Parse(tempArray[tempArray.Length - 1]);
            
            string newScene = "Levels/Level" + (sceneNumber + 1);
            SceneManager.LoadScene(newScene, LoadSceneMode.Single);
        }
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){

    }

    // Update is called once per frame
    void Update(){
        if(currentPlayer){
            playerLifeTimer += Time.deltaTime;
        }else{
            if(currentDeadTime > playerRespawnTime){
                SceneManager.LoadScene("Levels/Level1", LoadSceneMode.Single);
            }else{
                currentDeadTime += Time.deltaTime;
                Debug.Log(playerRespawnTime - currentDeadTime);
            }
        }
    }
}
