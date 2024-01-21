using UnityEngine;
using System;
using System.Collections;

public class HealthSystem : MonoBehaviour
{
    public int Health, MaxHealth = 10;
    public int Defence, MaxDefence = 10;
    public int Stamina, MaxStamina = 10;
    [HideInInspector]
    public bool Alive;

    public Animator animator;
    public float damageDelay = 0.5f;
    private float nextDamageTime = 0f;
    public float damageAnimationDuration = 0.5f;
    private BaseActor actor;

    private void Awake()
    {
        Health = MaxHealth;
        animator = GetComponent<Animator>();
        actor = GetComponent<BaseActor>();
        Alive = true;
    }

    public bool TakeDamage(int damage, BaseActor killer)
    {
        if (!Alive) return false;
        if (Time.time < nextDamageTime) return true;

        int true_damage = (int)Mathf.Min(damage, Health);
        Health -= true_damage;

        actor.TakeDamage(killer);

        // Ative a animação de dano.
        if (Health > 0)
        {
            nextDamageTime = Time.time + damageDelay;
            animator.SetTrigger("damage");
            actor.CanMove = false;
            StartCoroutine(ResetDamageAnimationAfterDelay(damageAnimationDuration));
            return true;
        }
        else
        {
            killer.KilledSomeone();
            Die(killer.Type);
            return false;
        }
    }

    private IEnumerator ResetDamageAnimationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        actor.CanMove = true;
    }

    public int Heal(int amount)
    {
        var true_amount = (int)Mathf.Min(amount, MaxHealth - Health);
        Health += true_amount;
        return true_amount;
    }

    public bool setHealth(int amount)
    {
        Health = (int)Mathf.Min(amount, MaxHealth);
        return Health == MaxHealth;
    }

    private void Die(BaseActor.ActorType killer)
    {
        Alive = false;
        actor.Die(killer);
    }
}
