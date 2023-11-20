using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [SerializeField] private string levelFolderPath;

    private void Awake()
    {
        instance = this;

        DontDestroyOnLoad(gameObject);

        ES3.Init();
    }


    private void Start()
    {
        SaveLevel();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            LoadLevel(0);
    }

    public void LoadLevel(int level)
    {
        LevelData l = ES3.Load<LevelData>("1");
        Debug.Log(l.test);
    }

    public void SaveLevel()
    {
        //Test level
        LevelData level = new LevelData();
        level.test = "testing";
        level.levelIndex = 1;

        string key = level.levelIndex.ToString();

        //Gate
        if(ES3.KeyExists(key))
        {
            Debug.Log("Need new key for level");
            return;
        }
        else
        {
            Debug.Log("New file added");
            ES3.Save(key, level);
        }

    }

    public void LoadScene(int level)
    {
        SceneManager.LoadScene(level);            
    }


}
