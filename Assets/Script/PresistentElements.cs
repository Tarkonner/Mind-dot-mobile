using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresistentElements : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
