using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Hold : MonoBehaviour
{
    public Vector3Int[] cells;
    public Tilemap tilemap;
    void Start()
    {
        tilemap = GetComponentInChildren<Tilemap>();
    }

    public void Load(TetrominoData HoldTile)
    {
        TetrominoData data = HoldTile;

        cells = new Vector3Int[data.cells.Length];

        for (int i = 0; i < cells.Length; i++)
        {
            cells[i] = (Vector3Int)data.cells[i];
        }

        for (int i = 0; i < cells.Length; i++)
        {
            Vector3Int tilePosition = cells[i];
            tilemap.SetTile(tilePosition, data.tile);
        }
    }
    public void Clear()
    {
        if (cells != null)
        {
            for (int i = 0; i < cells.Length; i++)
            {
                Vector3Int tilePosition = cells[i];
                tilemap.SetTile(tilePosition, null);
            }
        }
    }
}
