using UnityEngine;
using UnityEngine.SceneManagement;

public class SurveyScript : MonoBehaviour
{
    private string saveFilePath = "GameState";

    private string aiTrainingFilePath = "GameData";

    private string[] fileData;
    private int currentDifficulty = 1;
    private int nextLevel = 1;
    private const int maxDifficulty = 10;


    public TMPro.TextMeshProUGUI contextMessage;


    //Functions:
    private string[] getFileData(string filePath)
    {
        if (PlayerPrefs.HasKey(filePath))
        {
            string saveFileData = PlayerPrefs.GetString(filePath);
            return saveFileData.Split('\n');
        }
        return new string[0];
    }

    private void writeFileData(string filePath, string[] newFileData, bool overwriteExistingFileData)
    {
        string content = "";
        
        if (!overwriteExistingFileData && PlayerPrefs.HasKey(filePath))
        {
            content = PlayerPrefs.GetString(filePath);
            if (!content.EndsWith("\n") && content.Length > 0)
            {
                content += "\n";
            }
        }
        
        for (int i = 0; i < newFileData.Length; i++)
        {
            content += newFileData[i];
            if (i < newFileData.Length - 1)
            {
                content += "\n";
            }
        }
        
        PlayerPrefs.SetString(filePath, content);
        PlayerPrefs.Save();
    }

    private void saveAITrainingData(int newDifficulty)
    {
        if (newDifficulty < 1)
        {
            newDifficulty = 1;
        }
        else if (newDifficulty > maxDifficulty)
        {
            newDifficulty = maxDifficulty;
        }

        if (!PlayerPrefs.HasKey(aiTrainingFilePath))
        {
            writeFileData(aiTrainingFilePath, new string[]{
                "currentPlayerLives,totalPoints,totalEnemiesKilled,currentDifficulty,playerLifeTimer,levelsBeat,newDifficulty",
                fileData[0]+","+fileData[1]+","+fileData[2]+","+fileData[3]+"," +fileData[4]+","+fileData[5]+","+newDifficulty
            }, true);
        }
        else
        {
            writeFileData(aiTrainingFilePath, new string[]{
                fileData[0]+","+fileData[1]+","+fileData[2]+","+fileData[3]+","+fileData[4]+","+fileData[5]+","+newDifficulty
            }, false);
        }
    }

    private void adjustDifficulty(int newDifficulty)
    {
        string[] currentFileData = getFileData(saveFilePath);

        // Ensure we have enough data in the file
        if (currentFileData.Length < 4)
        {
            Debug.LogWarning("Save file does not contain enough data");
            return;
        }

        if (newDifficulty < 1)
        {
            newDifficulty = 1;
        }
        else if (newDifficulty > maxDifficulty)
        {
            newDifficulty = maxDifficulty;
        }

        writeFileData(saveFilePath, new string[]{
            currentFileData[0], //currentPlayerLives
            currentFileData[1], //totalPoints
            currentFileData[2], //totalEnemiesKilled
            newDifficulty + "",  //currentDifficulty
            currentFileData[3],  //playerLifeTimer
            true + "" //isTrainingMode
        }, true);
    }

    public void SetNextLevel(int newNextLevel)
    {
        nextLevel = newNextLevel;
    }

    public void SetFileData(string[] newFileData)
    {
        fileData = newFileData;
        currentDifficulty = System.Int32.Parse(fileData[3]);
    }

    public void TooEasyAdjustment()
    {
        saveAITrainingData(currentDifficulty + 1);
        adjustDifficulty(currentDifficulty + 1);
        SceneManager.LoadScene(nextLevel, LoadSceneMode.Single);
        Destroy(gameObject);
    }

    public void JustRightAdjustment()
    {
        saveAITrainingData(currentDifficulty);
        SceneManager.LoadScene(nextLevel, LoadSceneMode.Single);
        Destroy(gameObject);
    }

    public void TooHardAdjustment()
    {
        saveAITrainingData(currentDifficulty - 1);
        adjustDifficulty(currentDifficulty - 1);
        SceneManager.LoadScene(nextLevel, LoadSceneMode.Single);
        Destroy(gameObject);
    }

    void Start()
    {
        contextMessage.text = "Current Difficulty Level: " + currentDifficulty;
    }
}
