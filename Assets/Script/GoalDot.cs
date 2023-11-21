using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalDot : MonoBehaviour
{
    [SerializeField] public Vector2 gridPos;
    [SerializeField] public DotType dotType;

    void Start()
    {
        Image renderer = GetComponent<Image>();

        switch (dotType)
        {
            case DotType.Red:
                renderer.color = Color.red;
                break;
            case DotType.Blue:
                renderer.color = Color.blue;
                break;
            case DotType.Yellow:
                renderer.color = Color.yellow;
                break;
        }
    }
}
