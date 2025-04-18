using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// Retrieves and displays top leaderboard entries with rank, score, and date columns.
/// </summary>
public class Leaderboards : MonoBehaviour
{
    private const int MAX_ENTRIES = 5;
    public TMPro.TextMeshProUGUI leaderboardText;
    public TMPro.TextMeshProUGUI rankList;
    public TMPro.TextMeshProUGUI scoreList;
    public TMPro.TextMeshProUGUI dateList;

    /// <summary>
    /// Unity Start: begins asynchronous loading of leaderboard data.
    /// </summary>
    void Start()
    {
        StartCoroutine(LoadLeaderboards());
    }

    /// <summary>
    /// Coroutine that fetches leaderboard entries via SaveSystem,
    /// then invokes display logic upon successful retrieval.
    /// </summary>
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

    /// <summary>
    /// Populates the UI text fields with the top entries up to MAX_ENTRIES.
    /// </summary>
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

    /// <summary>
    /// Closes the leaderboard UI when exit is requested.
    /// </summary>
    public void exitMenu()
    {
        Destroy(gameObject);
    }
}

/// <summary>
/// Data model representing a single leaderboard entry.
/// </summary>
[System.Serializable]
public class LeaderboardEntry
{
    public int rank;
    public float score;
    public string date;
}
