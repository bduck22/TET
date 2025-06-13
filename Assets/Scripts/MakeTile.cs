using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

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
    void Start()
    {
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
        //JsonUtility.ToJson
    }

    public void LoadTile()
    {

    }
    public void Clear()
    {
        tilemap.ClearAllTiles();
    }
    public void middle()
    {
        Middle = !Middle;
    }
    public void Exit()
    {
        SceneManager.LoadScene(0);
    }
}
