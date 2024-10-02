using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadIntoGame : MonoBehaviour
{
    void Start()
    {
        if(SceneManager.GetActiveScene().buildIndex == 0)
            SceneManager.LoadScene(1);
        else
            Destroy(gameObject);
    }
}
