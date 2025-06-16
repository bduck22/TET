using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using DamageNumbersPro;
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

    public Transform pause;

    public Transform gameover;

    public Transform set;

    public bool gameOver;

    public Transform effectOb;

    public DamageNumber ComboPre;

    public DamageNumber ScorePre;
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
        tetrominoes[7].tile = tetrominoes[Data.TileNumber].tile;
        StageInit();
    }

    public void SpawnPiece()
    {
        if (Hold != -1)
        {
            hold.Clear();
            hold.Load(tetrominoes[Hold]);
        }

        TetrominoData data = tetrominoes[NextTile];
        if(blocks.Count == 0)
        {
            blocks = new List<int>{
            0, 1, 2, 3, 4, 5, 6, 7
        };
            if (Data.Cells[Tetromino.C].Length == 0) blocks.RemoveAt(7);
        }
        int R = Random.Range(0, blocks.Count);
        NextTile = blocks[R];
        blocks.RemoveAt(R);

            preView.Clear();
            preView.Load(tetrominoes[NextTile]);

            activePiece.Initialize(this, spawnPosition, data);

            if (IsValidPosition(activePiece, spawnPosition))
            {
                Set(activePiece);
            }
            else
            {
            GetComponent<AudioSource>().Stop();
            Time.timeScale = 0;
                gameOver = true;
                set.gameObject.SetActive(true);
                gameover.gameObject.SetActive(true);
                pause.gameObject.SetActive(false);
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
            GetComponent<AudioSource>().Stop();
            Time.timeScale = 0;
            gameOver = true;
            set.gameObject.SetActive(true);
            gameover.gameObject.SetActive(true);
            pause.gameObject.SetActive(false);
        }
    }
    bool p = false;
    public void Pause()
    {
        if (!gameOver)
        {
            p = !p;
            if (p)
            {
                GetComponent<AudioSource>().Stop();
                Time.timeScale = 0;
                set.gameObject.SetActive(true);
                gameover.gameObject.SetActive(false);
                pause.gameObject.SetActive(true);
            }
            else
            {
                GetComponent<AudioSource>().Play();
                Time.timeScale = 1;
                set.gameObject.SetActive(false);
                gameover.gameObject.SetActive(false);
                pause.gameObject.SetActive(false);
            }
        }
    }
    public void blockChange()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(2);
    }
    public void Lobby()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void GameOver()
    {
        if (gameOver) p = true;
        gameOver = false;
        Pause();
        tilemap.ClearAllTiles();
        StageInit();
        // Do anything else you want on game over here..
    }

    public void StageInit()
    {
        GetComponent<Piece>().stepDelay = 1;
        blocks = new List<int>{
            0, 1, 2, 3, 4, 5, 6, 7
        };
        if (Data.Cells[Tetromino.C].Length == 0) blocks.RemoveAt(7);
        int R = Random.Range(0, blocks.Count);
        NextTile = blocks[R];
        blocks.RemoveAt(R);
        gameOver = false;
        Score = 0;
        HighScore = PlayerPrefs.GetInt("HighScore");
        Combo = 0;
        Hold = -1;
        preView.Clear();
        hold.Clear();
        SpawnPiece();
        GetComponent<AudioSource>().Play();
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
        int row = bounds.yMax-1;
        int lrow=-10;

        // Clear from bottom to top
        int plus=0;
        while (row >= bounds.yMin)
        {
            // Only advance to the next row if the current is not cleared
            // because the tiles above will fall down when a row is cleared
            if (IsLineFull(row)) 
            {
                plus++;
                LineClear(row);
                lrow = row;
            } 
            else 
            {
                row--;
            }
        }

        if(plus > 0)
        {
            Score += (plus*2-1 + (plus / 4)+((plus/5)*2)) * 100 * (1 + (Combo-1)/2);
            ScorePre.Spawn(new Vector3(8, lrow+0.5f, 0), (plus * 2 - 1 + (plus / 4) + ((plus / 5) * 2)) * 100 * (1 + (Combo - 1) / 2));
            Combo++;
            ComboPre.Spawn(new Vector3(-8, lrow + 0.5f, 0), Combo);
        }
        else
        {
            Combo = 0;
        }


        if (IsLineEmpty(-10))
        {
            Score += 10000;
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
        Instantiate(effectOb, new Vector3(0, row + 0.5f, 0), Quaternion.identity);
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
