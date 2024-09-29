using UnityEngine;

public class Bear : Form
{
	[Header("ATTACK VALUES")]
	[SerializeField] private float _duration = 0.3f;
	[SerializeField] private float _cooldown = 0.2f;
	[SerializeField] private Vector2 _force = new(15f, 0f);
	[Space]
	[SerializeField] private Collider _collider;
	[SerializeField] private Player _player;

	private delegate void MyDelegate();
	private MyDelegate _Updating = delegate { };

	private void Update()
	{
		_Updating();
	}

	private void EvaluateAction()
    {
		_Updating = Attack;

		void Attack()
        {
			if (MyInput.instance.GetKeyDown(ActionKeybind.ActionKeybind)) BearAttack();
		}
	}

	public void BearAttack()
    {
		_player.Movement.BearAttack(_force.x, _force.y, _duration, _cooldown, _collider);
	}

	private void OnTriggerEnter(Collider other)
    {
		IDamageable entity = other.GetComponent<IDamageable>();
		if (entity != null) entity.ReceivesHit();
	}

    protected override void OnEnable()
    {
        base.OnEnable();
		EventManager.instance.Trigger(Events.StartPinShaking);
		if (!Application.isMobilePlatform) EvaluateAction();
	}

    protected override void OnDisable()
    {
        base.OnDisable();
		EventManager.instance.Trigger(Events.StopPinShaking);
	}
}
