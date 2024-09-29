using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingLine : MonoBehaviour
{
    [Tooltip("It should be 99 for Ultimate Voyage")]
    [SerializeField] private int _levelID;
    [Space]
    [SerializeField] private GameObject _entireObject;
    [SerializeField] private MeshRenderer _triggerRenderer;
    [SerializeField] private GameObject _stopwatchModel;
    [SerializeField] private ParticleSystem _celebrationParticle;
    [SerializeField] private string _celebrationAudio, _celebrationAudio2;
    [SerializeField] private GameEvent _voyageOnEvent;

    private string _chosenLevelKey = "Chosen Level";

    private void Start()
    {      
        int chosenLevel = GameDataManager.instance.GetInt(_chosenLevelKey);

        if (chosenLevel != _levelID) _entireObject.SetActive(false);
        else _triggerRenderer.enabled = false;
    }

    public void StartSpeedrun()
    {
        GameManager.instance.MakeStopwatchesWork();

        _celebrationParticle.Play();
        AudioManager.instance.Play(_celebrationAudio);
        AudioManager.instance.Play(_celebrationAudio2);

        gameObject.SetActive(false);
        _stopwatchModel.SetActive(false);
        _voyageOnEvent.Raise();
    }
}
