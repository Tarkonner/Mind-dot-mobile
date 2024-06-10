using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataBetweenLevels : MonoBehaviour
{
    public static DataBetweenLevels Instance;

    public int targetLevel = 0;

    private void Awake()
    {
        Instance = this;
    }
}