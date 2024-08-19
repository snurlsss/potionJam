using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public float progressSpeed;
    public int potionBaseValue = 10;

    public List<Potion> spawnedPotions;
    public Dictionary<Potion, BeltManager> potionToBelt = new();

    [Header("References")]
    public EventHubSO eventHub;
    public Potion potionPrefab;
    public Transform potionParent;
    public List<BeltManager> belts;

    List<Tile> passedBuffer = new(10);
    List<Potion> completedBuffer = new(10);

    public bool GetTile(int beltIndex, int tileIndex, out Tile tile)
    {
        if(beltIndex < belts.Count)
        {
            return belts[beltIndex].TryGetTile(tileIndex, out tile);
        }

        tile = null;
        return false;
    }

    public void Tick(float delta)
    {
        foreach (var potion in spawnedPotions)
        {
            if (potion.progress >= 1)
                completedBuffer.Add(potion);
        }

        foreach (var completedPotion in completedBuffer)
        {
            spawnedPotions.Remove(completedPotion);
            potionToBelt.Remove(completedPotion);

            completedPotion.onCollectAbility?.ExecuteActives(new ActivationContext(completedPotion.gameObject, gameObject));

            eventHub.Instance.Publish(new PotionCollectEvent(completedPotion.gameObject));

            Destroy(completedPotion.gameObject);
        }

        completedBuffer.Clear();

        foreach (var potion in spawnedPotions)
        {
            var belt = potionToBelt[potion];
            var currentProgress = potion.progress;
            var newProgress = Mathf.Clamp(currentProgress + (progressSpeed * Time.deltaTime), 0, 1);

            belt.GetPassedTilesInProgressRange(currentProgress, newProgress, passedBuffer);

            foreach (var passed in passedBuffer)
            {
                passed.currentAbility?.ExecuteActives(new ActivationContext(potion.gameObject, passed.gameObject));

                eventHub.Instance.Publish(new PassedEvent(potion.gameObject, passed));
            }

            passedBuffer.Clear();

            potion.progress = newProgress;
            potion.transform.position = belt.GetPositionForProgress(newProgress);
        }
    }

    public void SpawnPotion(int beltIndex, string collectAbilityId, AbilityFactory abilityFactory)
    {
        var spawnedPotion = Instantiate(potionPrefab, potionParent);
        spawnedPotion.CurrentScore = potionBaseValue;

        if(abilityFactory.TryGetAbilityFromID(collectAbilityId, out Ability ability))
            spawnedPotion.Initialize(ability);

        var belt = belts[beltIndex];
        Vector3 spawnPos = belt.GetPositionForProgress(spawnedPotion.progress);
        spawnedPotion.transform.position = spawnPos;

        spawnedPotions.Add(spawnedPotion);
        potionToBelt[spawnedPotion] = belt;
    }

    public void ClearPotions()
    {
        foreach(var potion in spawnedPotions)
        {
            potionToBelt.Remove(potion);
            Destroy(potion.gameObject);
        }

        spawnedPotions.Clear();
    }

    public void ShiftPotionToBelt(Potion potion, int beltIndex)
    {
        if (!spawnedPotions.Contains(potion))
            return;

        if (beltIndex >= belts.Count)
            return;

        potionToBelt[potion] = belts[beltIndex];
    }

    public bool TryGetBeltIndexFromPotion(Potion potion, out int index)
    {
        if (!spawnedPotions.Contains(potion))
        {
            index = -1;
            return false;
        }

        var belt = potionToBelt[potion];
        index = belts.IndexOf(belt);
        return true;
    }

    public void AddTower(int beltIndex, int tileIndex, TowerSO towerData, AbilityFactory abilitySource)
    {
        if (!GetTile(beltIndex, tileIndex, out Tile tile))
            return;

        tile.AttachTower(towerData, abilitySource);
    }

    public void RemoveTower(int beltIndex, int tileIndex)
    {
        if (!GetTile(beltIndex, tileIndex, out Tile tile))
            return;

        tile.RemoveTower();
    }

    public static float CalculateScoreFromPotion(Potion potion)
    {
        return potion.CurrentScore * potion.CurrentMult;
    }
}
