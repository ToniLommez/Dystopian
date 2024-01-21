using UnityEngine;
using System.Collections;

public class IntelligentEnemy : Enemy
{
    public NeuralNetwork NN;

    // Parâmetros da Rede Neural
    private const int NumberOfInputs = 7; // Modifique conforme as entradas necessárias
    private const int NumberOfOutputs = 6; // Esquerda, Direita, Cima, Baixo, Atirar, Wander
    private int[] Layers = new int[] { 7, 10, 10, 6 }; // Exemplo: 1 camada oculta

    private float[] lastInputs;
    private float[] lastOutputs;

    private Vector2 lastScorePosition;

    public enum EnemyAction
    {
        LEFT,
        RIGHT,
        UP,
        DOWN,
        ATTACK,
        WANDER
    }
    public EnemyAction decision = EnemyAction.LEFT;

    protected override void Awake()
    {
        isInteligent = true;
        base.Awake();
        NN = new NeuralNetwork(Layers);
        lastScorePosition = transform.position;
    }

    protected override void Update()
    {
        base.Update();

        if (!Rage)
        {
            MakeIntelligentDecision();
        }

        // Verifica se a distância percorrida desde a última posição registrada é maior ou igual a 1
        if (Vector2.Distance(transform.position, lastScorePosition) >= 1f)
        {
            // Adiciona pontos ao Score
            Score += 1; // Ou outro valor de pontuação que você deseja adicionar

            // Atualiza a última posição onde pontos foram adicionados
            lastScorePosition = transform.position;
        }
    }

    private void MakeIntelligentDecision()
    {
        // Obter entradas
        float[] inputs = GetInputs();
        lastInputs = inputs;

        // Processar na rede neural
        float[] outputs = NN.FeedForward(inputs);
        lastOutputs = outputs;

        // Tomar ação baseada na saída da rede neural
        ProcessOutputs(outputs);
    }

    private float[] GetInputs()
    {
        float[] inputs = new float[NumberOfInputs];

        // Raycast para cada direção - esquerda, direita, cima, baixo
        inputs[0] = RaycastDistance(Vector2.left);
        inputs[1] = RaycastDistance(Vector2.right);
        inputs[2] = RaycastDistance(Vector2.up);
        inputs[3] = RaycastDistance(Vector2.down);

        // Vida e distância do inimigo alvo
        if (currentTarget != null && targetHealthSystem != null)
        {
            inputs[4] = targetHealthSystem.Health / targetHealthSystem.MaxHealth;
            Vector2 toTarget = (currentTarget.transform.position - transform.position);
            inputs[5] = toTarget.magnitude / ViewDistance;
            inputs[6] = TargetGroupMenaceLevel;
        }
        else
        {
            inputs[4] = 0;
            inputs[5] = 1;
            inputs[6] = 0;
        }

        return inputs;
    }

    private float RaycastDistance(Vector2 direction)
    {
        // Realiza o raycast
        int layerMask = LayerMask.GetMask("Water");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, ViewDistance, layerMask);

        // Calcula a distância real ou a distância máxima de visão
        float rayDistance = hit.collider != null ? hit.distance : ViewDistance;

        // Desenha o raio azul para visualização
        Debug.DrawRay(transform.position, direction.normalized * rayDistance, Color.blue, 0.1f);

        // Retorna a distância normalizada
        return hit.collider != null ? hit.distance / ViewDistance : 1f;
    }

    private void ProcessOutputs(float[] outputs)
    {
        int maxIndex = 0;
        float maxValue = outputs[0];
        for (int i = 1; i < outputs.Length; i++)
        {
            if (outputs[i] > maxValue)
            {
                maxIndex = i;
                maxValue = outputs[i];
            }
        }

        switch ((EnemyAction)maxIndex)
        {
            case EnemyAction.LEFT:
                decision = EnemyAction.LEFT;
                MoveInDirection(Vector2.left);
                break;
            case EnemyAction.RIGHT:
                decision = EnemyAction.RIGHT;
                MoveInDirection(Vector2.right);
                break;
            case EnemyAction.UP:
                decision = EnemyAction.UP;
                MoveInDirection(Vector2.up);
                break;
            case EnemyAction.DOWN:
                decision = EnemyAction.DOWN;
                MoveInDirection(Vector2.down);
                break;
            case EnemyAction.ATTACK:
                decision = EnemyAction.ATTACK;
                if (currentTarget != null) { ChaseAndAttackTarget(currentTarget, targetHealthSystem); }
                break;
            case EnemyAction.WANDER:
                decision = EnemyAction.WANDER;
                Wander();
                break;
        }
    }

    private void MoveInDirection(Vector2 direction)
    {
        rb.velocity = direction.normalized * Speed * Time.fixedDeltaTime;
        shouldFlip(direction);
    }

    public void PrintNeuralNetwork()
    {
        string str = "In |||| Raycast = ";
        str += "←: " + lastInputs[0].ToString("F3") + " ";
        str += "→: " + lastInputs[1].ToString("F3") + " ";
        str += "↑: " + lastInputs[2].ToString("F3") + " ";
        str += "↓: " + lastInputs[3].ToString("F3") + " || ";

        str += "Target.Health: " + lastInputs[4].ToString("F3") + " ";
        str += "Target.Distance: " + lastInputs[5].ToString("F3") + " ";
        str += "Target.Menace: " + lastInputs[6].ToString("F3") + " ";

        str += "========= Out |||| ";
        str += "←: " + lastOutputs[0].ToString("F3") + " ";
        str += "→: " + lastOutputs[1].ToString("F3") + " ";
        str += "↑: " + lastOutputs[2].ToString("F3") + " ";
        str += "↓: " + lastOutputs[3].ToString("F3") + " ";
        str += "Shoot: " + lastOutputs[4].ToString("F3") + " ";
        str += "Wander: " + lastOutputs[5].ToString("F3") + " ";

        str += "|||| decision: " + decision + " ";

        str += "|| SCORE = " + Score;

        Debug.Log(str);
    }
}