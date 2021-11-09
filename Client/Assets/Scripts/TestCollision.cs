using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TestCollision : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase tile;

    void Start()
    {
        tilemap.SetTile(new Vector3Int(0, 0, 0), tile);
    }

    void Update()
    {
        List<Vector3Int> blocked = new List<Vector3Int>();

        foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(pos);
            if (tile != null)
                blocked.Add(pos);
        }
    }
}
