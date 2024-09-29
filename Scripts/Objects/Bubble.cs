using UnityEngine;

public class Bubble : Resetable
{
	public FormName FormContainer { get { return _formContainer; } }

	[SerializeField] private FormName _formContainer;
	[Space]
	[SerializeField] private FormModel[] _formModels;
	[SerializeField] private ParticleSystem _popParticle;
	[SerializeField] private string _popsAudio;
	[SerializeField] private string _changesAudio;

	// Reset
	private FormName _initialForm;

    private void Start()
    {
		SetContainer(_formContainer);
		_initialForm = _formContainer;
	}

    public void ChangeContainer(FormName newContainer)
	{
		SubscribeToReset();
		AudioManager.instance.Play(_changesAudio);
		SetContainer(newContainer);
	}

	public void Pop()
    {
		SubscribeToReset();
		AudioManager.instance.Play(_popsAudio);
		_popParticle.Play();
		Turn(false);
    }

    protected override void DoReset()
    {
		Turn(true);
		ChangeModel(_initialForm);
		_formContainer = _initialForm;
	}

	private void SetContainer(FormName newContainer)
    {
		_formContainer = newContainer;
		ChangeModel(_formContainer);
	}

    private void ChangeModel(FormName container)
	{
		foreach (var form in _formModels)
		{
			form.model.SetActive(container == form.name);
		}
	}
}
