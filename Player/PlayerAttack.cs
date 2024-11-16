using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] protected int _attackDamage;

    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        if (col.TryGetComponent(out Health targetHealth))
        {
            targetHealth.Remove(_attackDamage);
        }
    }
}
