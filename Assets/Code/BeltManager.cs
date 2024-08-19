using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltManager : MonoBehaviour
{
    public int beltNum;
    public List<Tile> tiles;
    public Transform start;
    public Transform end;

    List<float> progressPoints = new();

    private void Awake()
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            var tile = tiles[i];

            tile.beltId = beltNum;
            tile.tileId = i;
            tile.name = $"Tile: {beltNum} - {i}";
        }

        LoadProgressPoints();
    }



    void LoadProgressPoints()
    {
        var baseDistance = Vector3.Distance(start.position, end.position);
        foreach(var tile in tiles)
        {
            var distanceToTile = Vector3.Distance(start.position, tile.transform.position);
            progressPoints.Add(distanceToTile / baseDistance);
        }
    }

    public bool TryGetTile(int index, out Tile tile)
    {
        if(index < tiles.Count)
        {
            tile = tiles[index];
            return true;
        }

        tile = null;
        return false;
    }

    public void GetPassedTilesInProgressRange(float startingProgress, float endProgress, List<Tile> buffer)
    {
        for (int i = 0; i < progressPoints.Count; i++)
        {
            var point = progressPoints[i];
            if (point >= startingProgress && point < endProgress)
            {
                buffer.Add(tiles[i]);
            }

            if (point >= endProgress)
                break;
        }
    }

    public Vector3 GetPositionForProgress(float progress)
    {
        return Vector3.Lerp(start.position, end.position, progress);
    }
}
