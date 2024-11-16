using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] protected int m_Max;
    [SerializeField] protected int m_Current;
    [SerializeField] protected bool m_ResetOnStart;

    [Tooltip("Notifies listeners of updated value")]
    public UnityEvent<int> HealthChanged;

    [Tooltip("Notifies listeners that this object has recieved a hit")]
    public UnityEvent OnHit;

    [Tooltip("Notifies listeners that this object has died")]
    public UnityEvent Died;

    public int Max => m_Max;
    public int Current => m_Current;
    public bool ResetOnStart => m_ResetOnStart;

    private bool m_IsDead;
    private bool m_IsInvulnerable;

    private void Awake()
    {
        if (m_ResetOnStart)
            m_Current = Max;
    }

    private void Start()
    {
    }

    public virtual void Remove(int amount)
    {
        // If already dead, do nothing
        if (m_IsDead || m_IsInvulnerable)
            return;

        m_Current -= amount;

        if (m_Current <= 0)
        {
            m_Current = 0;
        }

        OnHit?.Invoke();
        HealthChanged?.Invoke(amount);

        // Check for death condition.
        if (m_Current <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Only die once
        if (m_IsDead)
            return;

        m_IsDead = true;
        Died?.Invoke();
    }

    public void SetInvulnerable(bool invulnerable)
    {
        m_IsInvulnerable = invulnerable;
    }
}
