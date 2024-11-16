using System;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Animator _anim;

    [SerializeField] private Transform _verticalAttack;
    [SerializeField] private Transform _horizontalAttack;
    [SerializeField] private Transform _walkEffectPosition;
    [SerializeField] private SpriteRenderer _sprite;

    [Header("Particles")][SerializeField] private ParticleSystem _jumpParticles;
    [SerializeField] private ParticleSystem _launchParticles;
    [SerializeField] private ParticleSystem _moveParticles;
    [SerializeField] private GameObject _specialAttackEffect;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip _attackSound;
    [SerializeField] private AudioClip[] _footsteps;

    [SerializeField] private float _specialAttackSpeed = 10f;

    public event Action ParryEnded;

    private AudioSource _source;
    private IPlayerController _player;
    private PlayerManager _playerManager;

    private bool _grounded;
    private bool _attacked;
    private float _verticalOrientation;
    private AttackDirection _lastDirection;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
        _player = GetComponentInParent<IPlayerController>();
        _playerManager = GetComponentInParent<PlayerManager>();
    }

    private void OnEnable()
    {
        _player.Parry += OnParry;
        _player.Jumped += OnJumped;
        _player.Attacked += OnAttacked;
        _player.GroundedChanged += OnGroundedChanged;

        //_moveParticles.Play();
    }

    private void OnDisable()
    {
        _player.Parry -= OnParry;
        _player.Jumped -= OnJumped;
        _player.Attacked += OnAttacked;
        _player.GroundedChanged -= OnGroundedChanged;

        _anim.SetBool(WalkingKey, false);
        //_moveParticles.Stop();
    }

    private void Update()
    {
        if (_player == null) return;

        HandleSpriteFlip();

        HandleWalking();
    }

    private void HandleSpriteFlip()
    {
        if (_player.FrameInput.x == 0 || _attacked) return;

        _sprite.flipX = _player.FrameInput.x > 0f;
        _horizontalAttack.rotation = Quaternion.Euler(0, _player.FrameInput.x > 0 ? 180f : 0f, 0f);
        _verticalOrientation = _player.FrameInput.x > 0f ? 180f : 0f;
        _verticalAttack.localPosition = new Vector3(_player.FrameInput.x > 0f ? 0.5f : -0.5f, 0f, 0f);
        _walkEffectPosition.localPosition = new Vector3(_player.FrameInput.x > 0f ? 0.7f : -0.7f, -1.62f, 0f);
    }

    private void HandleWalking()
    {
        if (_player.FrameInput.x != 0 && _grounded)
        {
            _anim.SetBool(WalkingKey, true);
        }
        else
        {
            _anim.SetBool(WalkingKey, false);
        }
    }

    public void WalkEffectTrigger()
    {
        _source.PlayOneShot(_footsteps[UnityEngine.Random.Range(0, _footsteps.Length)], 0.5f);
        Instantiate(_moveParticles, _walkEffectPosition.position, Quaternion.identity);
    }

    private void OnJumped()
    {
        _anim.SetTrigger(JumpKey);
        _anim.ResetTrigger(GroundedKey);


        if (_grounded) // Avoid coyote
        {
            //_jumpParticles.Play();
        }
    }

    private void OnAttacked(AttackDirection attackDirection)
    {
        _attacked = true;

        _source.PlayOneShot(_attackSound, 0.5f);
        _lastDirection = attackDirection;

        switch (attackDirection)
        {
            case AttackDirection.Normal:
                _anim.SetTrigger(HorizontalAttackKey);
                break;
            case AttackDirection.Up:
                _verticalAttack.rotation = Quaternion.Euler(0, _verticalOrientation, -90);
                _anim.SetTrigger(VerticalAttackKey);
                break;
            case AttackDirection.Down:
                _verticalAttack.rotation = Quaternion.Euler(180, _verticalOrientation, -90);
                _anim.SetTrigger(VerticalAttackKey);
                break;
        }
    }

    public void LaunchSpecialAttack()
    {
        if (_playerManager.SpecialEnabled)
        {
            Vector2 velocity = Vector2.zero;
            Transform transform = _horizontalAttack.transform;

            switch (_lastDirection)
            {
                case AttackDirection.Normal:
                    velocity += _horizontalAttack.rotation.y == 1f ? Vector2.right : Vector2.left;
                    break;
                case AttackDirection.Up:
                    velocity += Vector2.up;
                    transform = _verticalAttack.transform;
                    break;
                case AttackDirection.Down:
                    velocity += Vector2.down;
                    transform = _verticalAttack.transform;
                    break;
            }

            GameObject go = Instantiate(_specialAttackEffect, transform.position, transform.rotation);
            go.GetComponent<Rigidbody2D>().velocity += velocity * _specialAttackSpeed;
            _playerManager.RemoveSpecialAttack();
        }
    }

    public void AttackAnimStop()
    {
        _attacked = false;
    }

    private void OnParry()
    {
        _anim.SetTrigger(ParryKey);
    }

    public void ParryAnimStop()
    {
        ParryEnded?.Invoke();
    }

    private void OnGroundedChanged(bool grounded, float impact)
    {
        _grounded = grounded;

        if (grounded)
        {
            _anim.SetTrigger(GroundedKey);
            _anim.ResetTrigger(JumpKey);

            _source.PlayOneShot(_footsteps[UnityEngine.Random.Range(0, _footsteps.Length)], 0.5f);

            Instantiate(_moveParticles, _walkEffectPosition.position, Quaternion.identity);
        }
        else
        {
            _anim.SetTrigger(JumpKey);
            _anim.ResetTrigger(GroundedKey);
        }
    }

    private static readonly int JumpKey = Animator.StringToHash("Jump");
    private static readonly int ParryKey = Animator.StringToHash("Parry");
    private static readonly int WalkingKey = Animator.StringToHash("Walking");
    private static readonly int GroundedKey = Animator.StringToHash("Grounded");
    private static readonly int VerticalAttackKey = Animator.StringToHash("VerticalAttack");
    private static readonly int HorizontalAttackKey = Animator.StringToHash("HorizontalAttack");
}
