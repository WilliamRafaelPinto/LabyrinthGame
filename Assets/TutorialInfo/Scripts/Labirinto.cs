// using System.Collections.Generic;
// using UnityEngine;

// public class MazeGenerator : MonoBehaviour
// {
//     public int mazeWidth = 50;
//     public int mazeHeight = 50;
//     public float cellSize = 20f;

//     public GameObject wallPrefab;
//     public GameObject collectiblePrefab; // New
//     public Transform mazeParent;

//     private Vector2Int[] directions = {
//         new Vector2Int(0, 1),   // up
//         new Vector2Int(1, 0),   // right
//         new Vector2Int(0, -1),  // down
//         new Vector2Int(-1, 0)   // left
//     };

//     private struct Cell
//     {
//         public bool visited;
//         public bool[] walls; // top, right, bottom, left

//         public Cell(bool v)
//         {
//             visited = v;
//             walls = new bool[] { true, true, true, true };
//         }
//     }

//     private Cell[,] maze;
//     private List<Vector2Int> emptyCells = new List<Vector2Int>();

//     void Start()
//     {
//         GenerateMaze();
//         DrawMaze();
//         PlaceCollectibles(3); // Place 3 collectibles
//     }

//     void GenerateMaze()
//     {
//         maze = new Cell[mazeWidth, mazeHeight];

//         for (int x = 0; x < mazeWidth; x++)
//             for (int y = 0; y < mazeHeight; y++)
//                 maze[x, y] = new Cell(false);

//         RecursiveBacktrack(new Vector2Int(0, 0));
//     }

//     void RecursiveBacktrack(Vector2Int pos)
//     {
//         maze[pos.x, pos.y].visited = true;

//         List<int> dirs = new List<int> { 0, 1, 2, 3 };
//         Shuffle(dirs);

//         foreach (int i in dirs)
//         {
//             Vector2Int next = pos + directions[i];

//             if (IsInBounds(next) && !maze[next.x, next.y].visited)
//             {
//                 maze[pos.x, pos.y].walls[i] = false;
//                 maze[next.x, next.y].walls[(i + 2) % 4] = false;

//                 RecursiveBacktrack(next);
//             }
//         }
//     }

//     void DrawMaze()
//     {
//         Vector3 origin = new Vector3(-mazeWidth * cellSize / 2, 0, -mazeHeight * cellSize / 2);

//         for (int x = 0; x < mazeWidth; x++)
//         {
//             for (int y = 0; y < mazeHeight; y++)
//             {
//                 Vector3 cellPos = origin + new Vector3(x * cellSize, 0, y * cellSize);
//                 emptyCells.Add(new Vector2Int(x, y)); // Store cell for collectible placement

//                 if (maze[x, y].walls[0]) // top
//                     Instantiate(wallPrefab, cellPos + new Vector3(0, 0, cellSize / 2), Quaternion.identity, mazeParent).transform.localScale = new Vector3(cellSize, 10, 1);

//                 if (maze[x, y].walls[1]) // right
//                     Instantiate(wallPrefab, cellPos + new Vector3(cellSize / 2, 0, 0), Quaternion.Euler(0, 90, 0), mazeParent).transform.localScale = new Vector3(cellSize, 10, 1);

//                 if (y == 0 && maze[x, y].walls[2]) // bottom
//                     Instantiate(wallPrefab, cellPos + new Vector3(0, 0, -cellSize / 2), Quaternion.identity, mazeParent).transform.localScale = new Vector3(cellSize, 10, 1);

//                 if (x == 0 && maze[x, y].walls[3]) // left
//                     Instantiate(wallPrefab, cellPos + new Vector3(-cellSize / 2, 0, 0), Quaternion.Euler(0, 90, 0), mazeParent).transform.localScale = new Vector3(cellSize, 10, 1);
//             }
//         }
//     }

//     void PlaceCollectibles(int amount)
//     {
//         Vector3 origin = new Vector3(-mazeWidth * cellSize / 2, 0, -mazeHeight * cellSize / 2);
//         List<Vector2Int> cells = new List<Vector2Int>(emptyCells);
//         Shuffle(cells);

//         int placed = 0;
//         foreach (Vector2Int cell in cells)
//         {
//             if (cell == Vector2Int.zero) continue; // Avoid start position

//             Vector3 spawnPos = origin + new Vector3(cell.x * cellSize, 1, cell.y * cellSize);
//             Instantiate(collectiblePrefab, spawnPos, Quaternion.identity);

//             placed++;
//             if (placed >= amount)
//                 break;
//         }
//     }

//     bool IsInBounds(Vector2Int pos)
//     {
//         return pos.x >= 0 && pos.x < mazeWidth && pos.y >= 0 && pos.y < mazeHeight;
//     }

//     void Shuffle(List<int> list)
//     {
//         for (int i = 0; i < list.Count; i++)
//         {
//             int rnd = Random.Range(i, list.Count);
//             (list[i], list[rnd]) = (list[rnd], list[i]);
//         }
//     }

//     void Shuffle<T>(List<T> list)
//     {
//         for (int i = 0; i < list.Count; i++)
//         {
//             int rnd = Random.Range(i, list.Count);
//             (list[i], list[rnd]) = (list[rnd], list[i]);
//         }
//     }
// }

