using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class VoyageMode : MonoBehaviour, IPausable
{
    [ReadOnly, SerializeField] private int _myRecordMilliseconds;
    [Space]
    [SerializeField] private string _miniVoyageDataName = "Mini Voyage Mode";
    [SerializeField] private string _voyageDataName = "Voyage Mode";
    [Space]
    [SerializeField] private GameEvent _miniVoyageAvailable;
    [SerializeField] private GameEvent _voyageAvailable;
    [SerializeField] private GameObject _gameTime;
    [SerializeField] private Button _selectedButton;
    [SerializeField] private TextMeshProUGUI _minutesAndSeconds;
    [SerializeField] private TextMeshProUGUI _milliseconds;
    [SerializeField] private GameObject _endingMenu;
    [SerializeField] private TextMeshProUGUI _minutesAndSecondsText;
    [SerializeField] private TextMeshProUGUI _millisecondsText;

    private float _startTime;

    private delegate void MyDelegate();
    private MyDelegate _Updating = delegate { };

    private void Start()
    {
        EvaluateVoyage();
    }

    private void Update()
    {
        _Updating();
    }

    public int GetTime() => _myRecordMilliseconds;

    public void StartVoyage()
    {
        ResetTimer();
        _startTime = Time.time;
        _Updating = UpdateTimeDisplay;
    }

    public void EndVoyage()
    {
        StopVoyage();
        StartCoroutine(OpenEndingMenu());

        IEnumerator OpenEndingMenu()
        {
            yield return new WaitForSeconds(1f);

            _endingMenu.SetActive(true);
            _selectedButton.Select();

            int minutes = _myRecordMilliseconds / (60 * 1000);
            int seconds = (_myRecordMilliseconds % (60 * 1000)) / 1000;
            int milliseconds = _myRecordMilliseconds % 1000;

            _minutesAndSecondsText.text = string.Format("{0:00} : {1:00}", minutes, seconds);
            _millisecondsText.text = string.Format(".{0:000}", milliseconds);
        }
    }

    private void EvaluateVoyage()
    {
        var gdm = GameDataManager.instance;

        if (gdm.GetInt(_voyageDataName) == 0)
        {
            _gameTime.SetActive(false);
        }
        else
        {
            _voyageAvailable.Raise();
            _gameTime.SetActive(true);
            ResetTimer();
        }

        if (gdm.GetInt(_miniVoyageDataName) == 1)
        {
            _miniVoyageAvailable.Raise();
        }
    }

    private void UpdateTimeDisplay()
    {
        float totalElapsedTime = Time.time - _startTime;
        _myRecordMilliseconds = Mathf.FloorToInt(totalElapsedTime * 1000);

        int minutes = (int)(totalElapsedTime / 60);
        int seconds = (int)(totalElapsedTime % 60);
        int milliseconds = (int)(totalElapsedTime * 1000 % 1000);

        string minutesAndSecondsString = string.Format("{0:00} : {1:00}", minutes, seconds);
        string microSecondsString = string.Format(".{0:000}", milliseconds);

        _minutesAndSeconds.text = minutesAndSecondsString;
        _milliseconds.text = microSecondsString;
    }

    private void ResetTimer()
    {
        _minutesAndSeconds.text = "00 : 00";
        _milliseconds.text = ".000";
    }

    private void StopVoyage()
    {
        _Updating = delegate { };
    }

    private void ContinueVoyage()
    {
        _Updating = UpdateTimeDisplay;
    }

    #region Pause

    public void Subscriptions()
    {
        EventManager.instance.Subscribe(Pause, Events.Pause);
        EventManager.instance.Subscribe(Unpause, Events.Unpause);
    }

    public void Pause()
    {
        StopVoyage();
    }

    public void Unpause()
    {
        ContinueVoyage();
    }

    #endregion
}
