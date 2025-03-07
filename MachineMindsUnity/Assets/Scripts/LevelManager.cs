using UnityEngine;
using UnityEngine.SceneManagement;
using static WebGLSaveSystem;
using System.Collections.Generic;
using System.Text;

public class LevelManager : MonoBehaviour
{
    private string saveFilePath = "GameState";

    private string aiTrainingFilePath = "GameData";
    private bool isTrainingMode = true;

    private const string playerName = "player";
    private const string enemyName = "enemy";

    private GameObject currentPlayer;
    private TMPro.TextMeshProUGUI pointsUI;
    private TMPro.TextMeshProUGUI countdownUI;
    public GameObject difficultySurveyPrefab; // Reference to the DifficultySurvey prefab
    private GameObject activeSurvey = null; // Reference to currently active survey
    private float playerLifeTimer = 0f;

    private bool wonLevel = false;
    public float playerCelebrateTime = 3f;
    private float currentWinTime = 0f;

    public float playerRespawnTime = 3f;
    private float currentDeadTime = 0f;

    private int currentEnemyTotal = 0;
    private int totalEnemiesKilled = 0;

    public float difficultyMultiplier = 2f;
    public int currentDifficulty = 1;
    private float totalPoints = 0f;

    private int levelStartEnemies = 0;
    private float levelStartPoints = 0f;
    private float playerLifeTimerStart = 0f;

    public int totalPlayerLives = 3;
    private int currentPlayerLives;

    private int numberLevelsCheckpoint = 3;
    private int currentLevelNumber;

    void OnEnemyDeath(int enemyPointWorth)
    {
        totalEnemiesKilled += 1;
        currentEnemyTotal -= 1;
        totalPoints += (enemyPointWorth * Mathf.Pow(difficultyMultiplier, currentDifficulty - 1));

        pointsUI.text = totalPoints + " pts";

        if (currentEnemyTotal <= 0 && currentPlayer)
        {
            wonLevel = true;
        }
    }

    void ResetSaveFile(int currentLevelNumber = 1)
    {
        playerLifeTimer = 0f;
        currentPlayerLives = totalPlayerLives;
        totalPoints = 0f;
        totalEnemiesKilled = 0;
        CreateSave_LevelEnd(currentLevelNumber);
    }

