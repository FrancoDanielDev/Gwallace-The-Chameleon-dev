using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerTechs
{
    Player _player;
    GameObject _model;
    Animator _animator;

    public PlayerTechs(Player player, GameObject model, Animator animator)
    {
        _player = player;
        _model = model;
        _animator = animator;
    }

    public void Start()
    {
        Spawn(true);
    }

    public void EndVoyage()
    {
        _player.VoyageOff();

        var menu = MenuManager.instance;

        menu.freezeUp = true;
        _player.Freeze();
        _model.SetActive(false);
        _player.Particle.Confetti.Play();
        AudioManager.instance.Play(_player.SFX.Confetti);
    }

    #region Spawning

    public void EvaluteGoingNextLevel()
    {
        if (GameDataManager.instance.GetInt("Mini Voyage Mode") == 1)
        {
            EndVoyage();
        }
        else
        {
            GoToNextLevel();
        }
    }

    public void GoToNextLevel()
    {
        _player.StartCoroutine(Do());

        IEnumerator Do()
        {
            var menu = MenuManager.instance;
            var ui = UIManager.instance;
            var level = LevelManager.instance;

            menu.freezeUp = true;
            _player.Freeze();
            _model.SetActive(false);
            _player.Particle.LeavesExplosion.Play();
            yield return new WaitForSeconds(1f);

            AudioManager.instance.Play(_player.SFX.LevelTransition);
            ui.TransitionIn(true);
            yield return new WaitForSeconds(1.5f);

            level.NewLevel();
            Spawn();
            yield return new WaitForSeconds(0.3f);

            _player.SpawnParticle(_player.Particle.LeavesTrail);
            _model.SetActive(true);
            _player.Unfreeze();
            yield return new WaitForSeconds(0.3f);

            AudioManager.instance.Play(_player.SFX.LeavesInvocation);
            _player.Particle.LeavesInvocation.Play();
            ui.TransitionOut(true);
            ui.TransitionIn(false);
            yield return new WaitForSeconds(1.5f);

            ui.TransitionOut(false);
            menu.freezeUp = false;
        }
    }

    public void DieAndSpawn()
    {
        _player.StartCoroutine(Do());

        IEnumerator Do()
        {
            AudioManager.instance.Play(_player.SFX.Death);
            _animator.SetBool("Dead", true);
            _animator.SetTrigger("Die");
            _player.Particle.Death.Play();
            _player.Freeze();
            yield return new WaitForSeconds(0.7f);

            if (_player.IsVoyageAvailable)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                yield return null;
            }

            Spawn();
            _animator.SetBool("Dead", false);
            yield return new WaitForSeconds(0.1f);

            _player.Unfreeze();
        }
    }

    private void Spawn(bool spawning = false)
    {
        _player.Movement._xAxis = 0;
        var spawner = LevelManager.instance;

        if (!spawning) EventManager.instance.Trigger(Events.ResetParameters);
        else spawner.SetLevel(GameDataManager.instance.GetInt("Chosen Level"));

        _player.transform.position = spawner.SpawnPosition();
        _player.FormSwitch.InitializeForms();
        _player.Movement.ShouldFlipRight();
    }

    #endregion

    #region Updatable

    /*public void ResetScene()
    {
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.P)) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        #endif
    }*/

    public void ResetLevel()
    {
        //if (Input.GetButtonDown("Reset")) DieAndSpawn();
        if (MyInput.instance.GetKeyDown(ActionKeybind.ResetKeybind)) DieAndSpawn();
    }

    /*public void TPCheating()
    {
        TP(KeyCode.Alpha1, 0);
        TP(KeyCode.Alpha2, 1);
        TP(KeyCode.Alpha3, 2);
        TP(KeyCode.Alpha4, 3);
        TP(KeyCode.Alpha5, 4);
        TP(KeyCode.Alpha6, 5);
        TP(KeyCode.Alpha7, 6);
        TP(KeyCode.Alpha8, 7);
        TP(KeyCode.Alpha9, 8);
        TP(KeyCode.Alpha0, 9);
        TP(KeyCode.Keypad1, 10);

        void TP(KeyCode key, int index)
        {
            if (Input.GetKeyDown(key))
            {
                var level = LevelManager.instance;
                level.SetLevel(index + 1);
                _player.transform.position = level.SpawnPosition();
                _player.FormSwitch.InitializeForms();
                EventManager.instance.Trigger(Events.ResetParameters);
            }
        }
    }*/

    #endregion
}
