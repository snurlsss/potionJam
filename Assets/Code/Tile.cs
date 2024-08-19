using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int beltId;
    public int tileId;
    public Ability currentAbility;
    public Transform towerParent;

    GameObject spawnedTower;

    public void AttachTower(TowerSO data, AbilityFactory abilityFactory)
    {
        RemoveTower();

        spawnedTower = Instantiate(data.visual, towerParent);
        abilityFactory.TryGetAbilityFromID(data.abilityId, out currentAbility);

        currentAbility?.Activate();
    }

    public void RemoveTower()
    {
        currentAbility?.Deactivate();
        currentAbility = null;

        if (spawnedTower)
            Destroy(spawnedTower);
    }

    public bool HasTower()
    {
        return spawnedTower;
    }
}
