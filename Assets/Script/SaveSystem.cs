using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    [SerializeField] LevelsBank levelsBank;

    public static string levelKey = "levelKey";

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        ES3.Init();

        if(!ES3.FileExists())
        {
            for (int i = 0; i < levelsBank.levels.Length; i++)
            {
                string key = levelKey + i.ToString();
                ES3.Save(key, false);
            }
        }
    }

    [ContextMenu("Delete file")]
    public void DeleteFile()
    {
        if(ES3.FileExists())
            ES3.DeleteFile();
    }
}
