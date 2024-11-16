using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] private FireDirection _attackDirection;
    [SerializeField] private GameObject _projectile;

    private void Start()
    {
        InvokeRepeating(nameof(SpawnProjectile), 1f, 10f);
    }

    private void SpawnProjectile()
    {
        GameObject go = Instantiate(_projectile, transform.position, Quaternion.identity);
        Rigidbody2D rb = go.GetComponent<Rigidbody2D>();
        FireProjectile(rb);
    }

    private void FireProjectile(Rigidbody2D rb)
    {
        switch (_attackDirection)
        {
            case FireDirection.Up:
                rb.velocity += Vector2.up * 5f;
                break;
            case FireDirection.Down:
                rb.velocity += Vector2.down * 5f;
                break;
            case FireDirection.Left:
                rb.velocity += Vector2.left * 5f;
                break;
            case FireDirection.Right:
                rb.velocity += Vector2.right * 5f;
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Health health = collision.gameObject.GetComponent<Health>();
            health.Remove(2);
        }
    }

    public void OnDie()
    {
        Destroy(gameObject);
    }
}

public enum FireDirection
{
    Right,
    Left,
    Up,
    Down
}
