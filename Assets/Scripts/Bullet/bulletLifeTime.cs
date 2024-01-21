using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletLifeTime : MonoBehaviour
{
    public float LifeTime;
    private float StartTime;
    public BaseActor Shooter;
    public int _bulletDamage; // Dano causado pela bala

    void Start()
    {
        StartTime = Time.time;
    }

    void FixedUpdate()
    {
        if (Time.time >= StartTime + LifeTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Se a bala colidir com seu atirador, simplesmente retorne e não faça mais nada
        if (Shooter == null) { return; }
        if (collision.gameObject == Shooter.gameObject) { return; }

        HealthSystem enemyHealth = collision.gameObject.GetComponent<HealthSystem>();

        if (enemyHealth != null)
        {
            Shooter.DoDamage();
            enemyHealth.TakeDamage(_bulletDamage, Shooter);
            Destroy(gameObject);
        }
    }
}
