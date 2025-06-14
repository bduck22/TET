using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }

    public TetrominoData[] tetrominoes;
    public Vector2Int boardSize = new Vector2Int(10, 20);
    public Vector3Int spawnPosition = new Vector3Int(-1, 8, 0);
    public List<int> blocks = new List<int>{
        0, 1, 2, 3, 4, 5, 6, 7
    };

    public int Score=0;
    public int HighScore = 0;
    public int Combo = 0;

    public int NextTile;

    public PreView preView;

    public int Hold=-1;

    public Hold hold;
    public RectInt Bounds 
    {
        get
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }

    private void Awake()
    {
        //PlayerPrefs.DeleteAll();//전체 삭제
        tilemap = GetComponentInChildren<Tilemap>();
        activePiece = GetComponentInChildren<Piece>();

        for (int i = 0; i < tetrominoes.Length; i++) 
        {
            tetrominoes[i].Initialize();
        }
        int R = Random.Range(0, blocks.Count);
        NextTile = blocks[R];
        blocks.Remove(R);
        tetrominoes[7].tile = tetrominoes[Data.TileNumber].tile;
        StageInit();
    }

    private void Start()
    {
        SpawnPiece();
    }

    public void SpawnPiece()
    {
        if (Hold != -1)
        {
            hold.Clear();
            hold.Load(tetrominoes[Hold]);
        }

        TetrominoData data = tetrominoes[NextTile];
        if (blocks.Count == 0)
        {
            blocks = new List<int>{
                0, 1, 2, 3, 4, 5, 6, 7
            };
        }
        int R = Random.Range(0, blocks.Count);
        NextTile = blocks[R];
        blocks.RemoveAt(R);
        if (data.cells == null)
        {
            SpawnPiece();
        }
        else
        {
            preView.Clear();
            preView.Load(tetrominoes[NextTile]);

            activePiece.Initialize(this, spawnPosition, data);

            if (IsValidPosition(activePiece, spawnPosition))
            {
                Set(activePiece);
            }
            else
            {
                GameOver();
            }
        }
    }

    public void SpawnPiece(int TileNum)
    {
        hold.Clear();
        hold.Load(tetrominoes[Hold]);

        TetrominoData data = tetrominoes[TileNum];

        activePiece.Initialize(this, spawnPosition, data);

        if (IsValidPosition(activePiece, spawnPosition))
        {
            Set(activePiece);
        }
        else
        {
            GameOver();
        }
    }

    public void GameOver()
    {


        tilemap.ClearAllTiles();
        StageInit();
        // Do anything else you want on game over here..
    }

    public void StageInit()
    {
        Score = 0;
        HighScore = PlayerPrefs.GetInt("HighScore");
        Combo = 0;
        Hold = -1;
    }

    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }

    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = Bounds;

        // The position is only valid if every cell is valid
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;

            // An out of bounds tile is invalid
            if (!bounds.Contains((Vector2Int)tilePosition)) 
            {
                return false;
            }

            // A tile already occupies the position, thus invalid
            if (tilemap.HasTile(tilePosition)) 
            {
                return false;
            }
        }

        return true;
    }

    public void ClearLines()
    {
        RectInt bounds = Bounds;
        int row = bounds.yMin;

        // Clear from bottom to top
        int plus=0;
        while (row < bounds.yMax)
        {
            // Only advance to the next row if the current is not cleared
            // because the tiles above will fall down when a row is cleared
            if (IsLineFull(row)) 
            {
                plus++;
                LineClear(row);
            } 
            else 
            {
                row++;
            }
        }
        if(plus > 0)
        {
            Score += (plus*2-1 + (plus / 4)+Combo) * 100;
            Combo++;
        }
        else
        {
            Combo = 0;
        }


        if (IsLineEmpty(-10))
        {
            Score += 1000;
        }
        if(Score > HighScore)
        {
            HighScore = Score;
        }
        PlayerPrefs.SetInt("HighScore", HighScore);
    }
    public bool IsLineEmpty(int row)
    {
        RectInt bounds = Bounds;
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            if (tilemap.HasTile(position))
            {
                return false;
            }
        }

        return true;
    }

    public bool IsLineFull(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            // The line is not full if a tile is missing
            if (!tilemap.HasTile(position)) {
                return false;
            }
        }

        return true;
    }

    public void LineClear(int row)
    {
        RectInt bounds = Bounds;

        // Clear all tiles in the row
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            tilemap.SetTile(position, null);
        }

        // Shift every row above down one
        while (row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row + 1, 0);
                TileBase above = tilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                tilemap.SetTile(position, above);
            }

            row++;
        }
    }
}
