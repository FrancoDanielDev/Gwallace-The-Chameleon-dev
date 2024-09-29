using UnityEngine;

public class PlayerFormSwitch
{
    #region Variables & Builder

    private FormName[] _availableForms;
	private int _currentIndex;
	private int _noneIndex;
	private bool _canSwitch = true;
	private FormName _lastForm;

	private FormModel[] _formModels;
	private Player _player;
	private float _switchCooldown;

	public PlayerFormSwitch(FormModel[] formModels, Player player, float switchCD)
	{
		_formModels = formModels;
		_player = player;
		_switchCooldown = switchCD;
	}

    #endregion

    #region Start and Update

    public void Start()
	{
		InitializeForms();
	}

	public void Update()
	{
		//if (Input.GetButtonDown("Change Form")) TryToSwitch();
		if (MyInput.instance.GetKeyDown(ActionKeybind.SwitchKeybind)) TryToSwitch();
	}

	public void InitializeForms()
	{
		// Available Forms

		_lastForm = _player.CurrentForm;
		FormName[] starterForms = LevelManager.instance.CurrentForms();
		_availableForms = new FormName[starterForms.Length];

		for (int i = 0; i < _availableForms.Length; i++)
		{
			_availableForms[i] = starterForms[i];
		}

		// Current Form

		_player.CurrentForm = starterForms[0];
		SwitchForm(true);
		_player.Particle.FormChange.Stop();
		SlotManager.instance.SetStartingParameters();

		_currentIndex = 0;
		SlotManager.instance.SelectSlot(_currentIndex);
	}

    #endregion

    #region Switch

    public void TryToSwitch()
	{
		if (!_canSwitch) return;

		if (IfMissingForm())
        {
			MyDebugs.Log("You only have 1 form available.", Color.gray);
			return;
        }
		else if (!_canSwitch)
        {
			MyDebugs.Log("You are in switch cooldown.", Color.magenta);
			return;
		}

		Animator a = _player.Animator;
		switch (_player.CurrentForm)
		{
			case FormName.Bear:
				DontSwitch(a.GetCurrentAnimatorStateInfo(0).IsName("Attack") &&
						   a.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.38f); break;

			case FormName.Frog:
				DontSwitch(!_player.FrogCanSwitch); break;
				//DontSwitch(a.GetBool("Doing Super Jump")); break;

			case FormName.Sloth:
				DontSwitch(_player.Movement.spinning); break;

			default: DontSwitch(false); break;
		}

		void DontSwitch(bool value)
		{
			if (!value)
			{
				SwitchToNextForm();
				_canSwitch = false;
				MyCoroutines.StartDelayedMethod(_player, () => _canSwitch = true, _switchCooldown);
			}
            else
            {
				MyDebugs.Log("You can't switch while performing this action.", Color.cyan);
            }
		}
	}

	private void SwitchToNextForm()
	{
		_lastForm = _player.CurrentForm;
		_currentIndex = (_currentIndex + 1) % _availableForms.Length;
		_player.CurrentForm = _availableForms[_currentIndex];
		SlotManager.instance.SelectSlot(_currentIndex);
		AudioManager.instance.Play(_player.SFX.ChangeForm);
		SwitchForm();
	}

	private void SwitchForm(bool initial = false)
	{
		for (int i = 0; i < _formModels.Length; i++)
		{
			_formModels[i].model.SetActive(_player.CurrentForm == _formModels[i].name);

			if (_player.CurrentForm == _formModels[i].name)
				_player.Animator.runtimeAnimatorController = _formModels[i].animController;
		}

		FormAlterations();
		_player.Particle.FormChange.Play();
		if (!initial) _player.StartDelayedMethod(() => _player.Animator.SetTrigger("Change Form"), 0.01f);
	}

	#endregion

    #region Bubble Functions

    public void BubblePerformance(Bubble bubble)
	{
		// If first time obtaining a form

		if (IfMissingForm())
		{
			AddNewForm(bubble.FormContainer);
			bubble.Pop();
		}
		else
		{
			FormName container = _player.CurrentForm;
			ReplaceForm(bubble.FormContainer);
			bubble.ChangeContainer(container);
		}
	}

	private void AddNewForm(FormName newForm)
	{
		_availableForms[_noneIndex] = newForm;
		SlotManager.instance.SetSlot(newForm, _noneIndex);

		SwitchToNextForm();
	}

	private void ReplaceForm(FormName newForm)
	{
		_lastForm = _player.CurrentForm;
		_player.CurrentForm = newForm;
		_availableForms[_currentIndex] = newForm;
		SlotManager.instance.SetSlot(newForm, _currentIndex);

		SwitchForm();
	}

	#endregion

	#region Utility

	private bool IfMissingForm()
	{
		for (int i = 0; i < _availableForms.Length; i++)
		{
			if (_availableForms[i] == FormName.None)
			{
				_noneIndex = i;
				return true;
			}
		}

		return false;
	}

    #endregion

    #region Form Switch Alterations

    private void FormAlterations()
	{
		var x = _player.Movement;

        switch (_lastForm)
        {
			case FormName.Bear:  x.ResetBearVariables();  break;
			case FormName.Frog:  x.ResetFrogVariables();  break;
            case FormName.Sloth: x.ResetSlothVariables(); break;
        }
	}

    #endregion
}