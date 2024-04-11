using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelText : MonoBehaviour
{
    TextMeshProUGUI textMesh;

    void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    public void LevelIndex(int levelIndex)
    {
        textMesh.text = "Level " + (levelIndex + 1).ToString();
    }
}
