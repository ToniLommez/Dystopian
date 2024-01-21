using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D), typeof(HealthSystem), typeof(Shooter))]
public class PlayerController : BaseActor
{
    public Joystick joystick;
    public float moveSpeed;
    private Vector2 move;

    public BarraHUD lifeBar, defBar, staminBar;

    public int ShootDamage = 10;

    public bool GodMode = false;

    protected override void Awake()
    {
        base.Awake();

        lifeBar.Init(healthSystem.MaxHealth);
    }

    private void Update()
    {
        if (!CanMove)
        {
            move.x = 0;
            move.y = 0;
            return;
        };

        move.x = joystick.Horizontal;
        move.y = joystick.Vertical;
        move.x = Input.GetAxisRaw("Horizontal");
        move.y = Input.GetAxisRaw("Vertical");

        if (move.x != 0)
        {
            transform.localScale = new Vector3(move.x > 0 ? 1 : -1, 1, 1);
        }
        
        
        if (move.x != 0 || move.y != 0)
        {
            move = move.normalized;
            animator.SetBool("IsMoving", true);
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }

        if ((Input.GetMouseButtonDown(0) && Input.mousePosition.x >= Screen.width / 2) || Input.GetButtonDown("Jump"))
        {
            shooter.Shoot("Enemy");
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            healthSystem.Health = healthSystem.MaxHealth;
            lifeBar.SetVal(healthSystem.Health);
        }

        if(Input.GetKeyDown(KeyCode.G))
        {
            GodMode = !GodMode;   
        }

        if(GodMode)
        {
            healthSystem.Health = healthSystem.MaxHealth;
            lifeBar.SetVal(healthSystem.Health);
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = move * (moveSpeed * Time.fixedDeltaTime);
    }

    public override void TakeDamage(BaseActor killer)
    {
        lifeBar.SetVal(healthSystem.Health);
    }

    public override void Die(ActorType killer)
    {
        StartCoroutine(GameOver());
        base.Die(killer);
    }

    private IEnumerator GameOver()
    {
        yield return new WaitForSeconds(DestroyTime*3/5);
        GameManager.Instance.GameOver(Score);
    }
}
