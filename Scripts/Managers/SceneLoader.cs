using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private float _minCooldown = 50;
    [Space]
    [SerializeField] private string _transitionAudio;
    [SerializeField] private GameObject _blackScreen;
    [SerializeField] private GameObject _loadingScreenCanvas;
    [SerializeField] private GameObject _fadeIn;
    [SerializeField] private GameObject _fadeOut;

    private float _fadeTime = 0.85f;

    private void Awake()
    {
        // Unpause problematic TimeScale
        Time.timeScale = 1;
    }

    private void Start()
    {
        #if UNITY_EDITOR
        if (EditorComfort.instance.active) return;
        #endif

        StartCoroutine(Do());
        IEnumerator Do()
        {
            var menu = MenuManager.instance;
            if (menu != null) menu.freezeUp = true;
            _blackScreen.SetActive(true);
            yield return new WaitForSecondsRealtime(0.2f);

            _fadeOut.SetActive(true);
            _blackScreen.SetActive(false);
            yield return new WaitForSecondsRealtime(_fadeTime);

            _fadeOut.SetActive(false);       
            if (menu != null) menu.freezeUp = false;
        }
    }

    public void AsyncLoadScene(int level)
    {
        StartCoroutine(BeginningSceneLoading(level));
    }

    private IEnumerator BeginningSceneLoading(int level)
    {
        _fadeIn.SetActive(true);
        AudioManager.instance.Play(_transitionAudio);
        yield return new WaitForSecondsRealtime(_fadeTime);

        _loadingScreenCanvas.SetActive(true);

        // Starts the sequence.
        var async = SceneManager.LoadSceneAsync(level);
        StartCoroutine(WaitToLoadScene(async));
    }

    // Don't recommended to change.
    private IEnumerator WaitToLoadScene(AsyncOperation async)
    {
        async.allowSceneActivation = false;
        int frames = 0;

        while (async.progress < 0.89)
        {
            frames += 1;
            yield return new WaitForEndOfFrame();
        }

        while (frames < _minCooldown)
        {
            frames += 1;
            yield return new WaitForEndOfFrame();
        }

        async.allowSceneActivation = true;
    }
}
