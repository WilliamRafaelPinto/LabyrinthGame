// gerar labirinto usando algoritmo de backtracking recursivo com funcionalidades dinâmicas
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    // 1. Mudar a propriedade Instance para ser privada e usar um método estático para acesso
    private static MazeGenerator _instance;
    public static MazeGenerator Instance
    {
        get
        {
            // Se a instância for nula, tenta encontrá-la na cena
            if (_instance == null)
            {
                _instance = FindObjectOfType<MazeGenerator>();

                // Se ainda for nula, algo está errado ou ainda não foi instanciada
                if (_instance == null)
                {
                    Debug.LogError("MazeGenerator: Nenhuma instância encontrada na cena.");
                }
            }
            return _instance;
        }
    }

    public int mazeWidth = 10;
    public int mazeHeight = 10;
    public float cellSize = 2f;

    public GameObject wallPrefab;
    public GameObject collectiblePrefab;
    public GameObject slowObstaclePrefab; // Novo: obstáculo que reduz velocidade
    public Transform mazeParent;

    [Header("Debug")]
    public bool isActiveInstance = false;

    [Header("Dynamic Walls Settings")]
    public bool enableDynamicWalls = true;
    public float wallToggleInterval = 5f; // Alternar a cada 5 segundos
    public Material transparentWallMaterial; // Material para paredes transparentes
    
    [Header("Multiple Paths Settings")]
    [Range(0f, 1f)]
    public float extraPathProbability = 0.5f; // Probabilidade de criar caminhos extras

    [Header("Obstacles Settings")]
    public int timePenaltyObstacleCount = 2;

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
        public bool isDynamicWall; // Se esta célula tem paredes dinâmicas

        public Cell(bool v)
        {
            visited = v;
            walls = new bool[] { true, true, true, true };
            isDynamicWall = false;
        }
    }

    private Cell[,] maze;
    private List<Vector2Int> emptyCells = new List<Vector2Int>();
    private List<GameObject> dynamicWalls = new List<GameObject>(); // Lista de paredes dinâmicas
    private float lastToggleTime = 0f;
    private bool wallsActive = true;
    
    void Awake()
    {
        //Debug.Log($"🔧 MazeGenerator Awake - Instance ID: {GetInstanceID()}");
        /*
        // 2. Lógica de Singleton modificada:
        // Se a instância for nula, esta é a primeira.
        if (_instance == null)
        {
            _instance = this;
            isActiveInstance = true;
            // O MazeGenerator NÃO deve usar DontDestroyOnLoad, pois ele é específico da cena.
            Debug.Log($"✅ MazeGenerator instância principal definida: {GetInstanceID()}");
        }
        // Se a instância já existe e não é esta, destrua-se.
        // Isso é para o caso de o prefab ser instanciado diretamente na cena.
        else if (_instance != this)
        {
            //Debug.Log($"🗑️ Destruindo MazeGenerator duplicado: {GetInstanceID()}");
            Destroy(gameObject);
        }*/
        // Se _instance == this, significa que o objeto já foi definido como a instância.
    }
    
    void OnDestroy()
    {
        // Se esta é a instância ativa, limpe a referência
        if (_instance == this)
        {
            //Debug.Log($"🧹 Limpando referência MazeGenerator: {GetInstanceID()}");
            _instance = null;
        }
    }
    
    public void Generate(int width, int height)
    {
        // Destruir o labirinto anterior antes de gerar um novo
        Debug.Log($"🔧 MazeGenerator Generate pt 1 - Instance ID: {GetInstanceID()}");
        if (mazeParent != null)
        {
            foreach (Transform child in mazeParent)
            {
                Destroy(child.gameObject);
            }
        }
        Debug.Log($"🔧 MazeGenerator Generate pt 2 - Instance ID: {GetInstanceID()}");
        mazeWidth = width;
        mazeHeight = height;

        emptyCells.Clear();
        dynamicWalls.Clear();
        GenerateMaze();
        Debug.Log($"🔧 MazeGenerator Generate pt 3 - Instance ID: {GetInstanceID()}");
        DrawMaze();
        Debug.Log($"🔧 MazeGenerator Generate pt 4 - Instance ID: {GetInstanceID()}");
        
        // Inicia a corrotina para alternar paredes se estiver habilitado
        if (enableDynamicWalls)
        {
            lastToggleTime = Time.time;
        }
    }

    void GenerateMaze()
    {
        maze = new Cell[mazeWidth, mazeHeight];
        
        for (int x = 0; x < mazeWidth; x++)
            for (int y = 0; y < mazeHeight; y++)
                maze[x, y] = new Cell(false);

        RecursiveBacktrack(new Vector2Int(0, 0));
        
        // Adicionar caminhos extras para mais dinâmica
        AddExtraPaths();
        
        // Marcar algumas paredes internas como dinâmicas
        MarkDynamicWalls();
        
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

    // Adiciona caminhos extras para criar múltiplas rotas
    void AddExtraPaths()
    {
        for (int x = 1; x < mazeWidth - 1; x++)
        {
            for (int y = 1; y < mazeHeight - 1; y++)
            {
                // Chance de remover uma parede interna para criar atalho
                if (Random.value < extraPathProbability)
                {
                    // Escolhe uma direção aleatória para remover a parede
                    int dir = Random.Range(0, 4);
                    Vector2Int next = new Vector2Int(x, y) + directions[dir];
                    
                    if (IsInBounds(next) && !IsBorderWall(x, y, dir))
                    {
                        maze[x, y].walls[dir] = false;
                        maze[next.x, next.y].walls[(dir + 2) % 4] = false;
                    }
                }
            }
        }
    }

    // Marca paredes internas como dinâmicas (não inclui bordas)
    void MarkDynamicWalls()
    {
        //Debug.Log("Marking dynamic walls...");
        for (int x = 0; x < mazeWidth; x++)
        {
            //Debug.Log("Processing row " + x);
            for (int y = 0; y < mazeHeight; y++)
            {
                //Debug.Log(" Processing cell " + y);
                for (int dir = 0; dir < 4; dir++)
                {
                    if (maze[x, y].walls[dir] && !IsBorderWall(x, y, dir))
                    {
                        // Chance de ser uma parede dinâmica
                        if (Random.value <= 1.0f) // 30% de chance
                        {
                            maze[x, y].isDynamicWall = true;
                            //Debug.Log("  Cell (" + x + "," + y + ") wall " + dir + " marked as dynamic.");
                        }else
                        {
                            //Debug.Log("  Cell (" + x + "," + y + ") wall " + dir + " is static.");
                        }
                    }
                }
            }
        }
    }

    // Verifica se é uma parede da borda
    bool IsBorderWall(int x, int y, int direction)
    {
        return (x == 0 && direction == 3) || // Parede esquerda da borda
               (x == mazeWidth - 1 && direction == 1) || // Parede direita da borda
               (y == 0 && direction == 2) || // Parede inferior da borda
               (y == mazeHeight - 1 && direction == 0); // Parede superior da borda
    }

    void DrawMaze()
    {
        emptyCells.Clear();
        //dynamicWalls.Clear();

        Vector3 origin = new Vector3(-mazeWidth * cellSize / 2f, 0, -mazeHeight * cellSize / 2f);
        int dynamicWallCount = 0;
        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                Vector3 cellPos = origin + new Vector3(x * cellSize, 0, y * cellSize);
                emptyCells.Add(new Vector2Int(x, y));

                // top (z +)
                if (maze[x, y].walls[0])
                {
                    CreateWall(cellPos + new Vector3(0, 0, cellSize / 2f), Quaternion.identity, x, y, 0);
                    dynamicWallCount++;
                }

                // right (x +)
                if (maze[x, y].walls[1])
                {
                    CreateWall(cellPos + new Vector3(cellSize / 2f, 0, 0), Quaternion.Euler(0, 90, 0), x, y, 1);
                    dynamicWallCount++;
                }

                // bottom (only draw on y==0 to avoid duplicates)
                if (y == 0 && maze[x, y].walls[2])
                {
                    CreateWall(cellPos + new Vector3(0, 0, -cellSize / 2f), Quaternion.identity, x, y, 2);
                    dynamicWallCount++;
                }

                // left (only draw on x==0 to avoid duplicates)
                if (x == 0 && maze[x, y].walls[3])
                {
                    CreateWall(cellPos + new Vector3(-cellSize / 2f, 0, 0), Quaternion.Euler(0, 90, 0), x, y, 3);
                    dynamicWallCount++;
                }
            }
        }
        Debug.Log("DrawMaze finished. Total dynamic walls: " + dynamicWallCount);
        Debug.Log("dynamicWalls list count: " + dynamicWalls.Count);
    }

    void CreateWall(Vector3 position, Quaternion rotation, int x, int y, int direction)
    {
        var wall = Instantiate(wallPrefab, position, rotation, mazeParent);
        wall.transform.localScale = new Vector3(cellSize, wall.transform.localScale.y, 1f);
        
        // Se for uma parede dinâmica e não for da borda, adiciona à lista
        if (maze[x, y].isDynamicWall && !IsBorderWall(x, y, direction) && enableDynamicWalls)
        {
            dynamicWalls.Add(wall);
            //Debug.Log("Added dynamic wall at cell (" + x + "," + y + ") direction " + direction);
            // Adiciona componente para controlar o estado da parede
            var wallController = wall.AddComponent<DynamicWallController>();
            wallController.SetTransparentMaterial(transparentWallMaterial);
        }
    }

    void Update()
    {
        // Alternar estado das paredes dinâmicas a cada intervalo
        //Debug.Log("Updating MazeGenerator...");
        Debug.Log("MazeGenerator update - Instance ID: " + GetInstanceID());
        if (enableDynamicWalls && dynamicWalls.Count > 0 && 
            Time.time - lastToggleTime >= wallToggleInterval)
        {
            ToggleDynamicWalls();
            lastToggleTime = Time.time;
        }else
        {
            Debug.Log("Not time to toggle walls yet."+enableDynamicWalls+" "+dynamicWalls.Count+" "+(Time.time - lastToggleTime)+" "+wallToggleInterval);
        }
    }

    void ToggleDynamicWalls()
    {
        wallsActive = !wallsActive;
        Debug.Log("Toggling dynamic walls. Now active: " + wallsActive);
        foreach (var wall in dynamicWalls)
        {
            var controller = wall.GetComponent<DynamicWallController>();
            if (controller != null)
            {
                controller.ToggleWall(wallsActive);
            }else
            {
                Debug.LogWarning("Dynamic wall missing controller component.");
            }
        }
    }

    public void PlaceCollectibles(int amount)
    {
        if (collectiblePrefab == null) return;

        Vector3 origin = new Vector3(-mazeWidth * cellSize / 2f, 0, -mazeHeight * cellSize / 2f);
        
        // Garante que a lista de células vazias não esteja vazia
        if (emptyCells.Count == 0)
        {
            Debug.LogWarning("Não há células vazias para colocar colecionáveis.");
            return;
        }

        // Limita a quantidade ao número de células vazias
        int count = Mathf.Min(amount, emptyCells.Count);
        
        // Escolhe posições aleatórias sem repetição
        List<Vector2Int> positions = new List<Vector2Int>(emptyCells);
        Shuffle(positions);

        for (int i = 0; i < count; i++)
        {
            Vector2Int pos = positions[i];
            Vector3 worldPos = origin + new Vector3(pos.x * cellSize, 0.5f, pos.y * cellSize);
            Instantiate(collectiblePrefab, worldPos, Quaternion.identity, mazeParent);
        }
    }

    public void PlaceObstacles()
    {
        if (slowObstaclePrefab == null) return;

        Vector3 origin = new Vector3(-mazeWidth * cellSize / 2f, 0, -mazeHeight * cellSize / 2f);
        
        // Garante que a lista de células vazias não esteja vazia
        if (emptyCells.Count == 0)
        {
            Debug.LogWarning("Não há células vazias para colocar obstáculos.");
            return;
        }

        // Limita a quantidade ao número de células vazias
        int count = Mathf.Min(timePenaltyObstacleCount, emptyCells.Count);
        
        // Escolhe posições aleatórias sem repetição
        List<Vector2Int> positions = new List<Vector2Int>(emptyCells);
        Shuffle(positions);

        for (int i = 0; i < count; i++)
        {
            Vector2Int pos = positions[i];
            Vector3 worldPos = origin + new Vector3(pos.x * cellSize, 0.5f, pos.y * cellSize);
            Instantiate(slowObstaclePrefab, worldPos, Quaternion.identity, mazeParent);
        }
    }

    bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < mazeWidth && pos.y >= 0 && pos.y < mazeHeight;
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}

// Classe auxiliar para controlar o estado da parede dinâmica (necessária para o Labirinto.cs)
public class DynamicWallController : MonoBehaviour
{
    private Material originalMaterial;
    private Material transparentMaterial;
    private Renderer wallRenderer;
    private Collider wallCollider;

    public void SetTransparentMaterial(Material material)
    {
        transparentMaterial = material;
        wallRenderer = GetComponent<Renderer>();
        wallCollider = GetComponent<Collider>();
        if (wallRenderer != null)
        {
            originalMaterial = wallRenderer.material;
        }
    }

    public void ToggleWall(bool active)
    {
        if (wallRenderer == null || wallCollider == null) return;

        if (active)
        {
            // Parede ativa: visível e com colisão
            wallRenderer.material = originalMaterial;
            wallCollider.enabled = true;
        }
        else
        {
            // Parede inativa: transparente e sem colisão (passável)
            wallRenderer.material = transparentMaterial;
            wallCollider.enabled = false;
        }
    }
}
