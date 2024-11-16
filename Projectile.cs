using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private int m_damage;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.gameObject.TryGetComponent(out PlayerManager playerManager))
        {
            if (playerManager.ParryEnabled)
            {
                playerManager.ParrySuccess();
            }
            else
            {
                Health health = collision.gameObject.GetComponent<Health>();
                health.Remove(m_damage);
            }
        }

        Destroy(gameObject);
    }
}
