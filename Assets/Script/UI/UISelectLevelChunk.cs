using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISelectLevelChunk : MonoBehaviour
{
    public void GiveLevelChunk(LevelsBank levelChunk) => DataBetweenLevels.Instance.currentLevelChunk = levelChunk;
}
