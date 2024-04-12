using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FrameCounter : MonoBehaviour
{
    private int lastFrameIndex;
    private float[] frameDeltaTimeArray;

    TextMeshProUGUI textMesh;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();

        frameDeltaTimeArray = new float[50];
    }

    void Update()
    {
        frameDeltaTimeArray[lastFrameIndex] = Time.deltaTime;
        lastFrameIndex = (lastFrameIndex + 1) % frameDeltaTimeArray.Length; ;

        textMesh.text = Mathf.RoundToInt(CalculateFPS()).ToString();
    }

    private float CalculateFPS()
    {
        float total = 0;
        foreach (float deltaTime in frameDeltaTimeArray)
            total += deltaTime;
        return frameDeltaTimeArray.Length / total;
    }
}
