using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    private Vector2Int Vec2(int x, int y) { return new Vector2Int(x, y); }
    private Vector3Int Vec3(int x, int y) { return new Vector3Int(x, y, 0); }

    public Tilemap groundTilemap;
    public Tilemap landTilemap;
    public Tilemap spawnTilemap;
    public Vector2Int groundTilemapOffset;
    public Vector2Int landTilemapOffset;
    public Vector2Int spawnTilemapOffset;

    public AdvancedRuleTile[] groundTiles;
    public AdvancedRuleTile[] waterTiles;
    public AdvancedRuleTile[] landTiles;
    public RandomEnemyRuleTile[] spawnTiles;

    public float[] perlinScales = { 0.1f, 0.05f, 0.05f };
    public int seed = 0;
    public bool randomSeed = true;

    public int blurAmount = 10;

    public float groundThreshold = 0.2f;
    public float landThreshold = 0.7f;
    public float spawnThreshold = 0.9f;

    public Vector2Int mapDimensions = new Vector2Int(100, 100);
    public float centerRegionRadius = 0.3f;

    public int borderDistance = 40;

    public List<Vector2Int> validSpawnPositions = new List<Vector2Int>();

    public Node[,] Grid;

    private void Awake()
    {
        if (randomSeed) seed = Random.Range(0, 100000);
        Random.InitState(seed);
        groundTilemapOffset = Vec2(Random.Range(0, 1000), Random.Range(0, 1000));
        landTilemapOffset = Vec2(Random.Range(0, 1000), Random.Range(0, 1000));
        spawnTilemapOffset = Vec2(Random.Range(0, 1000), Random.Range(0, 1000));

        GenerateMap();
        PostProcessMap();
        PostProcessMap();
        PostProcessMap();
        Grid = GenerateGrid(groundTilemap, landTilemap, mapDimensions);
    }

    private void GenerateMap()
    {
        for (int x = -borderDistance; x < mapDimensions.x + borderDistance; x++)
        {
            for (int y = -borderDistance; y < mapDimensions.y + borderDistance; y++)
            {
                int biome = getBiome(Vec2(x, y));

                if (x < 0 || x >= mapDimensions.x || y < 0 || y >= mapDimensions.y)
                {
                    groundTilemap.SetTile(Vec3(x, y), waterTiles[biome]);
                    continue;
                }

                float groundValue = Mathf.PerlinNoise(x * perlinScales[0] + groundTilemapOffset.x, y * perlinScales[0] + groundTilemapOffset.y);
                if (groundValue > groundThreshold)
                {
                    SetGroundAndLandTile(x, y, biome);
                }
                else
                {
                    TileBase waterTile = waterTiles[biome];
                    groundTilemap.SetTile(Vec3(x, y), waterTile);
                }
            }
        }
    }

    public Node[,] GenerateGrid(Tilemap groundTilemap, Tilemap landTilemap, Vector2Int mapDimensions)
    {
        Node[,] Grid = new Node[mapDimensions.x, mapDimensions.y];

        for (int x = 0; x < mapDimensions.x; x++)
        {
            for (int y = 0; y < mapDimensions.y; y++)
            {
                if (Grid[x, y] != null) { continue; }

                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                Vector3 worldPosition = groundTilemap.CellToWorld(tilePosition) + new Vector3(0.5f, 0.5f, 0);

                bool isWalkable = IsValidTile(x, y);

                // Marque o nó atual como não andável se houver um landTile
                if (landTilemap.GetTile(tilePosition) != null)
                {
                    isWalkable = false;
                    MarkNodeIfValid(Grid, x, y + 1, mapDimensions);
                }

                Grid[x, y] = new Node(new Vector2Int(x, y), isWalkable, worldPosition);
            }
        }

        return Grid;
    }

    private void MarkNodeIfValid(Node[,] Grid, int x, int y, Vector2Int mapDimensions)
    {
        if (x >= 0 && x < mapDimensions.x && y >= 0 && y < mapDimensions.y)
        {
            // Converte a posição do tile para posição do mundo e ajusta para o centro do tile
            Vector3Int tilePosition = new Vector3Int(x, y, 0);
            Vector3 worldPosition = groundTilemap.CellToWorld(tilePosition) + new Vector3(0.5f, 0.5f, 0);

            Grid[x, y] = new Node(new Vector2Int(x, y), false, worldPosition);
        }
    }

    void OnDrawGizmos()
    {
        if (Grid == null) return;
        for (int x = 0; x < mapDimensions.x; x++)
        {
            for (int y = 0; y < mapDimensions.y; y++)
            {
                Node node = Grid[x, y];
                if (node.IsWalkable)
                {
                    Gizmos.color = Color.green;
                    Vector3 position = new Vector3(node.Position.x + 0.5f, node.Position.y + 0.5f, 0);
                    Gizmos.DrawCube(position, Vector3.one * 0.5f);
                }
            }
        }
    }

    public void GenerateEnemies(int amount)
    {
        Debug.Log($"Generating {amount} enemies");
        List<Vector2Int> positions = new List<Vector2Int>();
        for (int x = 0; x < mapDimensions.x; x++)
        {
            for (int y = 0; y < mapDimensions.y; y++)
            {
                if (Grid[x, y].IsWalkable)
                {
                    positions.Add(Vec2(x, y));
                }
            }
        }
        positions = positions.OrderBy(x => Random.value).ToList();
        for (int i = 0; i < amount; i++)
        {
            Vector2Int position = positions[i];
            int biome = getBiome(position);
            spawnTilemap.SetTile(Vec3(position.x, position.y), spawnTiles[biome]);
        }
    }

    public void GenerateEnemies(int amount, NeuralNetwork[] filhos)
    {
        foreach (var Enemy in BaseEnemy.AllEnemies)
        {
            Enemy.InstantDie();
        }
        BaseEnemy.AllEnemies.Clear();

        Debug.Log($"Generating {amount} children enemies");

        List<Vector3Int> positions = new List<Vector3Int>();
        for (int x = 0; x < mapDimensions.x; x++)
        {
            for (int y = 0; y < mapDimensions.y; y++)
            {
                if (Grid[x, y].IsWalkable)
                {
                    positions.Add(Vec3(x, y));
                }
            }
        }

        positions = positions.OrderBy(x => Random.value).ToList();
        for (int i = 0; i < amount; i++)
        {
            Vector3Int position = positions[i];

            GameObject novoInimigo = GameManager.Instance.EnemyManager.RandomEnemyRuleTile.GetRandomEnemy();
            IntelligentEnemy intelligentEnemy = novoInimigo.GetComponent<IntelligentEnemy>();

            intelligentEnemy.NN = filhos[(i % filhos.Length)];
            Instantiate(novoInimigo, position, Quaternion.identity);
        }
    }

    public Vector2Int SafeRandomSpot()
    {
        if (validSpawnPositions.Count > 0)
        {
            return validSpawnPositions[Random.Range(0, validSpawnPositions.Count)];
        }
        else
        {
            Debug.LogError("Não foram encontradas posições válidas para spawn.");
            return Vector2Int.zero; // Retorna uma posição padrão ou erro
        }
    }

    private void SetGroundAndLandTile(int x, int y, int biome)
    {
        groundTilemap.SetTile(Vec3(x, y), groundTiles[biome]);

        float landValue = Mathf.PerlinNoise(x * perlinScales[1] + landTilemapOffset.x, y * perlinScales[1] + landTilemapOffset.y);
        if (landValue > landThreshold)
        {
            landTilemap.SetTile(Vec3(x, y), landTiles[biome]);
        }
        /* else
        {
            float spawnValue = Mathf.PerlinNoise(x * perlinScales[2] + spawnTilemapOffset.x, y * perlinScales[2] + spawnTilemapOffset.y);
            if (spawnValue > spawnThreshold)
            {
                spawnTilemap.SetTile(Vec3(x, y), spawnTiles[biome]);
                validSpawnPositions.Add(new Vector2Int(x, y));
            }
        } */
    }

    private bool IsWaterTile(int x, int y)
    {
        if (x >= 0 && x < mapDimensions.x && y >= 0 && y < mapDimensions.y)
        {
            return waterTiles.Contains(groundTilemap.GetTile(Vec3(x, y)) as AdvancedRuleTile);
        }
        return false;
    }

    private bool IsValidTile(int x, int y)
    {
        if (x >= 0 && x < mapDimensions.x && y >= 0 && y < mapDimensions.y)
        {
            return !(waterTiles.Contains(groundTilemap.GetTile(Vec3(x, y)) as AdvancedRuleTile) ||
                    landTiles.Contains(landTilemap.GetTile(Vec3(x, y)) as AdvancedRuleTile));
        }
        return false;
    }

    private void PostProcessMap()
    {
        for (int x = 0; x < mapDimensions.x; x++)
        {
            for (int y = 0; y < mapDimensions.y; y++)
            {
                if (IsWaterTile(x, y))
                {
                    int Check = IsIsolatedWater(x, y);
                    int biome = getBiome(Vec2(x, y));
                    switch (Check)
                    {
                        case 1:
                            groundTilemap.SetTile(Vec3(x, y), groundTiles[biome]);
                            break;
                        case 2:
                            groundTilemap.SetTile(Vec3(x - 1, y + 1), waterTiles[biome]);
                            groundTilemap.SetTile(Vec3(x + 1, y - 1), waterTiles[biome]);
                            break;
                        case 3:
                            groundTilemap.SetTile(Vec3(x - 1, y - 1), waterTiles[biome]);
                            groundTilemap.SetTile(Vec3(x + 1, y + 1), waterTiles[biome]);
                            break;
                    }
                }
            }
        }
    }

    private int IsIsolatedWater(int x, int y)
    {
        bool left = IsWaterTile(x - 1, y);
        bool right = IsWaterTile(x + 1, y);
        bool up = IsWaterTile(x, y + 1);
        bool down = IsWaterTile(x, y - 1);

        bool upLeft = IsWaterTile(x - 1, y + 1);
        bool upRight = IsWaterTile(x + 1, y + 1);
        bool downLeft = IsWaterTile(x - 1, y - 1);
        bool downRight = IsWaterTile(x + 1, y - 1);

        if ((!left && !right) || (!up && !down))
        {
            return 1;
        }
        else if ((left && right) || (up && down))
        {
            if (!upLeft && !downRight) return 2;
            if (!upRight && !downLeft) return 3;
        }

        return 0;
    }

    private int getBiome(Vector2Int position)
    {
        Vector2Int blurredPosition = new(position.x + Random.Range(-blurAmount, blurAmount), position.y + Random.Range(-blurAmount, blurAmount));

        Vector2Int centerPoint = new(mapDimensions.x / 2, mapDimensions.y / 2);
        float distanceToCenter = Vector2Int.Distance(blurredPosition, centerPoint);

        if (distanceToCenter <= centerRegionRadius * Mathf.Min(mapDimensions.x, mapDimensions.y) / 2)
        {
            return 0;
        }

        if (blurredPosition.x >= centerPoint.x) return blurredPosition.y >= centerPoint.y ? 1 : 2;
        return blurredPosition.y >= centerPoint.y ? 3 : 4;
    }
}
