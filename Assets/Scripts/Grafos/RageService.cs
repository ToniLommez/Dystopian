using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class RageService : MonoBehaviour
{
    public Dictionary<(BaseActor.ActorType, BaseActor.ActorType), float> rageRelations;

    // Definindo as probabilidades
    private const float BAIXA = 0.25f;
    private const float MEDIA = 0.5f;
    private const float ALTA = 0.75f;
    private const float MUITO_ALTA = 1f;

    // Definindo as entidades
    private Dictionary<int, string> entidades = new Dictionary<int, string>
    {
        { 0, "Bismuts" },
        { 1, "Autômatos" },
        { 2, "Corrompidos" },
        { 3, "Cultistas" },
        { 4, "Ciborgues" },
        { 5, "Biomecanicos" },
        { 6, "Infectados" },
        { 7, "Xenomorfos" },
        { 8, "Cyberpunks" },
        { 9, "Pistoleiros" },
        { 10, "Templários" },
        { 11, "Piratas Espaciais" },
        { 12, "Player" },
        { 13, "Aparicao"}
    };

    private Dictionary<string, int> entidades_pos = new Dictionary<string, int>
    {
        { "Bismuts", 0 },
        { "Autômatos", 1 },
        { "Corrompidos", 2 },
        { "Cultistas", 3 },
        { "Ciborgues", 4 },
        { "Biomecanicos", 5 },
        { "Infectados", 6 },
        { "Xenomorfos", 7 },
        { "Cyberpunks", 8 },
        { "Pistoleiros", 9 },
        { "Templários", 10 },
        { "Piratas Espaciais", 11 },
        { "Player", 12 },
        { "Aparicao", 13}
    };

    public float GetRageFactor(BaseActor from, BaseActor to)
    {
        // Tenta pegar o valor na tabela
        var key = (from.Type, to.Type);
        if (rageRelations.TryGetValue(key, out float rageFactor))
        {
            return rageFactor;
        }

        return 0.1f;
    }

    // Método para atualizar o fator de raiva entre dois tipos de inimigos
    public void UpdateRageFactor(BaseActor.ActorType type1, BaseActor.ActorType type2, float newRageFactor)
    {
        var key = (type1, type2);
        rageRelations[key] = newRageFactor;
    }

    private void InitializeRageMatrix()
    {
        int num_entidades = 14;

        float[,] matriz_relacao = new float[num_entidades, num_entidades];

        matriz_relacao[0, 9] = matriz_relacao[0, 10] = matriz_relacao[0, 11] = MUITO_ALTA;
        matriz_relacao[0, 1] = MEDIA;

        matriz_relacao[1, 8] = matriz_relacao[1, 10] = matriz_relacao[1, 11] = MUITO_ALTA;
        matriz_relacao[1, 2] = MEDIA;

        matriz_relacao[2, 8] = matriz_relacao[2, 9] = matriz_relacao[2, 11] = MUITO_ALTA;
        matriz_relacao[2, 3] = MEDIA;

        matriz_relacao[3, 8] = matriz_relacao[3, 9] = matriz_relacao[3, 10] = MUITO_ALTA;
        matriz_relacao[3, 0] = MEDIA;

        matriz_relacao[4, 1] = matriz_relacao[4, 2] = matriz_relacao[4, 3] = matriz_relacao[4, 5] = matriz_relacao[4, 6] = matriz_relacao[4, 7] = matriz_relacao[4, 9] = matriz_relacao[4, 10] = matriz_relacao[4, 11] = MEDIA;

        matriz_relacao[5, 0] = matriz_relacao[5, 2] = matriz_relacao[5, 3] = matriz_relacao[5, 4] = matriz_relacao[5, 6] = matriz_relacao[5, 7] = matriz_relacao[5, 8] = matriz_relacao[5, 10] = matriz_relacao[5, 11] = MEDIA;

        matriz_relacao[6, 0] = matriz_relacao[6, 1] = matriz_relacao[6, 3] = matriz_relacao[6, 4] = matriz_relacao[6, 5] = matriz_relacao[6, 7] = matriz_relacao[6, 8] = matriz_relacao[6, 9] = matriz_relacao[6, 11] = MEDIA;

        matriz_relacao[7, 0] = matriz_relacao[7, 1] = matriz_relacao[7, 2] = matriz_relacao[7, 4] = matriz_relacao[7, 5] = matriz_relacao[7, 6] = matriz_relacao[7, 8] = matriz_relacao[7, 9] = matriz_relacao[7, 10] = MEDIA;

        matriz_relacao[8, 0] = matriz_relacao[8, 4] = ALTA;

        matriz_relacao[9, 1] = matriz_relacao[9, 5] = ALTA;

        matriz_relacao[10, 2] = matriz_relacao[10, 6] = ALTA;

        matriz_relacao[11, 3] = matriz_relacao[11, 7] = ALTA;

        for (int i = 0; i < num_entidades; i++)
        {
            if (i == 12) continue;
            matriz_relacao[12, i] = MEDIA;
            matriz_relacao[i, 12] = MEDIA;
        }

        matriz_relacao[13, 12] = MUITO_ALTA;

        // Adicionando relações semi-aleatórias
        int num_relacoes_aleatorias = 12;
        System.Random random = new System.Random();

        for (int count = 0; count < num_relacoes_aleatorias; count++)
        {
            int i = random.Next(0, num_entidades); // Escolhendo aleatoriamente um atacante
            int j = random.Next(0, num_entidades); // Escolhendo aleatoriamente um alvo

            // Garantindo que não estamos modificando relações previamente definidas
            while (matriz_relacao[i, j] > 0)
            {
                i = random.Next(0, num_entidades);
                j = random.Next(0, num_entidades);
            }

            matriz_relacao[i, j] = (float)random.NextDouble() < 0.5 ? BAIXA : MEDIA;
        }

        // Adicionando perturbação aleatória
        float perturbacao = 0.1f;
        for (int i = 0; i < num_entidades; i++)
        {
            for (int j = 0; j < num_entidades; j++)
            {
                if (matriz_relacao[i, j] > 0)
                {
                    float delta = (float)(random.NextDouble() * 2 * perturbacao - perturbacao);
                    matriz_relacao[i, j] = Mathf.Max(0, matriz_relacao[i, j] + delta);
                }
            }
        }

        // remove self loop
        matriz_relacao[0, 0] = 0;
        matriz_relacao[1, 1] = 0;
        matriz_relacao[2, 2] = 0;
        matriz_relacao[3, 3] = 0;
        matriz_relacao[12, 12] = 0;
        // remove situações impossiveis
        matriz_relacao[4, 0] = 0;
        matriz_relacao[5, 1] = 0;
        matriz_relacao[6, 2] = 0;
        matriz_relacao[7, 3] = 0;

        for (int i = 0; i < num_entidades; i++)
        {
            BaseActor.ActorType type1 = (BaseActor.ActorType)i;
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}\t = \t", type1.ToString().PadRight(15));
            for (int j = 0; j < num_entidades; j++)
            {
                BaseActor.ActorType type2 = (BaseActor.ActorType)j;
                sb.AppendFormat("{0:F3} \t", matriz_relacao[i, j]);
                UpdateRageFactor(type1, type2, matriz_relacao[i, j]);
            }
            // Debug.Log(sb.ToString());
        }
    }

    private void Start()
    {
        this.rageRelations = new Dictionary<(BaseActor.ActorType, BaseActor.ActorType), float>();
        InitializeRageMatrix();
    }
}
