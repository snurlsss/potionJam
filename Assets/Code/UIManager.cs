using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TMP_Text scoreText;
    public TMP_Text goldText;
    public TMP_Text clockText;

    public void SetScore(int value)
    {
        scoreText.text = $"Score: {value}";
    }

    public void SetGold(int value)
    {
        goldText.text = $"Gold: {value}";
    }

    public void SetTime(float time)
    {
        clockText.text = $"{time.ToString("F1")}s";
    }
}
