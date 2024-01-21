using System;
using UnityEngine;


public class Shooter : MonoBehaviour
{
    public Transform shootingPoint;
    public GameObject bullet;
    public float bulletSpeed;
    public float fireRate = 0.5f;
    private float nextFireTime = 0f;
    private Transform target;
    public float changeTargetDelay = 0.5f;
    private float nextTargetChangeTime = 0f;
    public float DistanceToShoot = 10f;
    public int damage = 1;

    private Transform GetClosestEnemy(BaseEnemy[] targets)
    {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (Enemy t in targets)
        {
            if (!t.Alive) continue;
            float dist = Vector3.Distance(t.transform.position, currentPos);
            if (dist < minDist && dist <= DistanceToShoot)
            {
                tMin = t.transform;
                minDist = dist;
            }
        }
        return tMin;
    }

    public void Shoot(string Target)
    {
        if (fireRate > 0 && Time.time < nextFireTime) return;
        if (Time.time >= nextTargetChangeTime)
        {
            switch (Target)
            {
                case "Enemy":
                    target = GetClosestEnemy(BaseEnemy.AllEnemies.ToArray());
                    nextTargetChangeTime = Time.time + changeTargetDelay;
                    break;
                default:
                    break;
            }
        }

        if (target == null) return;

        GameObject bulletClone = Instantiate(bullet, shootingPoint.position, Quaternion.identity);
        bulletLifeTime bulletScript = bulletClone.GetComponent<bulletLifeTime>();
        

        PlayerController player = GetComponent<PlayerController>();
        bulletScript.Shooter = player;
        bulletScript._bulletDamage = player.ShootDamage;

        Vector2 direction = (target.position - shootingPoint.position).normalized;

        Rigidbody2D bulletRB = bulletClone.GetComponent<Rigidbody2D>();
        bulletRB.AddForce(bulletSpeed * direction, ForceMode2D.Impulse);
        nextFireTime = Time.time + fireRate;
    }

    public void Shoot(Vector3 direction)
    {
        if (fireRate > 0 && Time.time < nextFireTime) return;
        if (Time.time < nextTargetChangeTime) return;

        GameObject bulletClone = Instantiate(bullet, shootingPoint.position, Quaternion.identity);
        bulletLifeTime bulletScript = bulletClone.GetComponent<bulletLifeTime>();

        Enemy shooter = GetComponent<Enemy>();
        bulletScript.Shooter = shooter;
        bulletScript._bulletDamage = shooter.ShootDamage;

        // Usamos a direção fornecida como parâmetro
        Vector2 shootDirection = direction.normalized;

        Rigidbody2D bulletRB = bulletClone.GetComponent<Rigidbody2D>();
        bulletRB.AddForce(bulletSpeed * shootDirection, ForceMode2D.Impulse);
        nextFireTime = Time.time + fireRate;
    }
}
