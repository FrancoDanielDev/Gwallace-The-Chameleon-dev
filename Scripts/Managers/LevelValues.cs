using UnityEngine;

[CreateAssetMenu(fileName = "Level Values", menuName = "Level Values/Level Values")]
public class LevelValues : ScriptableObject
{
    [System.Serializable]
    public struct Levels
    {
        public string level;
        public FormName Form1;
        public FormName Form2;
        public Vector3 spawnLocation;

        public Levels(string lvl, FormName form1, FormName form2, Vector3 loc)
        {
            level = lvl;
            Form1 = form1;
            Form2 = form2;
            spawnLocation = loc;
        }
    }

    public Levels[] levels;

    public FormName[] CurrentForms(int levelCount)
    {
        FormName[] forms = new FormName[2];
        forms[0] = levels[levelCount].Form1;
        forms[1] = levels[levelCount].Form2;
        return forms;
    }
}
