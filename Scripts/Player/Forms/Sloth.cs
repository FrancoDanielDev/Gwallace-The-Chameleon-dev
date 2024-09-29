using UnityEngine;

public class Sloth : Form
{
	[Space]
	[SerializeField] private Player _player;

	private delegate void MyDelegate();
	private MyDelegate _Updating = delegate { };

	private void Update()
	{
		_Updating();
	}

	private void EvaluteAction()
	{
		_Updating = Spin;

		void Spin()
		{
			if (MyInput.instance.GetKeyDown(ActionKeybind.ActionKeybind)) SlothSpin();
		}
	}

	public void SlothSpin()
	{
		_player.Movement.SlothSpin();
	}

	protected override void OnEnable()
    {
        base.OnEnable();
		if (!Application.isMobilePlatform) _Updating = EvaluteAction;
    }
}
