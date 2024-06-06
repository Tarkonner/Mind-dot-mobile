using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Levelbank", menuName = "Level Bank")]
public class LevelsBank : ScriptableObject
{
    public LevelSO[] levels;
}
