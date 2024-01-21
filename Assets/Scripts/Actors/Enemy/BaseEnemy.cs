using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseEnemy : BaseActor
{
    [HideInInspector]
    public static List<BaseEnemy> AllEnemies = new();
    public static GameObject Player { get; private set; }

    protected SpriteRenderer spriteRenderer;

    // Logica de visao
    // [HideInInspector]
    public List<GameObject> visibleTargets = new List<GameObject>();
    public static float visionUpdateTime = 0.5f;
    protected float lastVisionUpdate = 0f;
    protected float ViewDistance = 10f;

    [HideInInspector]
    public int Label;
    private static int nextLabel;

    public double GroupMenaceLevel = 0.0f;

    public static void InitLabels()
    {
        nextLabel = 0;
        foreach(var enemy in AllEnemies)
        {
            enemy.Label = nextLabel++;
        }
    }

    public void UpdateLabel()
    {
        Dictionary<int, int> existingLabels = new Dictionary<int, int>();

        foreach(var target in visibleTargets)
        {
            if(target == null)
            {
                continue;
            }

            var baseEnemy = target.GetComponent<BaseEnemy>();
            
            if(baseEnemy == null || GameManager.Instance.RageService.GetRageFactor(this, baseEnemy) <= 0)
            {
                continue;
            }

            if(baseEnemy.Type != BaseActor.ActorType.Player)
            {
                if(existingLabels.TryGetValue(baseEnemy.Label, out int value))
                {
                    existingLabels[baseEnemy.Label] = value + 1;
                }
                else
                {
                    existingLabels[baseEnemy.Label] = 1;
                }
            }
        }

        // if(visibleTargets.Count > 0)
        // {
        //     Debug.Log("visibleTargets: " + visibleTargets.Count);
        // }

        if(existingLabels.Count > 0)
        {
            // string str = "oldLabel: " + Label + " ";
            Label = existingLabels.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
            // Debug.Log(str + "newLabel: " + Label);
        }
    }

    protected void Start()
    {
        base.Awake();
        AllEnemies.Add(this);
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (Player == null)
        {
            Player = GameObject.FindWithTag("Player");
        }
    }

    protected virtual void Update()
    {
        isMoving();
        if (Time.time >= lastVisionUpdate + visionUpdateTime * Random.Range(.5f, 1.5f))
        {
            UpdateVision();
            lastVisionUpdate = Time.time;
        }
    }

    private void UpdateVision()
    {
        visibleTargets.Clear();
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, ViewDistance);
        foreach (var hitCollider in hitColliders)
        {
            // Verifica se o objeto não é ele mesmo e se tem o componente BaseActor
            if (hitCollider.gameObject != gameObject && hitCollider.gameObject.GetComponent<BaseActor>() != null)
            {
                visibleTargets.Add(hitCollider.gameObject);
            }
        }
    }

    /* private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, ViewDistance);
    } */

    // Andar
    public enum ModoDeMovimento
    {
        Parado,
        AndaEPara,
        SoAnda
    }
    public ModoDeMovimento modoDeMovimento = ModoDeMovimento.AndaEPara;
    public float Speed = 80f;
    public float WanderSpeed = 15f;
    private Vector2 randomDirection;
    private float randomDirectionUpdateTime = 0f;
    private float randomDirectionChangeInterval = 5f;
    private float stateUpdateTime = 0f;
    private float stateChangeInterval = 5f;
    private bool estaAndando = true;

    private void UpdateRandomDirection()
    {
        float randomAngle = Random.Range(0f, 360f);
        randomDirection = new Vector2(Mathf.Cos(randomAngle * Mathf.Deg2Rad), Mathf.Sin(randomAngle * Mathf.Deg2Rad));
    }

    protected void Wander()
    {
        if (modoDeMovimento == ModoDeMovimento.Parado) { return ;}

        if (Time.time >= randomDirectionUpdateTime)
        {
            UpdateRandomDirection();
            randomDirectionUpdateTime = Time.time + randomDirectionChangeInterval;
        }

        if (estaAndando)
        {
            rb.velocity = randomDirection * WanderSpeed * Time.fixedDeltaTime;
            // shouldFlip(randomDirection);
        }

        if (Time.time >= stateUpdateTime && modoDeMovimento == ModoDeMovimento.AndaEPara)
        {
            estaAndando = !estaAndando;
            stateUpdateTime = Time.time + (stateChangeInterval * Random.Range(0.5f, 1.5f));
        }
    }

    protected void Chase(Vector2 direction)
    {
        rb.velocity = direction.normalized * (Speed * Time.fixedDeltaTime);
        shouldFlip(direction);
    }

    public void shouldFlip(Vector2 direction)
    {
        if (direction.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (direction.x < 0)
        {
            spriteRenderer.flipX = true;
        }
    }

    protected void isMoving()
    {
        var move = rb.velocity;

        if (move.x != 0 || move.y != 0)
        {
            animator.SetBool("IsMoving", true);
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }
    }
}
