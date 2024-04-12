using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetFramerate : MonoBehaviour
{
    private void Start()
    {
        DontDestroyOnLoad(this);

        Application.targetFrameRate = 60;
    }
}
