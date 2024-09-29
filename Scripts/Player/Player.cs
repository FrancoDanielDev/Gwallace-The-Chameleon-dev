using UnityEngine;
using System;

public enum FormName{ None, Chameleon, Bear, Frog, Sloth, Owl }

[Serializable] public struct FormModel
{
    public FormName name;
    public GameObject model;
    public RuntimeAnimatorController animController;
}

public class Player : MonoBehaviour, IPausable
{
    #region Variables

    [Serializable]
    public class Particles
    {
        public ParticleSystem FormChange;
        public ParticleSystem JumpBoing;
        public ParticleSystem DownJumpDust;
        public ParticleSystem LeftJumpDust;
        public ParticleSystem RightJumpDust;
        public ParticleSystem BearAttack;
        public ParticleSystem MagicBoom;
        public ParticleSystem SoftBoom;
        public ParticleSystem Death;
        public ParticleSystem LeavesExplosion;
        public ParticleSystem LeavesInvocation;
        public ParticleSystem PlayerTrail;
        public ParticleSystem NormalTrail;
        public ParticleSystem UltimateTrail;
        public ParticleSystem BearDash;
        public ParticleSystem FrogJump;
        public ParticleSystem FrogCharge;
        public ParticleSystem LeavesTrail;
        public ParticleSystem Confetti;
    }

    [Serializable]
    public class Sfx
    {
        public string ChangeForm;
        public string Jump;
        public string BearAttack;
        public string Death;
        public string FrogJump;
        public string FrogCharges;
        public string WallSlide;
        public string Land;
        public string LevelTransition;
        public string SlothClimbing;
        public string LeavesInvocation;
        public string Confetti;
    }

    public FormName CurrentForm { get; set; }

    public bool Immortal { get; set; }

    public bool FrogCanSwitch { get; set; } = true;

    public bool IsVoyageAvailable { get; private set; }

    public Transform CameraFollow { get { return _cameraFollow; } }

    public Animator Animator { get { return _animator; } }

    public Particles Particle { get { return _particles; } }

    public Sfx SFX { get { return _sfx; } }

    [Space]
    [SerializeField] private bool _startsFacingRight = true;
    [Space]
    [SerializeField] private FormModel[] _formModels;
    [SerializeField] private Particles _particles;
    [SerializeField] private Sfx _sfx;
    [Space]
    [Header("GENERAL")]
    [SerializeField] private GameEvent _voyageIsAvailable;
    [SerializeField] private GameEvent _voyageOn;
    [SerializeField] private GameEvent _voyageOff;
    [SerializeField] private PlayerValues _playerValues;
    [SerializeField] private Rigidbody _RB;
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _cameraFollow;
    [SerializeField] private GameObject _model;
    [SerializeField] private GameObject _xmasHat;
    [Space]
    [Header("CHECKS")]
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private Transform _frontWallCheck;
    [SerializeField] private Transform _backWallCheck;

    // Classes
    public PlayerMovement Movement;
    public PlayerFormSwitch FormSwitch;
    public PlayerInteractions Interactions;
    public PlayerTechs PlayerTechs;

    // Delegates
    private delegate void MyDelegate();
    private MyDelegate _Updating = delegate { };
    private MyDelegate _FixedUpdating = delegate { };

    private RigidbodyConstraints _myConstraints;

    #endregion

    #region Awake, Start, Update++

    private void Awake()
    {
        AssignClasses();
    }

    private void Start()
    {
        _myConstraints = _RB.constraints;
        StartCoroutine(Movement.Start());
        FormSwitch.Start();
        PlayerTechs.Start();
        TurnOnDelegates(true);
        Subscriptions();
    }

    private void Update()
    {
        _Updating();
    }

    private void FixedUpdate()
    {
        _FixedUpdating();
    }

    private void OnTriggerEnter(Collider other)
    {
        Interactions.OnTriggerEnter(other);
    }

    private void OnTriggerExit(Collider other)
    {
        Interactions.OnTriggerExit(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Interactions.OnCollisionEnter(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        Interactions.OnCollisionExit(collision);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_groundCheck.position, _playerValues.maxGroundDistance);
    }

    #endregion

    #region Class assignment

    private void AssignClasses()
    {
        Movement = new(_playerValues, transform, _groundCheck, _RB, _startsFacingRight,
                       _frontWallCheck, _backWallCheck, this, _animator);

        FormSwitch   = new(_formModels, this, _playerValues.formSwitchCooldown);
        PlayerTechs  = new(this, _model, _animator);
        Interactions = new(this);
    }

    #endregion

    #region Delegates

    private void TurnOnDelegates(bool activate)
    {
        if (activate)
        {
            _Updating = Updatable;
            _FixedUpdating = FixedUpdatable;
        }
        else
        {
            _Updating = delegate { };
            _FixedUpdating = delegate { };
        }
    }

    private void Updatable()
    {
        Movement.Update();
        FormSwitch.Update();

        //PlayerTechs.ResetScene();
        PlayerTechs.ResetLevel();
        //PlayerTechs.TPCheating();
    }

    private void FixedUpdatable()
    {
        Movement.FixedUpdate();
    }

    #endregion

    #region Freezes

    public void Freeze()
    {
        _RB.constraints = RigidbodyConstraints.FreezeAll;
        TurnOnDelegates(false);
    }

    public void Unfreeze()
    {
        _RB.constraints = _myConstraints;
        TurnOnDelegates(true);
    }

    #endregion

    #region Events

    #region System

    public void SetPCParameters()
    {
        Movement.SetPCParameters();
    }

    public void SetMobileParameters()
    {
        Movement.SetMobileParameters();
    }

    #endregion

    #region Mobile Buttons

    public void SwitchButtonPressed()
    {
        FormSwitch.TryToSwitch();
    }

    public void JumpButtonPressed()
    {
        Movement.ExecuteJumpBuffer(ButtonInput.Down);
    }

    public void JumpButtonReleased()
    {
        Movement.ExecuteJumpBuffer(ButtonInput.Up);
    }

    #endregion

    #region Voyage Mode

    public void VoyageAvailable()
    {
        IsVoyageAvailable = true;

        _particles.NormalTrail.gameObject.SetActive(false);
        _particles.UltimateTrail.gameObject.SetActive(true);
    }

    public void VoyageOn()
    {
        _voyageOn.Raise();
    }

    public void VoyageOff()
    {
        _voyageOff.Raise();
    }

    #endregion

    #region Miscellaneous

    public void XmasHatOn()
    {
        _xmasHat.SetActive(true);
    }

    #endregion

    #endregion

    #region Pause

    public void Subscriptions()
    {
        EventManager.instance.Subscribe(Pause, Events.Pause);
        EventManager.instance.Subscribe(Unpause, Events.Unpause);
    }

    public void Pause()
    {
        TurnOnDelegates(false);
    }

    public void Unpause()
    {
        TurnOnDelegates(true);
    }

    #endregion
}
