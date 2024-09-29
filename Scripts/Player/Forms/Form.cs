using UnityEngine;

public abstract class Form : MonoBehaviour
{
	[Space]
	[SerializeField] protected GameObject[] _outfit;

	protected virtual void OnEnable()
	{
		TurnOutfit(true);
	}

	protected virtual void OnDisable()
	{
		TurnOutfit(false);
	}

	protected virtual void TurnOutfit(bool on)
    {
		for (int i = 0; i < _outfit.Length; i++)
		{
			_outfit[i].SetActive(on);
		}
	}
}
