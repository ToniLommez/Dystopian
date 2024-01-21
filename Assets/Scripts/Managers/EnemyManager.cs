using System.Collections;
using GUI;
using UnityEngine;
using System.Linq;

public class EnemyManager : MonoBehaviour
{
    public bool TrainingMode;
    public RandomEnemyRuleTile RandomEnemyRuleTile;
    public MapGenerator MapGenerator;
    public Wave waveUI;
    public int wave = 1;
    public float waveTime = 5f;
    public int baseEnemies = 15;

    private string saveFilePath = "RedeNeural.json";

    public void Start()
    {
        MapGenerator.GenerateEnemies(getMaxEnemies(), LoadNeuralNetwork());
        StartCoroutine(StartWave());
    }

    public IEnumerator StartWave()
    {
        yield return new WaitForSeconds(waveTime);
        wave++;
        if (wave == 50)
        {
            foreach (var enemy in BaseEnemy.AllEnemies)
            {
                enemy.InstantDie();
                GameManager.Instance.Player.GetComponent<PlayerController>().CanMove = false;
            }
            GameManager.Instance.Victory();
            yield break;
        }
        if (TrainingMode)
        {
            MapGenerator.GenerateEnemies(getMaxEnemies(), GetBestEnemies());
        }
        else
        {
            MapGenerator.GenerateEnemies(getMaxEnemies());
        }
        StartCoroutine(StartWave());
    }

    public int getMaxEnemies()
    {
        if (TrainingMode)
        {
            return baseEnemies * 10;
        }
        else
        {
            return (int)((float)baseEnemies * wave * 0.3);
        }
    }

    public NeuralNetwork[] GetBestEnemies()
    {
        var topEnemies = BaseEnemy.AllEnemies.OrderByDescending(enemy => enemy.Score).Take(2).ToList();
        NeuralNetwork[] pais = new NeuralNetwork[4];
        NeuralNetwork[] filhos = new NeuralNetwork[4];
        int i = 0;

        foreach (var enemy in topEnemies)
        {
            if (enemy is IntelligentEnemy intelligentEnemy)
            {
                // Debug.Log("Best score = " + enemy.Score);
                pais[i] = intelligentEnemy.NN;
                i++;
            }
        }

        filhos[0] = pais[0];
        filhos[1] = pais[1];
        filhos[2] = pais[0].Crossover(pais[1]);
        filhos[3] = pais[0].Crossover(pais[1]);

        filhos[0].Mutate(0.05f, 0.2f);
        filhos[1].Mutate(0.06f, 0.3f);
        filhos[2].Mutate(0.08f, 0.45f);
        filhos[3].Mutate(0.20f, 0.6f);

        return filhos;
    }

    public NeuralNetwork[] LoadNeuralNetwork()
    {
        NeuralNetwork[] nn = new NeuralNetwork[4];
        nn[0] = NeuralNetwork.Load(saveFilePath);
        if (nn[0] == null)
        {
            Debug.Log("Iniciando um treinamento novo!");
            nn[0] = new NeuralNetwork(new int[] { 7, 10, 10, 6 });
        }
        else
        {
            Debug.Log("Rede Neural carregada com sucesso!");
            Debug.Log(nn[0].Serialize());
        }
        nn[1] = nn[2] = nn[3] = nn[0];
        return nn;
    }

    void OnApplicationQuit()
    {
        var topEnemies = BaseEnemy.AllEnemies.OrderByDescending(enemy => enemy.Score).Take(1).ToList();
        foreach (var enemy in topEnemies)
        {
            if (enemy is IntelligentEnemy intelligentEnemy)
            {
                intelligentEnemy.NN.Save(saveFilePath);
            }
        }
    }
}
