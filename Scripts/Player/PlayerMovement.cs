using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ButtonInput { Down, Up }

public class PlayerMovement
{
    #region Variables & Builder

    private Queue<ButtonInput> _jumpInputBuffer = new();

    public float _xAxis;
    private float _yAxis;
    private float _coyoteTimeCounter;
    private float _jumpBufferCounter;
    private float _wallJumpingDirection;
    private float _wallJumpingCounter;
    private float _smoothSpeed;

    private bool _isWallSliding;
    private bool _isWallJumping;
    private bool _canReleaseKey;
    public bool _hasAlreadyFlipped;
    private bool _landed;
    private bool _spawnMiniCD = true;
    //private bool _firstJump;

    // Bear
    private bool _inAttackCD;
    private bool _bearAttacking;

    // Frog
    private bool _holdingFrogJump;
    private bool _doingSuperFrogJump;
    private float _heldTimeForSuperJump;

    // Sloth
    public bool spinning;

    // Requested
    private PlayerValues Data;
    private Transform transform;
    private Transform _groundCheck;
    private Rigidbody _RB;
    public bool _isFacingRight;
    private Transform _frontWallCheck;
    private Transform _backWallCheck;
    private MonoBehaviour _monoBehaviour;
    private Player _player;
    private Animator _animator;

    // Delegates
    private delegate void MyDelegate();
    private MyDelegate _Updating = delegate { };

    public PlayerMovement(PlayerValues data, Transform playersTransform, Transform groundCheck, Rigidbody RB,
                          bool startsFacingRight, Transform frontWallCheck, Transform backWallCheck, Player pl,
                          Animator animator)
    {
        Data = data;
        transform = playersTransform;
        _groundCheck = groundCheck;
        _RB = RB;
        _isFacingRight = startsFacingRight;
        _frontWallCheck = frontWallCheck;
        _backWallCheck = backWallCheck;
        _monoBehaviour = pl;
        _player = pl;
        _animator = animator;
    }

    #endregion

    #region Unity Methods

