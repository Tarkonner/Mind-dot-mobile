using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPools : MonoBehaviour
{
    [Header("Dot pool")]
    [SerializeField] GameObject dotPrefab;
    public ObjectPool<GameObject> dotPool;
    [SerializeField] int defultSpawnedDots = 50;
    [SerializeField] int maxSpawnedDots = 200;



    private void Awake()
    {
        DontDestroyOnLoad(this);

        dotPool = new ObjectPool<GameObject>(() => { return Instantiate(dotPrefab); },
            dot => { dot.SetActive(true); },    //Get
            dot => { dot.SetActive(false); },   //Return
            dot => { Destroy(dot); },           //Destory
            true,
            defultSpawnedDots,
            maxSpawnedDots);


    }

    public void Setup()
    {

    }
}
