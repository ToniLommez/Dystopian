using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "RandomEnemyRuleTile", menuName = "Tiles/RandomEnemyRuleTile")]
public class RandomEnemyRuleTile : RuleTile
{
    public GameObject[] enemyPrefabs;  // Array para armazenar seus prefabs de inimigo

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        base.GetTileData(position, tilemap, ref tileData);

        if (enemyPrefabs != null && enemyPrefabs.Length > 0)
        {
            int randomIndex = Random.Range(0, enemyPrefabs.Length);
            GameObject randomEnemyPrefab = enemyPrefabs[randomIndex];
            if (randomEnemyPrefab != null)
            {
                tileData.gameObject = randomEnemyPrefab;
            }
        }
    }

    public GameObject GetEnemy(BaseActor.ActorType type)
    {
        foreach (GameObject enemyPrefab in enemyPrefabs)
        {
            BaseActor baseActor = enemyPrefab.GetComponent<BaseActor>();
            if (baseActor != null && baseActor.Type == type)
            {
                return enemyPrefab;
            }
        }
        return null;
    }

    public GameObject GetRandomEnemy()
    {
        int randomIndex = Random.Range(0, enemyPrefabs.Length);
        GameObject randomEnemyPrefab = enemyPrefabs[randomIndex];
        return randomEnemyPrefab;
    }
}