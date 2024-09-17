using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Levelbank", menuName = "Level Bank")]
public class LevelsBank : ScriptableObject
{
    public LevelSO[] levels;

    private void OnValidate()
    {
        ResetSave();
    }

    private void ResetSave()
    {
        if (ES3.FileExists())
        {
            for (int i = 0; i < levels.Length; i++)
            {
                string key = SaveSystem.levelKey + i.ToString();

                if (ES3.KeyExists(key))
                {
                    bool levelState = ES3.Load<bool>(key);
                    if (levelState)
                        ES3.Save(key, false);
                }
                else
                {
                    ES3.Save(key, false);
                }
            }
        }
    }
}