    void CreateSave_LevelRetry(int currentLevelNumber)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(currentPlayerLives.ToString());
        sb.AppendLine(levelStartPoints.ToString());
        sb.AppendLine(levelStartEnemies.ToString());
        sb.AppendLine(currentDifficulty.ToString());
        sb.AppendLine(playerLifeTimerStart.ToString());
        sb.AppendLine(isTrainingMode.ToString());
        WebGLSaveSystem.WriteAllText(saveFilePath, sb.ToString());
    }

    void CreateSave_LevelEnd(int currentLevelNumber)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(currentPlayerLives.ToString());
        sb.AppendLine(totalPoints.ToString());
        sb.AppendLine(totalEnemiesKilled.ToString());
        sb.AppendLine(currentDifficulty.ToString());
        sb.AppendLine(playerLifeTimer.ToString());
        sb.AppendLine(isTrainingMode.ToString());
        WebGLSaveSystem.WriteAllText(saveFilePath, sb.ToString());
    }

    void LoadSave()
    {
        bool fileExists = WebGLSaveSystem.FileExists(saveFilePath);

        if (fileExists)
        {
            string saveFileData = WebGLSaveSystem.ReadAllText(saveFilePath);

            string[] fileArray = saveFileData.Split('\n');
            currentPlayerLives = System.Int32.Parse(fileArray[0]);
            totalPoints = System.Single.Parse(fileArray[1]);
            totalEnemiesKilled = System.Int32.Parse(fileArray[2]);
            currentDifficulty = System.Int32.Parse(fileArray[3]);
            playerLifeTimer = System.Single.Parse(fileArray[4]);
            isTrainingMode = bool.Parse(fileArray[5]);

            playerLifeTimerStart = playerLifeTimer;
            levelStartEnemies = totalEnemiesKilled;
            levelStartPoints = totalPoints;
        }
        else
        {
            ResetSaveFile();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentLevelNumber = SceneManager.GetActiveScene().buildIndex;

        LoadSave();

        // Check if the difficulty survey prefab is assigned
        if (difficultySurveyPrefab == null)
        {
            Debug.LogWarning("DifficultySurvey prefab is not assigned in the Inspector!");
        }

        //Get enemy total + get player object
        Collider2D[] allHitObjectsInScence = Physics2D.OverlapCircleAll(new Vector2(0, 0), Mathf.Infinity);
        List<GameObject> allEnemies = new List<GameObject>();

        foreach (Collider2D currentHitObject in allHitObjectsInScence)
        {
            string currentObjectName = currentHitObject.transform.gameObject.name;

            if (currentObjectName.ToLower().Contains(enemyName))
            {
                allEnemies.Add(currentHitObject.transform.gameObject);
                currentEnemyTotal += 1;
            }
            else if (currentObjectName.ToLower().Contains(playerName))
            {
                currentPlayer = currentHitObject.transform.gameObject;
                currentPlayer.SendMessageUpwards("SetDifficultyLevel", currentDifficulty);
                currentPlayer.SendMessageUpwards("SetLivesUI", currentPlayerLives);
            }
        }

        foreach (GameObject enemyObject in allEnemies)
        {
            enemyObject.SendMessageUpwards("SetGameObjects", new GameObject[] { gameObject, currentPlayer });
            enemyObject.SendMessageUpwards("SetDifficultyLevel", currentDifficulty);
        }

        pointsUI = currentPlayer.transform.GetChild(2).GetChild(1).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
        countdownUI = transform.GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();

        //Save point total across levels
        pointsUI.text = totalPoints + " pts";
    }

    // Update is called once per frame
    void Update()
    {
        if (currentPlayer)
        {
            countdownUI.text = "";
            playerLifeTimer += Time.deltaTime;

            if (wonLevel)
            {
                if (currentWinTime > playerCelebrateTime)
                {
                    if (currentLevelNumber % 5 == 0 && currentPlayerLives >= totalPlayerLives)
                    {
                        currentPlayerLives++;
                    }

                    countdownUI.text = currentPlayerLives + " / " + totalPlayerLives;
                    Debug.Log("Total: " + SceneManager.sceneCountInBuildSettings);

                    // If in training mode, show the difficulty survey
                    // Instantiate the survey as a UI element
                    Canvas canvas = FindObjectOfType<Canvas>();
                    if (canvas != null)
                    {
                        activeSurvey = Instantiate(difficultySurveyPrefab, canvas.transform);
                    }
                    else
                    {
                        activeSurvey = Instantiate(difficultySurveyPrefab);
                    }
                    Debug.Log("Survey instantiated: " + (activeSurvey != null));

                    // Make sure the survey is visible in the scene
                    RectTransform surveyRect = activeSurvey.GetComponent<RectTransform>();
                    if (surveyRect != null)
                    {
                        surveyRect.anchoredPosition = Vector2.zero;
                        surveyRect.sizeDelta = new Vector2(1920, 1080); // Standard UI size
                    }

                    // Set up the survey with save file data
                    string saveFileData = WebGLSaveSystem.ReadAllText(saveFilePath);
                    string[] fileDataArray = saveFileData.Split('\n');

                    // Get the SurveyScript component
                    SurveyScript surveyScript = activeSurvey.GetComponent<SurveyScript>();
                    if (surveyScript != null)
                    {
                        Debug.Log("SurveyScript component found");
                        surveyScript.SetFileData(fileDataArray);
                        surveyScript.SetNextLevel(currentLevelNumber + 1);
                    }
                    else
                    {
                        Debug.LogError("SurveyScript component not found on the survey prefab");
                    }

                    // Pause the progression to next level until survey is completed
                    return;
                }

                if (currentLevelNumber + 1 == SceneManager.sceneCountInBuildSettings - 1 && isTrainingMode)
                {
                    int newDifficulty = currentDifficulty + 1;

                    bool fileExists = WebGLSaveSystem.FileExists(aiTrainingFilePath);
                    string csvLine = currentPlayerLives + "," + currentLevelNumber + "," + totalPoints + "," +
                        totalEnemiesKilled + "," + currentDifficulty + "," + playerLifeTimer + "," + newDifficulty;

                    if (!fileExists)
                    {
                        string header = "currentPlayerLives,currentLevelNumber,totalPoints,totalEnemiesKilled,currentDifficulty,playerLifeTimer,newDifficulty";
                        WebGLSaveSystem.WriteAllText(aiTrainingFilePath, header + "\n" + csvLine);
                    }
                    else
                    {
                        string existingData = WebGLSaveSystem.ReadAllText(aiTrainingFilePath);
                        WebGLSaveSystem.WriteAllText(aiTrainingFilePath, existingData + "\n" + csvLine);
                    }
                }

                //Load Next Level:
                CreateSave_LevelEnd(currentLevelNumber + 1);
                SceneManager.LoadScene(currentLevelNumber + 1, LoadSceneMode.Single);
            }
            else
            {
                //Celebrate Time!!!!:
                currentWinTime += Time.deltaTime;

                if (currentWinTime < 1f)
                {
                    countdownUI.text = "You Won! :)";
                }
                else if (currentWinTime < playerCelebrateTime * 0.5f && currentLevelNumber % 5 == 0 && currentPlayerLives >= totalPlayerLives)
                {
                    countdownUI.text = "Lives Increased";
                }
                else
                {
                    countdownUI.text = Mathf.Round(playerCelebrateTime - currentWinTime) + "";
                }
            }
        }
        else
        {
            countdownUI.text = Mathf.Round(playerRespawnTime - currentDeadTime) + "";
            if (currentDeadTime > playerRespawnTime)
            {
                //Loading:
                countdownUI.text = "Loading...";

                /*
                //Send to AI?:
                sendToAI()
                */

                currentPlayerLives -= 1;

                if (currentPlayerLives > 0)
                {
                    countdownUI.text = currentPlayerLives + " / " + totalPlayerLives;
                    CreateSave_LevelEnd(currentLevelNumber);
                    SceneManager.LoadScene(currentLevelNumber, LoadSceneMode.Single);
                }
                else
                {
                    if (isTrainingMode)
                    {
                        int newDifficulty = currentDifficulty - 1;
                        if (newDifficulty < 0)
                        {
                            newDifficulty = 0;
                        }

                        bool fileExists = WebGLSaveSystem.FileExists(aiTrainingFilePath);
                        string csvLine = currentPlayerLives + "," + currentLevelNumber + "," + totalPoints + "," +
                            totalEnemiesKilled + "," + currentDifficulty + "," + playerLifeTimer + "," + newDifficulty;

                        if (!fileExists)
                        {
                            string header = "currentPlayerLives,currentLevelNumber,totalPoints,totalEnemiesKilled,currentDifficulty,playerLifeTimer,newDifficulty";
                            WebGLSaveSystem.WriteAllText(aiTrainingFilePath, header + "\n" + csvLine);
                        }
                        else
                        {
                            string existingData = WebGLSaveSystem.ReadAllText(aiTrainingFilePath);
                            WebGLSaveSystem.WriteAllText(aiTrainingFilePath, existingData + "\n" + csvLine);
                        }
                    }

                    //Go Back to First Level:
                    if (currentLevelNumber < numberLevelsCheckpoint)
                    {
                        ResetSaveFile();
                        SceneManager.LoadScene(1, LoadSceneMode.Single);
                    }
                    else
                    {
                        ResetSaveFile(currentLevelNumber - (currentLevelNumber % numberLevelsCheckpoint));
                        SceneManager.LoadScene(currentLevelNumber - (currentLevelNumber % numberLevelsCheckpoint), LoadSceneMode.Single);
                    }
                }
            }
            else
            {
                //Enemy Dancing Time:
                currentDeadTime += Time.deltaTime;

                if (currentDeadTime < 1f)
                {
                    countdownUI.text = "You Lost. :(";
                }
                else
                {
                    countdownUI.text = Mathf.Round(playerRespawnTime - currentDeadTime) + "";
                }
            }
        }
    }
}


