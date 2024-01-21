using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject Player;
    public GameObject CameraObject;
    public MapGenerator MapGenerator;
    public GameObject MusicManager;
    public GameObject GameOverScreen;
    public GameObject VictoryScreen;
    public GameObject PauseScreen;
    public GameObject BossManager;
    public RageService RageService;
    public EnemyManager EnemyManager;
    public List<BaseEnemy> AllEnemies => BaseEnemy.AllEnemies;

    public float graphUpdateRate = 3f;
    public float graphUpdateTime = float.PositiveInfinity;

    public float graphUpdateTime2 = float.PositiveInfinity;

    public Vector2Int mapDimensions = new Vector2Int(100, 100);

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Garantir que o GameManager não seja destruído ao carregar cenas.
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // Destrua esta instância, pois ela é duplicada.
        }
    }

    public void TryUpdateLabel()
    {
        if (Time.time >= graphUpdateTime)
        {
            BaseEnemy.InitLabels();
            graphUpdateTime = Time.time + graphUpdateRate * 2;
            graphUpdateTime2 = Time.time + graphUpdateRate;
            LabelPropagation();
        }

        if (Time.time >= graphUpdateTime2)
        {
            Groupify();
            graphUpdateTime2 = Time.time + graphUpdateRate * 4;
        }
    }

    public void LabelPropagation()
    {
        const int heuristic = 15;
        for (int i = 0; i < heuristic; i++)
        {
            foreach (var enemy in AllEnemies)
            {
                enemy.UpdateLabel();
            }
        }
    }

    public void Groupify()
    {
        var groups = AllEnemies.GroupBy(item => item.Label);
        string str2 = "";
        foreach (var enemy in AllEnemies)
        {
            str2 += enemy.Label + " ";
        }
        (double, BaseEnemy[])[] groupList = new (double, BaseEnemy[])[groups.Count()];
        int j = 0;

        foreach (var group in groups)
        {
            BaseEnemy[] enemies = new BaseEnemy[group.Count()];

            int monsters = 0;
            int humans = 0;
            int humanoids = 0;
            int i = 0;

            foreach (var enemy in group)
            {
                enemies[i] = enemy;

                switch (enemy.TypeGroup)
                {
                    case BaseActor.ActorTypeGroup.Monster:
                        monsters++;
                        break;
                    case BaseActor.ActorTypeGroup.Humanoid:
                        humanoids++;
                        break;
                    case BaseActor.ActorTypeGroup.Human:
                        humans++;
                        break;
                }

                i++;
            }

            double weight = (humans * 1.1 - (monsters * 0.6 + humanoids * 0.4)) / 10;
            groupList[j++] = (weight, group.ToArray());
        }

        foreach ((double key, BaseEnemy[] enemies) in groupList)
        {
            foreach (BaseEnemy enemy in enemies)
            {
                enemy.GroupMenaceLevel = key;
            }
            string enemyList = string.Join(", ", enemies.Select(e => e.Type.ToString()));
            string str = key > 0 ? "Benefico" : "Perigoso";
            string f = key.ToString("F1");
            // Debug.Log($"[{str}]\t[{f}]\t = {enemyList}");
        }
    }

    public void Update()
    {
        if (EnemyManager != null) TryUpdateLabel();
    }

    public void PauseGame(bool pause)
    {
        Time.timeScale = pause ? 0 : 1;
        PauseScreen.SetActive(pause);
    }

    public void GameOver(int Score)
    {
        GameOverScreen.SetActive(true);
        GameOverScreen.GetComponent<DeathMenu>().SetScore(Score);
    }

    public void Victory()
    {
        VictoryScreen.SetActive(true);
        VictoryScreen.GetComponent<VictoryMenu>().SetScore(Player.GetComponent<PlayerController>().Score*2);
    }
}
