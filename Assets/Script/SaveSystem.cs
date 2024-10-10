using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    [SerializeField] LevelsBank[] levelsChunks;

    public static string levelKey = "levelKey_";

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        ES3.Init();

        if(!ES3.FileExists())
            MakeSaveFile();
    }

    [ContextMenu("Delete file")]
    public void DeleteFile()
    {
        if(ES3.FileExists())
        {
            ES3.DeleteFile();
            MakeSaveFile();
        }
    }

    private void MakeSaveFile()
    {
        //Target
        ES3.Save("SelectLevelChunk", "begin");


        foreach (var chunk in levelsChunks)
        {
            //Level chunks data
            ES3.Save(chunk.name, 0);

            //Levels
            for (int i = 0; i < chunk.levels.Length; i++)
            {
                string key = levelKey + chunk.name + i.ToString();
                ES3.Save(key, false);
            }
        }
    }
}