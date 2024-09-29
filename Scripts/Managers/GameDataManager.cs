using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

// Using a PlayerPrefs system.
public class GameDataManager : MonoBehaviour
{
    #region Variables

    public static GameDataManager instance;

    [Serializable]
    private class PlayerPrefsVariable<T>
    {
        public string key;
        [ReadOnly]
        public T value;
        public T initialValue;
    }

    [SerializeField] private List<PlayerPrefsVariable<string>> _stringVariables = new();
    [SerializeField] private List<PlayerPrefsVariable<int>>    _intVariables    = new();
    [SerializeField] private List<PlayerPrefsVariable<float>>  _floatVariables  = new();

    #endregion

    #region Initialization

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        LoadData();
    }

    #endregion

    #region Save, Load & Reset

    public void SaveData()
    {
        SaveVariables(_stringVariables);
        SaveVariables(_intVariables);
        SaveVariables(_floatVariables);
        PlayerPrefs.Save();
    }

    public void LoadData()
    {
        LoadVariables(_stringVariables);
        LoadVariables(_intVariables);
        LoadVariables(_floatVariables);
    }

    public void ResetData()
    {
        ResetVariables(_stringVariables);
        ResetVariables(_intVariables);
        ResetVariables(_floatVariables);
    }

    private void SaveVariables<T>(List<PlayerPrefsVariable<T>> variables)
    {
        foreach (var variable in variables)
            if (variable.value != null)
                PlayerPrefs.SetString(variable.key, variable.value.ToString());
    }

    private void LoadVariables<T>(List<PlayerPrefsVariable<T>> variables)
    {
        foreach (var variable in variables)
        {
            if (PlayerPrefs.HasKey(variable.key))
                variable.value = (T)Convert.ChangeType(PlayerPrefs.GetString(variable.key), typeof(T));
            else
                variable.value = variable.initialValue;
        }
    }

    private void ResetVariables<T>(List<PlayerPrefsVariable<T>> variables)
    {
        foreach (var variable in variables)
        {
            PlayerPrefs.DeleteKey(variable.key);
            variable.value = variable.initialValue;
        }
    }

    #endregion

    #region Reseters

    public void ResetInt(string key)
    {
        ResetVariable(_intVariables, key);
    }

    public void ResetString(string key)
    {
        ResetVariable(_stringVariables, key);
    }

    public void ResetFloat(string key)
    {
        ResetVariable(_floatVariables, key);
    }

    private void ResetVariable<T>(List<PlayerPrefsVariable<T>> variables, string key)
    {
        if (variables.Any(v => v.key == key))
        {
            var variable = variables.First(v => v.key == key);
            //PlayerPrefs.DeleteKey(variable.key);
            variable.value = variable.initialValue;
        }
    }

    #endregion

    #region Getters

    public int GetInt(string key) => _intVariables.FirstOrDefault(v => v.key == key)?.value ?? 0;
    public string GetString(string key) => _stringVariables.FirstOrDefault(v => v.key == key)?.value ?? "";
    public float GetFloat(string key) => _floatVariables.FirstOrDefault(v => v.key == key)?.value ?? 0f;

    #endregion

    #region Setters

    public void SetInt(string key, int value) => SetVariable(_intVariables, key, value);
    public void SetString(string key, string value) => SetVariable(_stringVariables, key, value);
    public void SetFloat(string key, float value) => SetVariable(_floatVariables, key, value);

    private void SetVariable<T>(List<PlayerPrefsVariable<T>> variables, string key, T value)
    {
        if (variables.Any(v => v.key == key))
        {
            var variable = variables.First(v => v.key == key);
            variable.value = value;
            SaveData();
        }
    }

    #endregion
}
