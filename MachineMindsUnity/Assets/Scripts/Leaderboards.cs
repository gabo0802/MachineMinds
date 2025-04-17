using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Leaderboards : MonoBehaviour
{
    private const int MAX_ENTRIES = 5;
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
        rankList.text = "";
        scoreList.text = "";
        dateList.text = "";

        foreach (var entry in entries)
        {
            if (entry.rank <= MAX_ENTRIES)
            {
                leaderboardText.text += $"{entry.rank}\t{entry.score}\t{entry.date}\n";
                
            }
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
