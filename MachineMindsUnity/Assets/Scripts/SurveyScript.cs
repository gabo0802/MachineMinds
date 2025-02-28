using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class SurveyScript : MonoBehaviour{
    private string saveFilePath = Application.dataPath + "/Resources/GameState.save";
    private string aiTrainingFilePath = Application.dataPath + "/Resources/game_data.csv";

    private string[] fileData;
    private int currentDifficulty = 1;
    private int nextLevel = 1;

    public TMPro.TextMeshProUGUI contextMessage;


    //Functions:
    private string[] getFileData(string filePath){
        string saveFileData = "";

        using (StreamReader saveFile = File.OpenText(filePath)){
            string currentLine;
            
            while ((currentLine = saveFile.ReadLine()) != null){
                saveFileData += currentLine + "\n";
            }
         }
        
        return saveFileData.Split('\n');
    }

    private void writeFileData(string filePath, string[] newFileData, bool overwriteExistingFileData){
        if (overwriteExistingFileData){
            using (StreamWriter sw = File.CreateText(filePath)){
                for(int i = 0; i < newFileData.Length; i++){
                    sw.WriteLine(newFileData[i]);
                }
            }
        }else{
            using(TextWriter tw = new StreamWriter(filePath, true)){
                for(int i = 0; i < newFileData.Length; i++){
                    tw.WriteLine(newFileData[i]);
                }
            }
        }
    }

    private void saveAITrainingData(int newDifficulty){
        int maxDifficulty = 10;

        if(newDifficulty < 0){
            newDifficulty = 0;
        }else if(newDifficulty > maxDifficulty){
            newDifficulty = maxDifficulty;
        }

        if (!File.Exists(aiTrainingFilePath)){
            writeFileData(aiTrainingFilePath, new string[]{
                "currentPlayerLives,totalPoints,totalEnemiesKilled,currentDifficulty,playerLifeTimer,newDifficulty",
                fileData[0] + "," + fileData[1] + "," + fileData[2] + "," + fileData[3] + "," + fileData[4] + "," + newDifficulty
            }, true);
        }else{
            writeFileData(aiTrainingFilePath, new string[]{
                fileData[0] + "," + fileData[1] + "," + fileData[2] + "," + fileData[3] + "," + fileData[4] + "," + newDifficulty
            }, false);
        }
    }
    
    private void adjustDifficulty(int newDifficulty){
        string[] currentFileData = getFileData(saveFilePath);

        writeFileData(saveFilePath, new string[]{
            currentFileData[0], //currentPlayerLives
            currentFileData[1], //totalPoints
            currentFileData[2], //totalEnemiesKilled
            newDifficulty + "",  //currentDifficulty
            currentFileData[3],  //playerLifeTimer
            true + "" //isTrainingMode
        }, true);

        contextMessage.text = "How is Difficulty Level: " + currentDifficulty + "?";
    }

    public void SetNextLevel(int newNextLevel){
        nextLevel = newNextLevel;
    }

    public void SetFileData(string[] newFileData){
        fileData = newFileData;
        currentDifficulty = System.Int32.Parse(fileData[3]);

    }

    public void TooEasyAdjustment(){
        saveAITrainingData(currentDifficulty + 1);
        adjustDifficulty(currentDifficulty + 1);
        SceneManager.LoadScene(nextLevel, LoadSceneMode.Single);
        Destroy(gameObject);
    }

    public void JustRightAdjustment(){
        saveAITrainingData(currentDifficulty);
        SceneManager.LoadScene(nextLevel, LoadSceneMode.Single);
        Destroy(gameObject);
    }

    public void TooHardAdjustment(){
        saveAITrainingData(currentDifficulty - 1);
        adjustDifficulty(currentDifficulty - 1);
        SceneManager.LoadScene(nextLevel, LoadSceneMode.Single);
        Destroy(gameObject);
    }
}
