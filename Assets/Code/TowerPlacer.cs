using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerPlacer : MonoBehaviour
{
    public GameManager gameManager;
    public TowerSO tower;

    public Tile currentHoveredTile;
    GameObject spawnedVisual;

    private void Update()
    {
        ShowGhost();

        if (currentHoveredTile)
        {
            if (Input.GetMouseButtonDown(0))
            {
                gameManager.AddTower(currentHoveredTile.beltId, currentHoveredTile.tileId, tower);
            }

            if (Input.GetMouseButtonDown(1) && currentHoveredTile.HasTower())
            {
                gameManager.RemoveTower(currentHoveredTile.beltId, currentHoveredTile.tileId);
            }
        }
    }

    public void ShowGhost()
    {
        var point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var collider = Physics2D.OverlapPoint(point);

        if (!collider || !collider.TryGetComponent(out Tile tile))
        {
            currentHoveredTile = null;
            Clear();
            return;
        }

        if (tile == currentHoveredTile)
            return;

        currentHoveredTile = tile;

        if (tile.HasTower())
        {
            Clear();
            return;
        }

        ShowTowerVisual(tile);
    }

    public void ShowTowerVisual(Tile tile)
    {
        if (spawnedVisual)
            Destroy(spawnedVisual);

        Debug.Log("creating ghost");
        spawnedVisual = Instantiate(tower.visual, tile.towerParent);
    }

    public void Clear()
    {
        if (spawnedVisual)
            Destroy(spawnedVisual);
    }

}
