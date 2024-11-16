using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private Sprite _specialEffect1;
    [SerializeField] private Sprite _specialEffect2;
    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] private SpriteRenderer _specialEffect;
    [SerializeField] private float _invulnerabilityDuration = 1.0f;
    [SerializeField] private float _blinkInterval = 0.1f;
    [SerializeField] private AudioClip _parrySound;
    [SerializeField] private AudioClip _parrySuccessSound;

    [Tooltip("Notifies listeners that this object has done a parry")]
    public UnityEvent OnParry;

    public bool ParryEnabled { get; private set; }
    public bool SpecialEnabled { get; private set; }

    private Health _health;
    private CapsuleCollider2D _collider;
    private AudioSource _audioSource;
    private ScreenShake _screenShake;
    private PlayerAnimator _playerAnimator;
    private PlayerController _playerController;

    private void Awake()
    {
        _health = GetComponent<Health>();
        _collider = GetComponent<CapsuleCollider2D>();
        _audioSource = GetComponentInChildren<AudioSource>();
        _screenShake = Camera.main.GetComponent<ScreenShake>();
        _playerController = GetComponent<PlayerController>();
        _playerAnimator = GetComponentInChildren<PlayerAnimator>();
        SpecialEnabled = false;
    }

    private void Start()
    {
        _playerController.Parry += ParryEnable;
        _playerAnimator.ParryEnded += ParryDisable;
    }

    private void ParryEnable()
    {
        ParryEnabled = true;
        _audioSource.PlayOneShot(_parrySound, 0.2f);
        _collider.size = new Vector2(1.1f, 1.55f);
    }

    public void ParryDisable()
    {
        ParryEnabled = false;
        _collider.size = new Vector2(0.35f, 1.55f);
    }

    public void ParrySuccess()
    {
        OnParry?.Invoke();
        SpecialEnabled = true;
        _specialEffect.enabled = true;
        InvokeRepeating("SpecialEffectAnim", 0.1f, 0.1f);
        _audioSource.PlayOneShot(_parrySuccessSound, 0.2f);
        _screenShake.TriggerShake(0.1f);
    }

    private void SpecialEffectAnim()
    {
        _specialEffect.sprite = _specialEffect.sprite == _specialEffect1 ? _specialEffect2 : _specialEffect1;
    }

    public void RemoveSpecialAttack()
    {
        CancelInvoke("SpecialEffectAnim");
        SpecialEnabled = false;
        _specialEffect.enabled = false;
    }

    public void TakeDamage()
    {
        _screenShake.TriggerShake(0.1f);
        _health.SetInvulnerable(true);
        StartCoroutine(InvulnerabilityCoroutine());
    }

    private IEnumerator InvulnerabilityCoroutine()
    {
        float timeElapsed = 0f;

        // Boucle pendant la durée de l'invulnérabilité
        while (timeElapsed < _invulnerabilityDuration)
        {
            // Alterne l'opacité du sprite (actif/inactif)
            _sprite.enabled = !_sprite.enabled;

            // Attend un petit délai avant de changer à nouveau
            yield return new WaitForSeconds(_blinkInterval);

            timeElapsed += _blinkInterval;
        }

        // Assure que le sprite est visible à la fin de l'effet
        _sprite.enabled = true;

        // Fin de l'invulnérabilité
        _health.SetInvulnerable(false);
    }

    public void Die()
    {
        _playerController.enabled = false;
        _playerAnimator.enabled = false;
    }
}