using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    // If you want to preview in inspector, set defaults here.
    public int mazeWidth = 10;
    public int mazeHeight = 10;
    public float cellSize = 2f;

    public GameObject wallPrefab;
    public GameObject collectiblePrefab;
    public Transform mazeParent;

    private Vector2Int[] directions = {
        new Vector2Int(0, 1),   // up
        new Vector2Int(1, 0),   // right
        new Vector2Int(0, -1),  // down
        new Vector2Int(-1, 0)   // left
    };

    private struct Cell
    {
        public bool visited;
        public bool[] walls; // top, right, bottom, left

        public Cell(bool v)
        {
            visited = v;
            walls = new bool[] { true, true, true, true };
        }
    }

    private Cell[,] maze;
    private List<Vector2Int> emptyCells = new List<Vector2Int>();

    // void Start()
    // {
    //     // If GameManager specifies a maze size, use it
    //     if (GameManager.Instance != null)
    //     {
    //         int gmSize = GameManager.Instance.GetCurrentMazeSize();
    //         mazeWidth = gmSize;
    //         mazeHeight = gmSize;
    //     }

    //     GenerateMaze();
    //     DrawMaze();

    //     // ask GameManager how many coins should be placed (fallback to 3)
    //     int coinAmount = 3;
    //     if (GameManager.Instance != null)
    //         coinAmount = GameManager.Instance.GetCoinCount();

    //     PlaceCollectibles(coinAmount);
    // }

    public void Generate(int width, int height)
{
    mazeWidth = width;
    mazeHeight = height;

    emptyCells.Clear();
    GenerateMaze();
    DrawMaze();
}

    void GenerateMaze()
    {
        maze = new Cell[mazeWidth, mazeHeight];

        for (int x = 0; x < mazeWidth; x++)
            for (int y = 0; y < mazeHeight; y++)
                maze[x, y] = new Cell(false);

        RecursiveBacktrack(new Vector2Int(0, 0));
    }

    void RecursiveBacktrack(Vector2Int pos)
    {
        maze[pos.x, pos.y].visited = true;

        List<int> dirs = new List<int> { 0, 1, 2, 3 };
        Shuffle(dirs);

        foreach (int i in dirs)
        {
            Vector2Int next = pos + directions[i];

            if (IsInBounds(next) && !maze[next.x, next.y].visited)
            {
                maze[pos.x, pos.y].walls[i] = false;
                maze[next.x, next.y].walls[(i + 2) % 4] = false;

                RecursiveBacktrack(next);
            }
        }
    }

    void DrawMaze()
    {
        emptyCells.Clear();

        Vector3 origin = new Vector3(-mazeWidth * cellSize / 2f, 0, -mazeHeight * cellSize / 2f);

        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                Vector3 cellPos = origin + new Vector3(x * cellSize, 0, y * cellSize);
                emptyCells.Add(new Vector2Int(x, y)); // store for collectible placement

                // top (z +)
                if (maze[x, y].walls[0])
                {
                    var w = Instantiate(wallPrefab, cellPos + new Vector3(0, 0, cellSize / 2f), Quaternion.identity, mazeParent);
                    w.transform.localScale = new Vector3(cellSize, w.transform.localScale.y, 1f);
                }

                // right (x +)
                if (maze[x, y].walls[1])
                {
                    var w = Instantiate(wallPrefab, cellPos + new Vector3(cellSize / 2f, 0, 0), Quaternion.Euler(0, 90, 0), mazeParent);
                    w.transform.localScale = new Vector3(cellSize, w.transform.localScale.y, 1f);
                }

                // bottom (only draw on y==0 to avoid duplicates)
                if (y == 0 && maze[x, y].walls[2])
                {
                    var w = Instantiate(wallPrefab, cellPos + new Vector3(0, 0, -cellSize / 2f), Quaternion.identity, mazeParent);
                    w.transform.localScale = new Vector3(cellSize, w.transform.localScale.y, 1f);
                }

                // left (only draw on x==0 to avoid duplicates)
                if (x == 0 && maze[x, y].walls[3])
                {
                    var w = Instantiate(wallPrefab, cellPos + new Vector3(-cellSize / 2f, 0, 0), Quaternion.Euler(0, 90, 0), mazeParent);
                    w.transform.localScale = new Vector3(cellSize, w.transform.localScale.y, 1f);
                }
            }
        }
    }

    public void PlaceCollectibles(int amount)
    {
        if (collectiblePrefab == null) return;

        Vector3 origin = new Vector3(-mazeWidth * cellSize / 2f, 0, -mazeHeight * cellSize / 2f);
        List<Vector2Int> cells = new List<Vector2Int>(emptyCells);
        Shuffle(cells);

        int placed = 0;
        foreach (Vector2Int cell in cells)
        {
            if (cell == Vector2Int.zero) continue; // avoid start cell

            Vector3 spawnPos = origin + new Vector3(cell.x * cellSize, 1f, cell.y * cellSize);
            Instantiate(collectiblePrefab, spawnPos, Quaternion.identity, mazeParent);

            placed++;
            if (placed >= amount)
                break;
        }
    }

    bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < mazeWidth && pos.y >= 0 && pos.y < mazeHeight;
    }

    void Shuffle(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rnd = Random.Range(i, list.Count);
            (list[i], list[rnd]) = (list[rnd], list[i]);
        }
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rnd = Random.Range(i, list.Count);
            (list[i], list[rnd]) = (list[rnd], list[i]);
        }
    }
}
