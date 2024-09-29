using System.Collections;
using UnityEngine;

public class FallingPin : Resetable
{
    [SerializeField] private float _fallSpeed = 10f;
    [SerializeField] private float _fallDelay = 0.5f;
    [SerializeField] private float _maxGroundDistance = 1f;
    [Space]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Animator _animator;
    [SerializeField] private Collider _triggerCollider;
    [SerializeField] private ParticleSystem _particle;
    [Space]
    [SerializeField] private AudioSource _breakingAudio;
    [SerializeField] private string _fallAudio;

    private delegate void MyDelegate();
    private MyDelegate _myDelegate = delegate { };

    private Vector3 _initialPos;
    private bool _bearForm;
    private bool _falling;

    private void Start()
    {
        _initialPos = transform.position;
        EventManager.instance.Subscribe(Shake, Events.StartPinShaking);
        EventManager.instance.Subscribe(Unshake, Events.StopPinShaking);
    }

    private void FixedUpdate()
    {
        _myDelegate();
    }

    public void MakeItFall()
    {
        StartCoroutine(Fall());
    }

    private IEnumerator Fall()
    {
        if (_bearForm)
        {
            _particle.Play();
        }
        else
        {
            _breakingAudio.Play();
            _animator.SetBool("Shaking", true);
            yield return new WaitForSeconds(_fallDelay);
            _breakingAudio.Stop();
        }

        SubscribeToReset();
        _animator.SetBool("Shaking", false);
        _collider.enabled = false;
        _rb.isKinematic = false;
        _triggerCollider.enabled = false;
        _falling = true;
        AudioManager.instance.Play(_fallAudio);
        _myDelegate = Falling;

        void Falling()
        {
            _rb.AddForce(Vector3.down * _fallSpeed, ForceMode.Force);
            if (Physics.Raycast(transform.position, Vector3.down, _maxGroundDistance, _groundLayer)) Disappear();
        }

        void Disappear()
        {
            _model.SetActive(false);
            _myDelegate = delegate { };
        }
    }

    #region Subscriptions

    public void Shake()
    {
        if (!_falling)
        {
            _breakingAudio.Play();
            _animator.SetBool("Shaking", true);
        }

        _bearForm = true;
    }

    public void Unshake()
    {
        if (_breakingAudio != null) _breakingAudio.Stop();
        if (_animator != null) _animator.SetBool("Shaking", false);
        _bearForm = false;
    }

    protected override void DoReset()
    {
        base.DoReset();
        CancelInvoke(nameof(Fall));
        _myDelegate = delegate { };
        _rb.isKinematic = true;
        _triggerCollider.enabled = true;
        _falling = false;
        transform.position = _initialPos;
        _animator.SetBool("Shaking", false);
        _breakingAudio.Stop();
    }

    #endregion
}
