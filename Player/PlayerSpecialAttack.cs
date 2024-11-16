using UnityEngine;

public class PlayerSpecialAttack : PlayerAttack
{
    protected override void OnTriggerEnter2D(Collider2D col)
    {
        base.OnTriggerEnter2D(col);

        Destroy(gameObject);
    }
}
