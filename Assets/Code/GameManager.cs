using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public int score;
    public int gold;
    public float spawnFrequency;

    public UnityEvent<int> OnScoreChange;
    public UnityEvent<int> OnGoldChange;

    [Header("References")]
    public BoardManager boardManager;
    public AbilityFactory abilityFactory;
    public ContractManager contractManager;
    public EventHubSO eventHub;

    [Header("Testing")]
    public TowerSO redColorChangeTower;
    public TowerSO blueColorChangeTower;
    public TowerSO shiftUpTower;
    public TowerSO shiftDownTower;
    public TowerSO flipTower;
    public TowerSO squareBottleTower;
    public TowerSO diamondBottleTower;

    float timer;
    int spawnIndex = 0;

    public void ChangeScore(int delta)
    {
        score = Mathf.Clamp(score + delta, 0, int.MaxValue);
        OnScoreChange.Invoke(score);
    }

    public void ChangeGold(int delta)
    {
        gold = Mathf.Clamp(gold + delta, 0, int.MaxValue);
        OnGoldChange.Invoke(gold);
    }

    private void Start()
    {
        boardManager.AddTower(2, 2, redColorChangeTower, abilityFactory);

        boardManager.AddTower(0, 0, shiftDownTower, abilityFactory);
        boardManager.AddTower(1, 1, shiftDownTower, abilityFactory);

        boardManager.AddTower(4, 0, shiftUpTower, abilityFactory);
        boardManager.AddTower(3, 1, shiftUpTower, abilityFactory);

        boardManager.AddTower(2, 3, flipTower, abilityFactory);
        boardManager.AddTower(1, 4, diamondBottleTower, abilityFactory);
        boardManager.AddTower(3, 4, squareBottleTower, abilityFactory);

    }

    public void Tick(float delta)
    {
        timer += Time.deltaTime;
        if (timer > spawnFrequency)
        {
            boardManager.SpawnPotion(spawnIndex, "PotionCollect", abilityFactory);

            spawnIndex = (spawnIndex + 1) % boardManager.belts.Count;
            timer = 0;
        }

        boardManager.Tick(delta);
    }


    public static int CalculateScoreFromPotion(Potion potion)
    {
        return potion.CurrentScore * potion.CurrentMult;
    }

    public void ChangePotionFlavor(Potion potion, Flavor flavor)
    {
        potion.Flavors = flavor;
        potion.UpdateVisual();

        eventHub.Instance.Publish(new PotionChangeFlavorEvent(flavor, potion));
    }

    public void ChangePotionBottle(Potion potion, Bottle bottle)
    {
        var oldType = potion.Bottle;

        potion.Bottle = bottle;
        potion.UpdateVisual();

        eventHub.Instance.Publish(new PotionChangeBottleEvent(oldType, bottle, potion));
    }

    public void ShiftPotion(Potion potion, int amount)
    {
        if (!boardManager.TryGetBeltIndexFromPotion(potion, out var index))
            return;

        var newBeltIndex = index + amount;
        if (newBeltIndex < 0 || newBeltIndex >= boardManager.belts.Count)
            return;

        boardManager.ShiftPotionToBelt(potion, newBeltIndex);

        eventHub.Instance.Publish(new PotionShiftEvent(index, newBeltIndex, potion));
    }

    public void AddTower(int beltId, int tileId, TowerSO towerData)
    {
        boardManager.AddTower(beltId, tileId, towerData, abilityFactory);
    }

    public void RemoveTower(int beltId, int tileId)
    {
        boardManager.RemoveTower(beltId, tileId);
    }
}