    public IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);
        _spawnMiniCD = false;
    }

    public void Update()
    {
        JumpMethods();
        WallMethods();
        CoyoteTime();
        FlipConstantly();
        FallingAndLanding();
        _Updating();
    }

    public void FixedUpdate()
    {
        LateralMovement();
    }

    #endregion

    #region Basic Movement

    private void LateralMovement()
    {
        if (_isWallJumping || _holdingFrogJump || _doingSuperFrogJump || _bearAttacking)
        {
            _animator.SetBool("Walking", false);
            return;
        }

        if (!_isWallSliding)
        {
            _smoothSpeed = Mathf.Clamp01(_smoothSpeed + (_xAxis != 0 ? Data.acceleration : -Data.deceleration) * Time.fixedDeltaTime) * Mathf.Abs(_xAxis);
            _RB.velocity = new Vector3(_smoothSpeed * (_isFacingRight ? 1 : -1) * Data.movementSpeed, _RB.velocity.y);
        }
        else
        {
            _smoothSpeed = 0f;
            _RB.velocity = new Vector3(_xAxis * Data.movementSpeed, _RB.velocity.y);
        }

        _animator.SetBool("Walking", _smoothSpeed != 0f);
    }

    private void FallingAndLanding()
    {
        bool falling = !IsGrounded() && !IsWalled();
        _animator.SetBool("Falling", falling);

        if (falling) _landed = false;

        if (!falling && !_landed)
        {
            if (!_spawnMiniCD) AudioManager.instance.Play(_player.SFX.Land);
            _landed = true;
        }
    }

    #endregion

    #region Jump

    private void JumpMethods()
    {
        JumpCommands();
        JumpBuffer();
        JumpMovement();
    }

    public void JumpCommands()
    {
        if (Input.GetButtonDown("Jump"))
            ExecuteJumpBuffer(ButtonInput.Down);

        else if (Input.GetButtonUp("Jump"))
            ExecuteJumpBuffer(ButtonInput.Up);
    }

    public void ExecuteJumpBuffer(ButtonInput item)
    {
        _jumpInputBuffer.Enqueue(item);
        if (_jumpInputBuffer.Count == 1) _jumpBufferCounter = Data.jumpBufferTime;
    }

    private void JumpBuffer()
    {
        if (_jumpInputBuffer.Count == 0) return;

        _jumpBufferCounter -= Time.deltaTime;

        if (_jumpBufferCounter <= 0)
        {
            _jumpInputBuffer.Dequeue();

            if (_jumpInputBuffer.Count > 0)
                _jumpBufferCounter = Data.jumpBufferTime;
        }
    }

    private void JumpMovement()
    {
        // Frog form can't realize normal jumps.
        if (_player.CurrentForm == FormName.Frog || _jumpInputBuffer.Count == 0) return;

        if (_jumpInputBuffer.Peek() == ButtonInput.Down && _coyoteTimeCounter > 0f)
        {
            /*if (_player.IsVoyageAvailable && !_firstJump)
            {
                _firstJump = true;
                _player.VoyageOn();
            }*/

            _RB.velocity = new Vector3(_RB.velocity.x, Data.jumpForce);
            _jumpInputBuffer.Dequeue();
            _canReleaseKey = true;
            AudioManager.instance.Play(_player.SFX.Jump);
            _animator.SetTrigger("Jump");
            _monoBehaviour.SpawnParticle(_player.Particle.DownJumpDust);
        }

        else if (_jumpInputBuffer.Peek() == ButtonInput.Up && _RB.velocity.y > 0f && _canReleaseKey)
        {
            _RB.velocity = new Vector3(_RB.velocity.x, _RB.velocity.y * 0.5f);
            _coyoteTimeCounter = 0f;
            _jumpInputBuffer.Dequeue();
            _canReleaseKey = false;
            _animator.ResetTrigger("Jump");
        }
    }

    #endregion

    #region Wall Slide & Jump

    private void WallMethods()
    {
        WallSlide();
        WallJump();
    }

    private void WallSlide()
    {
        if (IsWalled() && !IsGrounded() && !_bearAttacking)
        {
            if (_player.CurrentForm == FormName.Sloth)
                SlothSlide();
            else
            {
                _RB.velocity = new(_RB.velocity.x, Mathf.Clamp(_RB.velocity.y, -Data.wallSlidingSpeed, float.MaxValue));
                AudioManager.instance.PlayTillEnds(_player.SFX.WallSlide);
            }

            _monoBehaviour.StopDelayedMethod(() => _isWallJumping = false);
            _isWallSliding = true;
            _isWallJumping = false;
            _wallJumpingCounter = Data.wallJumpingTime;
            _animator.SetBool("Sliding", true);
            _animator.ResetTrigger("Jump");

            if (!_hasAlreadyFlipped)
            {
                if (IsFrontWalled())
                {
                    _wallJumpingDirection = _isFacingRight ? -1f : 1f;
                    Flip();
                }
                else
                {
                    _wallJumpingDirection = _isFacingRight ? 1f : -1f;
                }

                _hasAlreadyFlipped = true;
            }
        }
        else
        {
            if (_player.CurrentForm == FormName.Sloth) ResetSlothVariables();
            _isWallSliding = false;
            _hasAlreadyFlipped = false;
            _wallJumpingCounter -= Time.deltaTime;
            _animator.SetBool("Sliding", false);
            AudioManager.instance.Stop(_player.SFX.WallSlide);
        }
    }



    private void WallJump()
    {
        if (_jumpInputBuffer.Count > 0 && _jumpInputBuffer.Peek() == ButtonInput.Down && _wallJumpingCounter > 0f)
        {
            _isWallJumping = true;
            _wallJumpingCounter = 0f;
            _RB.velocity = new Vector3(_wallJumpingDirection * Data.wallJumpingPower.x, Data.wallJumpingPower.y);
            _monoBehaviour.StartDelayedMethod(() => _isWallJumping = false, Data.wallJumpingDuration);
            _animator.SetTrigger("Jump");
            AudioManager.instance.Play(_player.SFX.Jump);

            ParticleSystem dust = null;

            if (_isFacingRight) dust = _player.Particle.LeftJumpDust;
            else                dust = _player.Particle.RightJumpDust;

            _monoBehaviour.SpawnParticle(dust);
        }
        else
        {
            if (_isWallJumping) _animator.ResetTrigger("Jump");
        }
    }

    #endregion

    #region Flip

    private void FlipConstantly()
    {
        if (_isWallJumping || _isWallSliding || _holdingFrogJump || _doingSuperFrogJump || _bearAttacking) return;

        if (_isFacingRight && _xAxis < 0f || !_isFacingRight && _xAxis > 0f)
        {
            Flip();
        }
    }

    public void Flip()
    {
        _isFacingRight = !_isFacingRight;
        Quaternion localRotation = transform.localRotation;
        localRotation.y = _isFacingRight ? 0f : 180f;
        transform.localRotation = localRotation;
        _animator.SetBool("Flipped", !_isFacingRight);
    }

    public void ShouldFlipRight()
    {
        if (!_isFacingRight)
        {
            Flip();
        }
    }

    #endregion

    #region Forms Behaviour

    #region Bear

    public void BearAttack(float xPower, float yPower, float actionTime, float cooldown, Collider collider)
    {
        if (!_inAttackCD)
        {
            _animator.SetTrigger("Bear Attack");
            _bearAttacking = true;
            _inAttackCD = true;
            collider.enabled = true;

            _player.StartCoroutine(Attacking());
        }

        IEnumerator Attacking()
        {
            yield return new WaitForSeconds(0.02f);
            _player.Particle.PlayerTrail.Stop();
            _player.Particle.BearDash.Play();
            AudioManager.instance.Play("Bear Attack");
            _RB.velocity = new Vector3((_isFacingRight ? 1 : -1) * xPower, yPower);
            yield return new WaitForSeconds(actionTime);

            _player.Particle.PlayerTrail.Play();
            _player.Particle.BearDash.Stop();
            _bearAttacking = false;
            collider.enabled = false;
            yield return new WaitForSeconds(cooldown);

            _inAttackCD = false;

            /*if (IsGrounded() || IsWalled())
            {
                _player.StartDelayedMethod(() => _inAttackCD = false, cooldown);
            }
            else
            {
                _player.StartCoroutine(StopCooldown());
            }

            IEnumerator StopCooldown()
            {
                yield return new WaitForSeconds(cooldown);
                while (!IsGrounded() && !IsWalled())
                {
                    Debug.Log("a");
                    yield return null;
                }
                
                _inAttackCD = false;
            }*/
        }
    }

    public void ResetBearVariables()
    {
        _player.Particle.PlayerTrail.Play();
        _player.Particle.BearDash.Stop();
        _bearAttacking = false;
        _inAttackCD = false;
    }

    #endregion

    #region Frog

    public void SuperFrogJump(Vector2 jumpForce, bool pressingButton, float timeInAir, float timeToHold, float switchCD)
    {
        if (_doingSuperFrogJump)
        {
            if (!IsWalled() && !IsGrounded()) return;
            _monoBehaviour.StopDelayedMethod(StopSuperJump);
            _monoBehaviour.StopDelayedMethod(StopSwitchDelay);
            StopSuperJump();
            StopSwitchDelay();
        }

        bool condition = _heldTimeForSuperJump >= timeToHold && IsGrounded(); //&& Input.GetButtonUp(keyCode);

        if (condition)
        {
            SuperJump(jumpForce.x, jumpForce.y);        
        }
        else
        {
            _heldTimeForSuperJump = pressingButton && !IsWalled() ? _heldTimeForSuperJump + Time.fixedDeltaTime : 0f;
            _holdingFrogJump = _heldTimeForSuperJump > 0f ? true : false;

            bool holding = pressingButton && !IsWalled() && !_doingSuperFrogJump;
            _player.Animator.SetBool("Holding Super Jump", holding);

            var charge = _player.Particle.FrogCharge;
            if (holding && !charge.isPlaying) charge.Play();
            else if (!holding || _doingSuperFrogJump) charge.Stop();

            if (_holdingFrogJump && IsGrounded())
            {
                _RB.velocity = Vector3.zero;
                AudioManager.instance.PlayTillEnds(_player.SFX.FrogCharges);
            }
            else
            {
                AudioManager.instance.Stop(_player.SFX.FrogCharges);
            }
        }

        void SuperJump(float x, float y)
        {
            _player.Particle.PlayerTrail.Stop();
            _player.Particle.FrogJump.Play();
            AudioManager.instance.Play(_player.SFX.FrogJump);
            _player.Animator.SetBool("Holding Super Jump", false);
            _player.Animator.SetBool("Doing Super Jump", true);

            _player.FrogCanSwitch = false;
            _RB.velocity = new Vector3((_isFacingRight ? 1 : -1) * x, y);
            _heldTimeForSuperJump = 0f;
            _holdingFrogJump = false;
            _doingSuperFrogJump = true;
            _monoBehaviour.StartDelayedMethod(StopSuperJump, timeInAir, false);
            _monoBehaviour.StartDelayedMethod(StopSwitchDelay, switchCD, false);
        }

        void StopSuperJump()
        {
            _player.Particle.PlayerTrail.Play();
            _player.Particle.FrogJump.Stop();
            _player.Animator.SetBool("Doing Super Jump", false);

            _doingSuperFrogJump = false;
        }

        void StopSwitchDelay()
        {
            _player.FrogCanSwitch = true;
        }
    }

    public void ResetFrogVariables()
    {
        _player.Particle.PlayerTrail.Play();
        _player.Particle.FrogCharge.Stop();
        _player.Particle.FrogJump.Stop();
        _player.Animator.SetBool("Holding Super Jump", false);

        _player.FrogCanSwitch = true;
        _holdingFrogJump = false;
        _doingSuperFrogJump = false;
        _heldTimeForSuperJump = 0f;
    }

    #endregion

    #region Sloth

    private void SlothSlide()
    {
        float verticalInput = _yAxis;

        if (verticalInput == 0)
        {
            IsSlothClimbing(false);
            _RB.velocity = Vector3.zero;
        }
        else
        {
            IsSlothClimbing(true);
            float slideSpeed = Data.slothClimbing * verticalInput;
            _RB.velocity = new(_RB.velocity.x, Mathf.Clamp(_RB.velocity.y, slideSpeed, float.MaxValue));
        }

        void IsSlothClimbing(bool value)
        {
            _RB.useGravity = value;
            _player.Animator.SetBool("Climbing", value);
            if (value) AudioManager.instance.PlayTillEnds(_player.SFX.SlothClimbing);
            else
            {
                AudioManager.instance.Stop(_player.SFX.SlothClimbing);
                AudioManager.instance.Stop(_player.SFX.WallSlide);
            }
        }
    }
    
    public void SlothSpin()
    {
        if (spinning) return;

        Spinner vine = ObtainVine();
        if (vine == null) return;

        _hasAlreadyFlipped = false;
        vine.Spin(_player);
        
        //_wallJumpingCounter = Data.wallJumpingTime;

        //bool isLeft = MyUtility.IsLeft(transform.position, vine.transform.position);
        //transform.position = new Vector3(isLeft ? vine.RightWaypoint.x : vine.LeftWaypoint.x, transform.position.y, transform.position.z);

        Spinner ObtainVine()
        {
            if (!IsVined()) return null;

            Collider[] frontColliders = Obtain(_frontWallCheck.position);
            Collider[] backColliders = Obtain(_backWallCheck.position);

            Collider[] Obtain(Vector3 wallCheck) => Physics.OverlapSphere(wallCheck, Data.maxWallDistance, Data.vineLayer);

            // other option could be using raycast in the middle

            return frontColliders.Length > 0 ? frontColliders[0].GetComponent<Spinner>() :
                   backColliders.Length > 0 ? backColliders[0].GetComponent<Spinner>() : null;
        }
    }

    public void ResetSlothVariables()
    {
        _RB.useGravity = true;
        _player.Animator.SetBool("Climbing", false);
    }

    #endregion

    #endregion

    #region Always Checked

    private void PCAxes()
    {
        _xAxis = Input.GetAxisRaw("Horizontal");
        _yAxis = Input.GetAxisRaw("Vertical");
    }

    private void MobileAxes()
    {
        _xAxis = JoyController.instance.GetMovementInput().x;
        _yAxis = JoyController.instance.GetMovementInput().z;
    }

    private void CoyoteTime() => _coyoteTimeCounter = IsGrounded() ? Data.coyoteTime : _coyoteTimeCounter - Time.deltaTime;

    #endregion

    #region Utility Checks

    private bool IsGrounded() => Physics.CheckSphere(_groundCheck.position, Data.maxGroundDistance, Data.groundLayer);

    private bool IsWalled() => IsFrontWalled() || IsBackWalled();

    private bool IsVined() => IsFrontVined() || IsBackVined();


    private bool IsFrontWalled() => Physics.CheckSphere(_frontWallCheck.position, Data.maxWallDistance, Data.wallLayer);

    private bool IsBackWalled() => Physics.CheckSphere(_backWallCheck.position, Data.maxWallDistance, Data.wallLayer);


    private bool IsFrontVined() => Physics.CheckSphere(_frontWallCheck.position, Data.maxWallDistance, Data.vineLayer);

    private bool IsBackVined() => Physics.CheckSphere(_backWallCheck.position, Data.maxWallDistance, Data.vineLayer);

    #endregion

    #region System Behaviours

    public void SetPCParameters()
    {
        _Updating += PCAxes;
    }

    public void SetMobileParameters()
    {
        _Updating += MobileAxes;
    }

    #endregion
}
