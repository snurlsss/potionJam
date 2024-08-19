using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameDirector : MonoBehaviour
{
    public float roundTime;
    public int currentRound = 1;
    public int baseRoundScore;
    public int roundScoreGrowth;
    public int currentScoreRequirement;

    [Header("Debug")]
    public bool progressingRound;

    [Header("References")]
    public GameManager gameManager;
    public UIManager uiManager;
    float timer;

    private void Update()
    {
        if (!progressingRound)
            return;

        timer = Mathf.Clamp(timer - Time.deltaTime, 0, roundTime);
        uiManager.SetTime(timer);

        gameManager.Tick(Time.deltaTime);

        if(timer == 0)
        {
            EndRound();
        }
    }

    [Button]
    public void BeginRound()
    {
        timer = roundTime;
        progressingRound = true;

        currentScoreRequirement = baseRoundScore + (roundScoreGrowth * (currentRound - 1));
    }
    public void EndRound()
    {
        gameManager.boardManager.ClearPotions();
        progressingRound = false;

        if (gameManager.score >= currentScoreRequirement)
            Debug.Log("Keep going shitter");
        else
            Debug.Log("Lose");
    }

    public void ConvertScoreToGold()
    {

    }

}
