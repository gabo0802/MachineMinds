using System.Diagnostics;
using System;
using UnityEngine;

using static EnemyBehavior;

public class UnitTests : MonoBehaviour
{
    public void Test_One(int newDifficultyLevel){
        EnemyBehavior e = new EnemyBehavior();
        e.SetDifficultyLevel(newDifficultyLevel);
        System.Diagnostics.Debug.Assert(e.currentDifficulty == newDifficultyLevel);
    }
}