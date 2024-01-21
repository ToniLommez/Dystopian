using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : BaseEnemy
{
    public GameObject currentTarget;
    public HealthSystem targetHealthSystem;
    public float TargetGroupMenaceLevel;

    public int Damage = 10;
    public int ShootDamage = 10;
    public bool CanShoot = true;

    public float damageRate = 2;
    public float shootRate = 3;
    public float StopDistance = 1.1f;
    public float MeleeDistance = 1.5f;
    public float ShootDistance = 5f;

    protected float nextDamageTime = 0f;
    protected float nextShootTime = 0f;

    public float rageThreshold = 0.07f;
    public bool Rage = false;
    private float lastTimeTargetSeen = 0f;
    private float forgivenessTime = 30f;

    protected bool isInteligent = false;
    public bool isChasing = false;
    private List<Node> currentPath;
    private float lastPathUpdateTime = 0f;
    private float pathUpdateCooldown = 1f;

    private AStarPathfinder pathfinder;

    protected override void Awake()
    {
        pathfinder = new AStarPathfinder(GameManager.Instance.MapGenerator.Grid);
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();
        SelectTarget();
        if (Rage)
        {
            MakeDecision();
        }
    }

    protected virtual void SelectTarget()
    {
        // Atualizar o tempo se o alvo estiver visível
        if (visibleTargets.Contains(currentTarget))
        {
            lastTimeTargetSeen = Time.time;
        }

        // Verifica se o alvo saiu do campo de visão e se passou o tempo de perdão
        if (Rage && (!visibleTargets.Contains(currentTarget) && Time.time - lastTimeTargetSeen > forgivenessTime))
        {
            pathfinder.PathDebug = null;
            pathfinder.ClosedListDebug = null;
            Rage = false;
            currentTarget = null;
            targetHealthSystem = null;
            return;
        }

        GameObject targetCandidate = null;
        float bestAttackScore = 0f;

        foreach (var target in visibleTargets)
        {
            if (target == null) { continue; }
            float dislikeFactor = GameManager.Instance.RageService.GetRageFactor(this, target.GetComponent<BaseActor>());
            float distance = Vector3.Distance(transform.position, target.transform.position);
            float attackScore = dislikeFactor / distance;

            // Adicionar alvos à fila se atenderem a um certo limite de pontuação de ataque
            if (attackScore > bestAttackScore)
            {
                bestAttackScore = attackScore;
                targetCandidate = target;
            }
        }

        if (bestAttackScore > rageThreshold)
        {
            this.currentTarget = targetCandidate;
            this.targetHealthSystem = targetCandidate.GetComponent<HealthSystem>();

            var baseEnemy = currentTarget.GetComponent<BaseEnemy>();
            if (baseEnemy != null && baseEnemy.Type != BaseActor.ActorType.Player)
            {
                this.TargetGroupMenaceLevel = (float)baseEnemy.GroupMenaceLevel;
            }
            else
            {
                this.TargetGroupMenaceLevel = 0.0f;
            }
        }
    }

    protected void MakeDecision()
    {
        if (!CanMove || !Alive) return;

        if (currentTarget == null || targetHealthSystem == null || !targetHealthSystem.Alive)
        {
            Rage = false;
        }
        else
        {
            ChaseAndAttackTarget(currentTarget, targetHealthSystem);
        }
    }

    protected void ChaseAndAttackTarget(GameObject currentTarget, HealthSystem targetHealthSystem)
    {
        float distance = Vector3.Distance(transform.position, currentTarget.transform.position);

        if (Rage && !isChasing)
        {
            ChaseUsingAStar(currentTarget.transform.position);
        }
        if (distance < ShootDistance)
        {
            if (CanShoot) TryShooting(currentTarget.transform.position - transform.position);
        }
        if (distance < MeleeDistance) TryMeleeAttack(targetHealthSystem);
    }

    private void ChaseUsingAStar(Vector3 targetPosition)
    {
        if (isChasing || Time.time - lastPathUpdateTime < pathUpdateCooldown)
        {
            return; // Ainda não é hora de atualizar o caminho
        }

        isChasing = true;

        // Converter as posições do mundo para posições do grid
        Vector2Int start = WorldToGrid(transform.position);
        Vector2Int end = WorldToGrid(targetPosition);

        // Obter o caminho usando A*
        List<Node> path = pathfinder.FindPath(start, end);

        // Siga o caminho
        if (path != null && path.Count > 0)
        {
            lastPathUpdateTime = Time.time; // Atualiza o tempo após calcular o caminho
            StartCoroutine(FollowPath(path));
        }
    }


    private Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        // Converte posição do mundo para posição do grid
        // Ajuste essa função de acordo com a escala e posicionamento do seu grid
        int x = Mathf.FloorToInt(worldPosition.x);
        int y = Mathf.FloorToInt(worldPosition.y);
        return new Vector2Int(x, y);
    }

    private IEnumerator FollowPath(List<Node> path)
    {
        float startTime = Time.time;
        foreach (Node node in path)
        {
            Vector3 targetPosition = new Vector3(node.Position.x + 0.5f, node.Position.y + 0.5f, 0);

            // Continue se movendo até chegar próximo do ponto alvo
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                // Verifica se o tempo de seguimento excedeu o pathUpdateCooldown
                if (Time.time - startTime > pathUpdateCooldown)
                {
                    rb.velocity = Vector2.zero;
                    isChasing = false;
                    yield break; // Interrompe a coroutine
                }

                Vector2 direction = (targetPosition - transform.position).normalized;
                shouldFlip(direction);
                rb.velocity = direction * Speed * Time.fixedDeltaTime;

                yield return null;
            }
        }

        // Parar o inimigo quando chegar no destino
        rb.velocity = Vector2.zero;
        isChasing = false; // A perseguição terminou
    }

    void OnDrawGizmos()
    {
        if (pathfinder != null)
        {
            /* if (pathfinder.ClosedListDebug != null)
            {
                Gizmos.color = Color.red;
                foreach (var node in pathfinder.ClosedListDebug)
                {
                    Gizmos.DrawCube(new Vector3(node.Position.x + 0.5f, node.Position.y + 0.5f, 0), Vector3.one * 1f);
                }
            } */

            if (pathfinder.PathDebug != null)
            {
                Gizmos.color = Color.black;
                foreach (var node in pathfinder.PathDebug)
                {
                    Gizmos.DrawCube(new Vector3(node.Position.x + 0.5f, node.Position.y + 0.5f, 0), Vector3.one * 1f);
                }
            }
        }
    }

    protected void TryShooting(Vector2 direction)
    {
        if (Time.time >= nextShootTime)
        {
            Score += 1;
            shooter.Shoot(direction);
            nextShootTime = Time.time + (shootRate * Random.Range(0.5f, 1.5f));
        }
    }

    protected void TryShooting()
    {
        var direction = (currentTarget.transform.position - transform.position);
        if (Time.time >= nextShootTime)
        {
            shooter.Shoot(direction);
            nextShootTime = Time.time + (shootRate * Random.Range(0.5f, 1.5f));
        }
    }

    protected void TryMeleeAttack(HealthSystem targetHealthSystem)
    {
        if (Time.time >= nextDamageTime)
        {
            Score += 25;
            DealDamage(targetHealthSystem);
            nextDamageTime = Time.time + damageRate;
        }
    }

    protected void DealDamage(HealthSystem targetHealthSystem)
    {
        if (CanMove && Time.time >= nextDamageTime)
        {
            if (!targetHealthSystem.TakeDamage(Damage, this))
            {
                Rage = false;
                currentTarget = null;
                targetHealthSystem = null;
            }
        }
    }

    public override void Die(ActorType killer)
    {
        if (TypeGroup == ActorTypeGroup.Human && GetActorCategory(killer) == ActorTypeGroup.Monster)
        {
            GameObject novoInimigo = GameManager.Instance.EnemyManager.RandomEnemyRuleTile.GetEnemy(killer);
            Instantiate(novoInimigo, transform.position, Quaternion.identity);
            DestroyTime = 0;
        }
        AllEnemies.RemoveAll(x => x == this);
        base.Die(killer);
    }

    public override void TakeDamage(BaseActor killer)
    {
        Rage = true;
        currentTarget = killer.gameObject;
        targetHealthSystem = currentTarget.GetComponent<HealthSystem>();
        base.TakeDamage(killer);
    }
}
