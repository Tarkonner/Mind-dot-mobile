using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [Header("Holders")]
    [SerializeField] GameObject goalHolder;
    [SerializeField] GameObject pieceHolder;

    [SerializeField] GameObject[] pices;
    [SerializeField] GameObject[] goals;

    void Start()
    {
        //Goal
        for (int i = 0; i < goals.Length; i++)
        {
            GameObject spawn = Instantiate(goals[i], transform);
            spawn.transform.SetParent(goalHolder.transform);
        }

        //Pieces
        for (int i = 0;i < pices.Length; i++)
        {
            GameObject spawn = Instantiate(pices[i], transform);
            spawn.transform.SetParent(pieceHolder.transform);
        }
    }
}
