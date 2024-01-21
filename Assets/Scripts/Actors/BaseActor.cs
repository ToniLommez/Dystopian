using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseActor : MonoBehaviour
{
    public enum ActorType
    {
        Bismuto,
        Automato,
        Corrompido,
        Cultista,
        Ciborgue,
        Biomecanico,
        Infectado,
        Xenomorfo,
        Cyberpunk,
        Pistoleiro,
        Templario,
        PirataEspacial,
        Player,
        Aparicao,
        Basilisco,
        Cthulhu,
        LichDragon,
        Grifo,
        Zeit,
    }

    public enum ActorTypeGroup
    {
        Monster,
        Humanoid,
        Human,
        Player,
        Boss,
    }

    public ActorType Type;
    public ActorTypeGroup TypeGroup { get; protected set; }

    protected HealthSystem healthSystem;
    protected Animator animator;
    protected Rigidbody2D rb;
    protected Shooter shooter;
    protected float DestroyTime = 5f;

    [HideInInspector]
    public bool CanMove = true;
    [HideInInspector]
    public bool Alive = true;

    public int Score = 0;

    protected virtual void Awake()
    {
        shooter = GetComponent<Shooter>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        healthSystem = GetComponent<HealthSystem>();
        TypeGroup = GetActorCategory(Type);
    }

    public virtual void Die(ActorType killer)
    {
        Score -= 5;
        Alive = false;
        CanMove = false;
        animator.SetTrigger("died");
        Destroy(gameObject, DestroyTime);
    }

    public virtual void InstantDie()
    {
        Alive = false;
        CanMove = false;
        Destroy(gameObject, 0);
    }

    public virtual void TakeDamage(BaseActor killer)
    {
        Score -= 1;
        StartCoroutine(DelayedMove());
    }

    public virtual void DoDamage()
    {
        Score += 5;
    }

    public virtual void KilledSomeone()
    {
        Score += 10;
    }

    private IEnumerator DelayedMove()
    {
        CanMove = false;
        yield return new WaitForSeconds(1f);
        CanMove = true;
    }

    public ActorTypeGroup GetActorCategory(ActorType type)
    {
        switch (type)
        {
            case ActorType.Bismuto:
            case ActorType.Automato:
            case ActorType.Corrompido:
            case ActorType.Cultista:
            case ActorType.Aparicao:
                return ActorTypeGroup.Monster;
            case ActorType.Ciborgue:
            case ActorType.Biomecanico:
            case ActorType.Infectado:
            case ActorType.Xenomorfo:
                return ActorTypeGroup.Humanoid;
            case ActorType.Cyberpunk:
            case ActorType.Pistoleiro:
            case ActorType.Templario:
            case ActorType.PirataEspacial:
                return ActorTypeGroup.Human;
            case ActorType.Player:
                return ActorTypeGroup.Player;
            case ActorType.Basilisco:
            case ActorType.Cthulhu:
            case ActorType.LichDragon:
            case ActorType.Grifo:
            case ActorType.Zeit:
                return ActorTypeGroup.Boss;
            default:
                Debug.LogError("ActorType não reconhecido!");
                return ActorTypeGroup.Boss;
        }
    }
}
