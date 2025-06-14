using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using static Unity.Burst.Intrinsics.X86;

[System.Serializable]
public class Save
{
    public Vector2Int[] cells;
    public int seletedtile;
    public bool Middle;
}
public class MakeTile : MonoBehaviour
{
    public Tilemap tilemap;
    public Tilemap gtilemap;
    public Tile[] tile;
    public Tile Ghosttile;
    public int seletedtile;
    public Image UI;
    public int TileCount;
    private Vector3Int Ghost;

    public bool Middle;

    string path;

    public Toggle Toggle;
    public Image tileImage;
    void Start()
    {
        path = Path.Combine(Application.dataPath, "CustomData.json");
        LoadTile();
        UILoad();
    }
    public void Uptile()
    {
        if (++seletedtile >= tile.Length)
        {
            seletedtile = 0;
        }
        UILoad();
    }
    public void Downtile()
    {
        if (--seletedtile < 0)
        {
            seletedtile = tile.Length - 1;
        }
        UILoad();
    }
    private void UILoad()
    {
        UI.sprite = tile[seletedtile].sprite;
    }

    void Update()
    {
        Vector2 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int Position = new Vector3Int(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y));

        if (Check(Position))
        {
            if (Ghost != new Vector3Int(10, 10, 10))
            {
               gtilemap.SetTile(Ghost, null);
            }
            if (IsLineCount() < TileCount)
            {
                if (!tilemap.HasTile(Position))
                {
                    gtilemap.SetTile(Position, Ghosttile);
                    Ghost = Position;
                }
                if (Input.GetMouseButtonDown(0))
                {
                    tilemap.SetTile(Position, tile[seletedtile]);
                    gtilemap.SetTile(Ghost, null);
                    Ghost = new Vector3Int(10, 10, 10);
                }
            }
            if (Input.GetMouseButtonDown(1))
            {
                tilemap.SetTile(Position, null);
            }
        }
        else
        {
            gtilemap.SetTile(Ghost, null);
            Ghost = new Vector3Int(10, 10, 10);
        }
    }
    public int IsLineCount()
    {
        int count = 0;
        for (int row = -2; row < 2; row++)
        {
            for (int col = -3; col < 3; col++)
            {
                Vector3Int position = new Vector3Int(col, row, 0);

                if (tilemap.HasTile(position))
                {
                    tilemap.SetTile(position,tile[seletedtile]);
                    count++;
                }
            }
        }
        return count;
    }
    bool Check(Vector3Int positon)
    {
        if (positon.x < 3 && positon.x >= -3 && positon.y >= -2 && positon.y < 2)
        {
            return true;
        }
        else return false;
    }
    public void SaveTile()
    {
        Vector2Int[] cells = new Vector2Int[IsLineCount()];
        int i = 0;
        for (int row = -2; row < 2; row++)
        {
            for (int col = -3; col < 3; col++)
            {
                Vector3Int position = new Vector3Int(col, row, 0);

                if (tilemap.HasTile(position))
                {
                    cells[i++] = new Vector2Int(col+1, row+1);
                }
            }
        }
        Data.TileNumber = seletedtile;
        Data.Cells[Tetromino.C]= cells;
        if (Middle)
        {
            Data.WallKicks[Tetromino.C] = Data.WallKicksI;
        }
        else
        {
            Data.WallKicks[Tetromino.C] = Data.WallKicksJLOSTZ;
        }
        Save save = new Save();
        save.Middle = Middle;
        save.seletedtile = seletedtile;
        save.cells = cells;

        string json = JsonUtility.ToJson(save, true);

        File.WriteAllText(path, json);
    }

    public void LoadTile()
    {
        Vector2Int[] cells = {new Vector2Int(0,0) };
        Save save = new Save();
        if (File.Exists(path))
        {
            string loadJson = File.ReadAllText(path);
            save = JsonUtility.FromJson<Save>(loadJson);

            if (save != null)
            {
                seletedtile = save.seletedtile;
                cells = save.cells;
            }
        }

        Data.TileNumber = seletedtile;
        Data.Cells[Tetromino.C] = cells;
        if (Middle)
        {
            Data.WallKicks[Tetromino.C] = Data.WallKicksI;
        }
        else
        {
            Data.WallKicks[Tetromino.C] = Data.WallKicksJLOSTZ;
        }

        if (save.Middle)
        {
            Toggle.isOn = save.Middle;
        }


        tileImage.sprite = tile[seletedtile].sprite;
        foreach (Vector2Int vector in cells)
        {
            Vector3Int vector3 = new Vector3Int(vector.x-1, vector.y-1,0);
            tilemap.SetTile(vector3, tile[seletedtile]);
        }
    }
    public void Clear()
    {
        tilemap.ClearAllTiles();
    }
    public void middle()
    {
        Middle = !Middle;
    }
    public void start()
    {
        SceneManager.LoadScene(1);
    }
}
