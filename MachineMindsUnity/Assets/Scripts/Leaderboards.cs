using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Leaderboards : MonoBehaviour
{
    private const int MAX_ENTRIES = 10;
    public TMPro.TextMeshProUGUI leaderboardText;

    public TMPro.TextMeshProUGUI rankList;
    public TMPro.TextMeshProUGUI scoreList;
    public TMPro.TextMeshProUGUI dateList;

    void Start()
    {
        StartCoroutine(LoadLeaderboards());
    }

    private IEnumerator LoadLeaderboards()
    {
        List<LeaderboardEntry> scores = null;
        yield return StartCoroutine(SaveSystem.GetLeaderboards((entries) =>
        {
            scores = entries;
        }));

        if (scores != null)
        {
            DisplayLeaderboards(scores);
        }
        else
        {
            Debug.LogError("Leaderboards: Received null scores list");
        }
    }

    private void DisplayLeaderboards(List<LeaderboardEntry> entries)
{
    leaderboardText.text = "";
    rankList.text    = "";
    scoreList.text   = "";
    dateList.text    = "";

    foreach (var entry in entries)
    {
        if (entry.rank > MAX_ENTRIES) 
            continue;

        // 1) turn your float into an int (drop or round fractional points as desired)
        int scoreInt = Mathf.RoundToInt(entry.score);

        // 2) if it's under one million, pad to 7 digits with leading zeros;
        //    otherwise just use the normal decimal string
        string scoreStr = scoreInt < 1_000_000
            ? scoreInt.ToString("D7")  // e.g. 0001234
            : scoreInt.ToString();     // e.g. 1000000 or above

        // 3) build your line however you're doing it
        leaderboardText.text += $"{entry.rank}\t{scoreStr}\t{entry.date}\n";
        // (or, if you're using the separate columns:)
        // rankList.text  += entry.rank + "\n";
        // scoreList.text += scoreStr    + "\n";
        // dateList.text  += entry.date  + "\n";
    }
}


    public void exitMenu()
    {
        Destroy(gameObject);
    }
}

[System.Serializable]
public class LeaderboardEntry
{
    public int rank;
    public float score;
    public string date;
}
