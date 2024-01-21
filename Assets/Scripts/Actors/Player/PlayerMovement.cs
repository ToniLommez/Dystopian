using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    public float speed = 5f;
    private Vector2 _direction;
    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");
        _direction = new Vector2(horizontal, vertical);
    }

    private void FixedUpdate()
    {
        _rb.velocity = _direction.normalized * (speed * Time.fixedDeltaTime);
    }
}