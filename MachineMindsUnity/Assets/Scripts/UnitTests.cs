using UnityEngine;
using System.Diagnostics;
using System;
using static EnemyBehavior;

public class UnitTests : MonoBehaviour
{
    public void Test_One(int newDifficultyLevel){
        EnemyBehavior e = new EnemyBehavior();
        e.SetDifficultyLevel(newDifficultyLevel);
        //System.Diagnostics.Debug.Assert(e.currentDifficulty == newDifficultyLevel);
        UnityEngine.Debug.Assert(e.currentDifficulty == newDifficultyLevel);
    }

    static void Main(string[] args){
        UnitTests u = new UnitTests();
        u.Test_One(1);
    }

    void Start(){
        Test_One(5);
    }
}