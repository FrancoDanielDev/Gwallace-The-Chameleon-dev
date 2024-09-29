using UnityEngine;

public class Frog : Form
{
	[SerializeField] private float _timeToHold = 0.35f;
	[SerializeField] private float _switchCD = 0.8f;
	[SerializeField] private float _timeInAir = 1f;
	[SerializeField] private Vector2 _jumpForce = new Vector2(10f, 25f);
	[Space]
	[SerializeField] private Player _player;

	private delegate void MyDelegate();
	private MyDelegate _FixedUpdating = delegate { };

	private bool _pressingButton = false;

	private void FixedUpdate()
    {
		_FixedUpdating();		
	}

	private void EvaluteActionPC()
	{
		_FixedUpdating = () => FrogJump(MyInput.instance.GetKey(ActionKeybind.ActionKeybind));
	}

	private void EvaluteActionMobile()
	{
		_FixedUpdating = () => FrogJump(_pressingButton);
	}

	public void ShouldFrogJump(bool answer)
    {
		_pressingButton = answer;
    }

	private void FrogJump(bool pressingButton)
    {
		_player.Movement.SuperFrogJump(_jumpForce, pressingButton, _timeInAir, _timeToHold, _switchCD);
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		if (!Application.isMobilePlatform) _FixedUpdating = EvaluteActionPC;
		else _FixedUpdating = EvaluteActionMobile;
	}
}
